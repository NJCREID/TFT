using System.Text.Json;
using System.Text.Json.Nodes;

namespace TFT_API.Services
{
    /// <summary>
    /// Service for checking and updating data related to Teamfight Tactics (TFT).
    /// </summary>
    public class DataCheckService(
        RiotApiService riotApiService,
        TeamBuilderService teamBuilderService,
        StatisticsService statisticsService,
        CooccurrenceService cooccurrenceStats,
        TFTUpdateDataService tftUpdateDataService,
        ILogger<DataCheckService> logger,
        HttpClient httpClient,
        IConfiguration configuration
        )
    {
        private readonly RiotApiService _riotApiService = riotApiService;
        private readonly TeamBuilderService _teamBuilderService = teamBuilderService;
        private readonly StatisticsService _statisticsService = statisticsService;
        private readonly CooccurrenceService _cooccurrenceStats = cooccurrenceStats;
        private readonly TFTUpdateDataService _TFTUpdateDataService = tftUpdateDataService;
        private readonly ILogger<DataCheckService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly HttpClient _httpClient = httpClient;

        // URL for fetching champion data, with a placeholder for the set
        private readonly string _url = $"https://tft.dakgg.io/api/v1/data/champions?hl=en&season=set{{set}}";
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() {WriteIndented = true};

        /// <summary>
        /// Starts the manual data check process.
        /// </summary>
        /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
        public async Task RunCheckAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Manual Data Check is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation("Manual Data Check is stopping."));

            await ProcessUpdateCheckAsync();

            _logger.LogInformation("Manual Data Check completed.");
        }

        /// <summary>
        /// Processes various tasks during the data check.
        /// </summary>
        private async Task ProcessUpdateCheckAsync()
        {
            // Retrieve the current set number from configuration
            var setValue = _configuration["TFT:Set"];
            if (!int.TryParse(setValue, out int setNumber)) return;

            // Fetch the latest patch version
            var latestPatch = await FetchLatestPatchAsync();
            var currentPatch = _configuration["TFT:Patch"];

            // Check if the patch version is up-to-date
            if (latestPatch == null || latestPatch == currentPatch)
            {
                _logger.LogInformation("Patch version is up-to-date.");
                return;
            }

            _logger.LogInformation("Updated data detected. Starting tasks...");

            await RunUpdateTasksAsync();

            // Check if there is new set data available
            bool isNewSet = await CheckForUpdatedDataAsync(setNumber);
            if (isNewSet)
            {
                // Update set data if new data is detected
                await _TFTUpdateDataService.UpdateSetDataAsync();
                _logger.LogInformation("Items, augments, champions, and traits updated.");
            }
            UpdateConfiguration("TFT:Patch", latestPatch);
        }

        /// <summary>
        /// Fetches the latest patch version from the API.
        /// </summary>
        /// <returns>The latest patch version, or null if the request fails.</returns>
        private async Task<string?> FetchLatestPatchAsync()
        {
            var patchResponse = await _httpClient.GetAsync("https://ddragon.leagueoflegends.com/api/versions.json");
            if (!patchResponse.IsSuccessStatusCode) return null;

            var patchJson = await patchResponse.Content.ReadAsStringAsync();
            var patchVersions = JsonSerializer.Deserialize<List<string>>(patchJson);
            return patchVersions?.FirstOrDefault();
        }

        /// <summary>
        /// Runs tasks that update the game data.
        /// </summary>
        private async Task RunUpdateTasksAsync()
        {
            await _riotApiService.FetchMatchHistoryAsync();
            _logger.LogInformation("Riot API data fetch completed.");

            await _cooccurrenceStats.ProcessMatches();
            _logger.LogInformation("Co-occurrence stats processing completed.");

            await _teamBuilderService.BuildTeams();
            _logger.LogInformation("Team build process completed.");

            await _statisticsService.CalculateAndStoreStatisticsAsync();
            _logger.LogInformation("Statistics calculation completed.");
        }

        /// <summary>
        /// Checks if there is updated data for the given set.
        /// </summary>
        /// <param name="set">The current set number.</param>
        /// <returns>True if updated data is available, otherwise false.</returns>
        private async Task<bool> CheckForUpdatedDataAsync(int set)
        {
            set++;
            var url = _url.Replace("{set}", set.ToString());
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return false;
            var isUpdated = await CheckForUpdatedDataAsync(set);
            if (!isUpdated)
            {
                UpdateConfiguration("TFT:Set", set.ToString());
                return true;
            }
            return true;
        }

        /// <summary>
        /// Updates configuration values in appsettings.json.
        /// </summary>
        /// <param name="key">The configuration key to update.</param>
        /// <param name="newValue">The new value to set for the configuration key.</param>
        private static void UpdateConfiguration(string key, string newValue)
        {
            var filePath = "appsettings.json";
            var json = File.ReadAllText(filePath);
            using JsonDocument doc = JsonDocument.Parse(json);
            var jsonObject = new JsonObject();

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                    jsonObject[property.Name] = property.Name == key 
                    ? JsonValue.Create(newValue)
                    : JsonSerializer.SerializeToNode(property.Value);
            }

            var updatedJson = JsonSerializer.Serialize(jsonObject, _jsonSerializerOptions);
            File.WriteAllText(filePath, updatedJson);
        }
    }
}
