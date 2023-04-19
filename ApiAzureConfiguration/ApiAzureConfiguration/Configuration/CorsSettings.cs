namespace ApiAzureConfiguration.Configuration
{
    public record CorsSettings
    {
        public static string SettingsKey => "CorsSettings";

        public string AllowedOrigins { get; init; } = string.Empty;
    }
}
