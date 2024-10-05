namespace TFT_API.Services
{
    /// <summary>
    /// RateLimiter class to manage API call limits for different regions.
    /// </summary>
    public class RateLimiter(int perSecondLimit, int per2MinLimit)
    {
        // Maximum number of calls allowed per second and per two minutes.
        private readonly int perSecondLimit = perSecondLimit;
        private readonly int per2MinLimit = per2MinLimit;
        // Dictionary to keep track of call timestamps for each region.
        private readonly Dictionary<string, Queue<DateTime>> regionCallsPerSecond = [];
        private readonly Dictionary<string, Queue<DateTime>> regionCallsPer2Min = [];


        /// <summary>
        /// Cleans up the queue by removing timestamps that fall outside the specified time window.
        /// </summary>
        /// <param name="callsQueue">Queue containing call timestamps.</param>
        /// <param name="currentTime">The current time for comparison.</param>
        /// <param name="window">The time window to keep calls within.</param>
        private static void Cleanup(Queue<DateTime> callsQueue, DateTime currentTime, TimeSpan window)
        {
            while (callsQueue.Count > 0 && (currentTime - callsQueue.Peek()) > window)
            {
                callsQueue.Dequeue();
            }
        }

        /// <summary>
        /// Checks if a call can be made for the specified region, applying rate limits.
        /// </summary>
        /// <param name="region">The region for which the call is being made.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating whether the call can be made.</returns>
        public async Task<bool> CanMakeCallAsync(string region)
        {
            DateTime currentTime = DateTime.UtcNow;

            // Retrieve or create a queue for the region's calls.
            if (!regionCallsPerSecond.TryGetValue(region, out Queue<DateTime>? calls))
            {
                calls = new Queue<DateTime>();
                regionCallsPerSecond[region] = calls;
                regionCallsPer2Min[region] = new Queue<DateTime>();
            }

            var callsPerSecond = calls;
            var callsPer2Min = regionCallsPer2Min[region];

            // Clean up old calls from the queues.
            Cleanup(callsPerSecond, currentTime, TimeSpan.FromSeconds(1));
            Cleanup(callsPer2Min, currentTime, TimeSpan.FromMinutes(2));

            // Check if the calls per second limit has been reached.
            if (callsPerSecond.Count >= perSecondLimit)
            {
                var waitTime = TimeSpan.FromSeconds(1) - (currentTime - callsPerSecond.Peek());
                await Task.Delay(waitTime);
            }

            // Check if the calls per 2 minutes limit has been reached.
            if (callsPer2Min.Count >= per2MinLimit)
            {
                var waitTime = TimeSpan.FromMinutes(2) - (currentTime - callsPer2Min.Peek());
                await Task.Delay(waitTime);
            }

            return true;
        }

        /// <summary>
        /// Records a call for the specified region, adding the current timestamp to the queues.
        /// </summary>
        /// <param name="region">The region for which the call is being recorded.</param>
        public void RecordCall(string region)
        {
            DateTime currentTime = DateTime.UtcNow;

            // Retrieve or create a queue for the region's call timestamps.
            if (!regionCallsPerSecond.TryGetValue(region, out Queue<DateTime>? time))
            {
                time = new Queue<DateTime>();
                regionCallsPerSecond[region] = time;
                regionCallsPer2Min[region] = new Queue<DateTime>();
            }

            var callsPerSecond = time;
            var callsPer2Min = regionCallsPer2Min[region];

            callsPerSecond.Enqueue(currentTime);
            callsPer2Min.Enqueue(currentTime);
        }
    }
}
