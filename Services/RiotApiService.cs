using System.Text.Json;
using TFT_API.Data;
using TFT_API.Models.Match;
using TFT_API.Models.FetchResponse;

namespace TFT_API.Services
{
    /// <summary>
    /// Service for interacting with the Riot API to fetch match history data.
    /// </summary>
    public class RiotApiService(HttpClient httpClient, IConfiguration configuration, TFTContext context)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;
        private readonly TFTContext _context = context;
        private readonly RateLimiter _rateLimiter = new(perSecondLimit: 20, per2MinLimit: 100);
        private readonly HashSet<string> _processedMatchIds = [];

        /// <summary>
        /// Fetches the match history for challenger, grand master, and master players.
        /// </summary>
        public async Task FetchMatchHistoryAsync()
        {
            var regionServerInfo = _configuration.GetSection("RegionServerInfo").Get<Dictionary<string, RegionServerInfo>>();
            var leagues = _configuration.GetSection("Leagues").Get<Dictionary<string, string>>();
            if (regionServerInfo == null || leagues == null) return;

            var summonerEndpoint = _configuration["Endpoints:GetSummonerBySummonerId"];
            var matchIdsEndpoint = _configuration["Endpoints:GetMatchIdsByPuuid"];
            var matchEndpoint = _configuration["Endpoints:GetMatchByMatchId"];

            if(summonerEndpoint == null || matchIdsEndpoint == null || matchEndpoint == null) return;

            // Process each league to fetch the match history.
            foreach (var league in leagues)
            {
                var leagueName = league.Key.Replace("Get", "").Replace("League", "");
                await ProccessLeagueAsync(league.Value, leagueName, regionServerInfo, summonerEndpoint, matchIdsEndpoint, matchEndpoint);
            }
        }

        /// <summary>
        /// Processes the league by fetching summoner information and match IDs.
        /// </summary>
        private async Task ProccessLeagueAsync(string leagueEndpoint, string leagueName, Dictionary<string, RegionServerInfo> regionServerInfo, string summonerEndpoint, string matchIdsEndpoint, string matchEndpoint)
        {
            if (leagueEndpoint == null) return;
            // Iterate through each region to fetch league data.
            foreach (var region in regionServerInfo)
            {
                var fetchedLeague = await FetchRequestAsync<FetchedLeague>(leagueEndpoint, region.Value.ServerCode);
                if (fetchedLeague == null) continue;

                // Process each summoner (player) in the league.
                foreach (var entry in fetchedLeague.Entries)
                {
                    await ProcessSummonerAsync(entry.SummonerId, region.Value, leagueName, summonerEndpoint, matchIdsEndpoint, matchEndpoint);
                }
            }
        }

        /// <summary>
        /// Processes individual summoner by fetching their match IDs and match data.
        /// </summary>
        private async Task ProcessSummonerAsync(string summonerId, RegionServerInfo regionInfo, string leagueName, string summonerEndpoint, string matchIdsEndpoint,string matchEndpoint)
        {
            var summonerUrl = summonerEndpoint.Replace("{encryptedSummonerId}", summonerId);
            var fetchedSummoner = await FetchRequestAsync<FetchedSummoner>(summonerUrl, regionInfo.ServerCode);
            if (fetchedSummoner == null) return;


            var matchIdsUrl = matchIdsEndpoint.Replace("{puuid}", fetchedSummoner.Puuid);
            var fetchedMatchIds = await FetchRequestAsync<List<string>>(matchIdsUrl, regionInfo.ServerLocation);
            if (fetchedMatchIds == null) return;

            // Process each match ID for the summoner.
            foreach (var matchId in fetchedMatchIds)
            {
                if (_processedMatchIds.Contains(matchId)) continue;
                await ProcessMatchAsync(matchId, fetchedSummoner.Puuid, regionInfo.ServerLocation, leagueName, matchEndpoint);
                _processedMatchIds.Add(matchId);
            }
        }

        /// <summary>
        /// Processes match data by fetching details and saving to the database.
        /// </summary>
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

        /// <summary>
        /// Fetches data from the specified API endpoint.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="endpoint">The API endpoint to fetch data from.</param>
        /// <param name="serverCode">The server code to use for the request.</param>
        /// <returns>The deserialized response or default value on failure.</returns>
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

        /// <summary>
        /// Class to represent region server information.
        /// </summary>
        public class RegionServerInfo
        {
            public string ServerCode { get; set; } = string.Empty;
            public string ServerLocation { get; set; } = string.Empty;
        }
    }
}
