namespace TFT_API.Services
{
    public class StatisticsBackgroundService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public StatisticsBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var statisticsService = scope.ServiceProvider.GetRequiredService<StatisticsService>();
                await statisticsService.CalculateAndStoreStatisticsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
