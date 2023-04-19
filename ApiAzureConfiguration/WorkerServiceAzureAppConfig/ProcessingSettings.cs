namespace WorkerServiceAzureAppConfig
{
    public class ProcessingSettings
    {
        public WorkerSettings Worker { get; set; } = null!;
    }

    public class WorkerSettings
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Delay { get; set; }
    }
}
