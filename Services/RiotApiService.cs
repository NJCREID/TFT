using System.Text.Json;
using TFT_API.Data;
using TFT_API.Models.Match;
using TFT_API.Models.FetchResponse;
using static TFT_API.Services.RiotApiService;
using System.Threading.RateLimiting;

namespace TFT_API.Services
{
    public class RiotApiService(HttpClient httpClient, IConfiguration configuration, TFTContext context)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;
        private readonly TFTContext _context = context;
        private readonly RateLimiter _rateLimiter = new(perSecondLimit: 20, per2MinLimit: 100);
        private readonly HashSet<string> _processedMatchIds = [];

        public async Task FetchChallengerMatchHistoryAsync()
        {
            var regionServerInfo = _configuration.GetSection("RegionServerInfo").Get<Dictionary<string, RegionServerInfo>>();
            var leagues = _configuration.GetSection("Leagues").Get<Dictionary<string, string>>();
            if (regionServerInfo == null || leagues == null) return;

            var summonerEndpoint = _configuration["Endpoints:GetSummonerBySummonerId"];
            var matchIdsEndpoint = _configuration["Endpoints:GetMatchIdsByPuuid"];
            var matchEndpoint = _configuration["Endpoints:GetMatchByMatchId"];

            if(summonerEndpoint == null || matchIdsEndpoint == null || matchEndpoint == null) return;

            foreach (var league in leagues)
            {
                var leagueName = league.Key.Replace("Get", "").Replace("League", "");
                await ProccessLeagueAsync(league.Value, leagueName, regionServerInfo, summonerEndpoint, matchIdsEndpoint, matchEndpoint);
            }
        }

        private async Task ProccessLeagueAsync(string leagueEndpoint, string leagueName, Dictionary<string, RegionServerInfo> regionServerInfo, string summonerEndpoint, string matchIdsEndpoint, string matchEndpoint)
        {
            if (leagueEndpoint == null) return;
            foreach(var region in regionServerInfo)
            {
                var fetchedLeague = await FetchRequestAsync<FetchedLeague>(leagueEndpoint, region.Value.ServerCode);
                if (fetchedLeague == null) continue;

                foreach(var entry in fetchedLeague.Entries)
                {
                    await ProcessSummonerAsync(entry.SummonerId, region.Value, leagueName, summonerEndpoint, matchIdsEndpoint, matchEndpoint);
                }
            }
        }


        private async Task ProcessSummonerAsync(string summonerId, RegionServerInfo regionInfo, string leagueName, string summonerEndpoint, string matchIdsEndpoint,string matchEndpoint)
        {
            var summonerUrl = summonerEndpoint.Replace("{encryptedSummonerId}", summonerId);
            var fetchedSummoner = await FetchRequestAsync<FetchedSummoner>(summonerUrl, regionInfo.ServerCode);
            if (fetchedSummoner == null) return;


            var matchIdsUrl = matchIdsEndpoint.Replace("{puuid}", fetchedSummoner.Puuid);
            var fetchedMatchIds = await FetchRequestAsync<List<string>>(matchIdsUrl, regionInfo.ServerLocation);
            if (fetchedMatchIds == null) return;

            foreach(var matchId in fetchedMatchIds)
            {
                if (_processedMatchIds.Contains(matchId)) continue;
                await ProcessMatchAsync(matchId, fetchedSummoner.Puuid, regionInfo.ServerLocation, leagueName, matchEndpoint);
                _processedMatchIds.Add(matchId);
            }
        }

        private async Task ProcessMatchAsync(string matchId, string puuid, string serverLocation, string leagueName, string matchEndpoint)
        {
            var matchUrl = matchEndpoint.Replace("{matchId}", matchId);
            var match = await FetchRequestAsync<FetchedMatch>(matchUrl, serverLocation);
            if (match == null || match.Info.TftGameType != "standard" ||
                !long.TryParse(_configuration["TFT:Patch"], out var patchNumber) ||
                match.Info.TftSetNumber != patchNumber)
            {
                return;
            }

            var targetParticipant = match.Info.Participants.Find(p => p.Puuid == puuid);
            if (targetParticipant is null) return;

            var matchEntity = new Match
            {
                Puuid = puuid,
                Placement = targetParticipant.Placement,
                Units = targetParticipant.Units.Select(u => new MatchUnit
                {
                    CharacterId = u.CharacterId,
                    ItemNames = u.ItemNames,
                    Name = u.Name,
                    Rarity = u.Rarity,
                    Tier = u.Tier,
                }).ToList(),
                Traits = targetParticipant.Traits.Select(t => new MatchTrait
                {
                    Name = t.Name,
                    NumUnits = t.NumUnits,
                    Style = t.Style,
                    TierCurrent = t.TierCurrent,
                    TierTotal = t.TierTotal,
                }).ToList(),
                Augments = [.. targetParticipant.Augments],
                League = leagueName
            };

            _context.Matches.Add(matchEntity);
            await _context.SaveChangesAsync();
        }

        private async Task<T?> FetchRequestAsync<T>(string endpoint, string serverCode)
        {
            var url = new UriBuilder($"https://{serverCode}.{_configuration["RiotApi:BaseApi"]}{endpoint}")
            {
                Query = $"api_key={_configuration["RiotApi:ApiKey"]}"
            };

            try
            {
                await _rateLimiter.CanMakeCallAsync(serverCode);

                var response = await _httpClient.GetAsync(url.ToString());
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                _rateLimiter.RecordCall(serverCode);

                return JsonSerializer.Deserialize<T>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.Error.WriteLine($"Request error: {e.Message}");
                return default;
            }
            catch (JsonException e)
            {
                Console.Error.WriteLine($"Deserialization error: {e.Message}");
                return default;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Unexpected error: {e.Message}");
                return default;
            }
        }

        public class RegionServerInfo
        {
            public string ServerCode { get; set; } = string.Empty;
            public string ServerLocation { get; set; } = string.Empty;
        }
    }
}
