using Microsoft.Extensions.Options;

namespace WorkerServiceAzureAppConfig
{
    public class WorkerRefresherService
    {
        private readonly WorkerSettings _workerSettings;

        public WorkerRefresherService(IOptions<WorkerSettings> workerOptions, IOptionsMonitor<WorkerSettings> workerMonitored)
        {
            _workerSettings = workerOptions.Value;
            IOptionsMonitor<WorkerSettings> _workerMonitored = workerMonitored;

            _workerMonitored.OnChange(newSettings =>
            {
                Console.WriteLine(newSettings.Name);
            });
        }
    }
}
