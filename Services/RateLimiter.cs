namespace TFT_API.Services
{
    public class RateLimiter(int perSecondLimit, int per2MinLimit)
    {
        private readonly int perSecondLimit = perSecondLimit;
        private readonly int per2MinLimit = per2MinLimit;
        private readonly Dictionary<string, Queue<DateTime>> regionCallsPerSecond = [];
        private readonly Dictionary<string, Queue<DateTime>> regionCallsPer2Min = [];

        private static void Cleanup(Queue<DateTime> callsQueue, DateTime currentTime, TimeSpan window)
        {
            while (callsQueue.Count > 0 && (currentTime - callsQueue.Peek()) > window)
            {
                callsQueue.Dequeue();
            }
        }

        public async Task<bool> CanMakeCallAsync(string region)
        {
            DateTime currentTime = DateTime.UtcNow;

            if (!regionCallsPerSecond.TryGetValue(region, out Queue<DateTime>? calls))
            {
                calls = new Queue<DateTime>();
                regionCallsPerSecond[region] = calls;
                regionCallsPer2Min[region] = new Queue<DateTime>();
            }

            var callsPerSecond = calls;
            var callsPer2Min = regionCallsPer2Min[region];

            Cleanup(callsPerSecond, currentTime, TimeSpan.FromSeconds(1));
            Cleanup(callsPer2Min, currentTime, TimeSpan.FromMinutes(2));

            if (callsPerSecond.Count >= perSecondLimit)
            {
                var waitTime = TimeSpan.FromSeconds(1) - (currentTime - callsPerSecond.Peek());
                await Task.Delay(waitTime);
            }

            if (callsPer2Min.Count >= per2MinLimit)
            {
                var waitTime = TimeSpan.FromMinutes(2) - (currentTime - callsPer2Min.Peek());
                await Task.Delay(waitTime);
            }

            return true;
        }

        public void RecordCall(string region)
        {
            DateTime currentTime = DateTime.UtcNow;

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
