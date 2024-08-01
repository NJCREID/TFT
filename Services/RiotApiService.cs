using System.Text.Json;
using TFT_API.Data;
using TFT_API.Models.Match;
using TFT_API.Models.FetchResponse;

namespace TFT_API.Services
{
    public class RiotApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly TFTContext _context;
        private readonly Dictionary<string, RequestCounter> _requestAmounts;
        private const int TwoMinutes = 1000 * 60 * 2;

        public RiotApiService(HttpClient httpClient, IConfiguration configuration, TFTContext context)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
            _requestAmounts = new Dictionary<string, RequestCounter>();
        }

        public async Task FetchChallengerMatchHistoryAsync()
        {
            var regionServerInfo = _configuration.GetSection("RegionServerInfo").Get<Dictionary<string, RegionServerInfo>>();
            var leagues = _configuration.GetSection("Leagues").Get<Dictionary<string, string>>();
            if (regionServerInfo == null || leagues == null) return;

            foreach (var leagueKey in leagues.Keys)
            {
                var leagueName = leagueKey.Replace("Get", "").Replace("League", "");
                var leagueEndpoint = leagues[leagueKey];
                if (leagueEndpoint == null) continue;

                foreach (var region in regionServerInfo.Keys)
                {
                    var fetchedLeague = await FetchRequestAsync<FetchedLeague>(leagueEndpoint, regionServerInfo[region].ServerCode);
                    if (fetchedLeague == null) continue;

                    foreach (var entry in fetchedLeague.entries)
                    {
                        var summonerEndpoint = _configuration["Endpoints:GetSummonerBySummonerId"];
                        if (summonerEndpoint == null) continue;
                        var summonerUrl = summonerEndpoint.Replace("{encryptedSummonerId}", entry.summonerId);
                        var fetchedSummoner = await FetchRequestAsync<FetchedSummoner>(summonerUrl, regionServerInfo[region].ServerCode);
                        if (fetchedSummoner == null) continue;

                        var matchIdsEndpoint = _configuration["Endpoints:GetMatchIdsByPuuid"];
                        if (matchIdsEndpoint == null) continue;
                        var matchIdsUrl = matchIdsEndpoint.Replace("{puuid}", fetchedSummoner.puuid);
                        var fetchedMatchIds = await FetchRequestAsync<List<string>>(matchIdsUrl, regionServerInfo[region].ServerLocation);
                        if (fetchedMatchIds == null) continue;

                        foreach (var matchId in fetchedMatchIds)
                        {
                            var matchEndpoint = _configuration["Endpoints:GetMatchByMatchId"];
                            if (matchEndpoint == null) continue;
                            var matchUrl = matchEndpoint.Replace("{matchId}", matchId);
                            var match = await FetchRequestAsync<FetchedMatch>(matchUrl, regionServerInfo[region].ServerLocation);
                            if (match == null) continue;

                            var targetParticipant = match.info.participants.Find(p => p.puuid == fetchedSummoner.puuid);
                            if (targetParticipant == null) continue;
                            var Match = new Match
                            {
                                Puuid = fetchedSummoner.puuid,
                                Placement = targetParticipant.placement,
                                Units = targetParticipant.units.Select(u => new MatchUnit
                                {
                                    CharacterId = u.character_id,
                                    ItemNames = u.itemNames,
                                    Name = u.name,
                                    Rarity = u.rarity,
                                    Tier = u.tier,
                                }).ToList(),
                                Traits = targetParticipant.traits.Select(t => new MatchTrait
                                {
                                    Name = t.name,
                                    NumUnits = t.num_units,
                                    Style = t.style,
                                    TierCurrent = t.tier_current,
                                    TierTotal = t.tier_total,
                                }).ToList(),
                                Augments = targetParticipant.augments.ToList(),
                                League = leagueName 
                            };
                            _context.Matches.Add(Match);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private async Task<T?> FetchRequestAsync<T>(string endpoint, string type)
        {
            if (_requestAmounts.TryGetValue(type, out var counter) && counter.Count == 100)
            {
                var now = DateTime.Now;
                var timeDif = now - counter.FirstTime;
                if (timeDif.TotalMilliseconds < TwoMinutes)
                {
                    await Task.Delay(TwoMinutes - (int)timeDif.TotalMilliseconds + 10000);
                }
                _requestAmounts[type] = new RequestCounter { Count = 0, FirstTime = DateTime.Now };
            }

            if (!_requestAmounts.ContainsKey(type))
            {
                _requestAmounts[type] = new RequestCounter { Count = 1, FirstTime = DateTime.Now };
            }
            else
            {
                _requestAmounts[type].Count++;
            }

            var url = new UriBuilder($"https://{type}.{_configuration["RiotApi:BaseApi"]}{endpoint}");
            url.Query = $"api_key={_configuration["RiotApi:ApiKey"]}";

            try
            {
                var response = await _httpClient.GetAsync(url.ToString());
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
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

        public class RequestCounter
        {
            public int Count { get; set; }
            public DateTime FirstTime { get; set; }
        }

        public class RegionServerInfo
        {
            public string ServerCode { get; set; } = string.Empty;
            public string ServerLocation { get; set; } = string.Empty;
        }
    }
}
