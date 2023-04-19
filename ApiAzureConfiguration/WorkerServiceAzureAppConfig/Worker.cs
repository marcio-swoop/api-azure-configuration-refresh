using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace WorkerServiceAzureAppConfig
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRefresher _refresher;

        public Worker(
            ILogger<Worker> logger,
            IConfigurationRefresher refresher,
            IConfiguration configuration
            )
        {
            _logger = logger;

            _refresher = refresher;

            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
                _logger.LogInformation("-----");

                var workerName = _configuration["worker:name"];
                _logger.LogInformation("CONFIG Worker Name: {name}", workerName ?? "Hello from Code!");

                await _refresher.TryRefreshAsync();

                _logger.LogInformation("-----");
                await Task.Delay(6000, stoppingToken);
            }
        }
    }
}