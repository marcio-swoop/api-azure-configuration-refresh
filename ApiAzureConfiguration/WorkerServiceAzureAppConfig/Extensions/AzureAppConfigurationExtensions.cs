using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace WorkerServiceAzureAppConfig.Extensions;

public static class AzureAppConfigurationExtensions
{
    public const string AzureAppConfiguration = "AppConfigConnectionString";
    public const string SentinelKey = "Settings:Sentinel";
    public const string SwoopCountry = "Swoop.Country";

    private static IConfigurationRefresher _refresher = null!;

    /// <summary>
    /// Method that will configure an refresher in AppConfiguration settings in Azure.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static WebApplicationBuilder ConfigureAzureAppConfiguration(this WebApplicationBuilder builder)
    {
        var labelCountryPrefix = GetLabelByCountryName(builder.Configuration);
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.ConfigureRefresher(builder.Configuration, labelCountryPrefix);
        });

        builder.Services.AddAzureAppConfiguration();

        return builder;
    }

    public static IHostBuilder ConfigureAzureAppConfigurationWorker(this IHostBuilder builder)
    {
        builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureAppConfiguration((hostContext, configBuilder) =>
            {
                var settings = configBuilder.Build();
                var labelCountryPrefix = GetLabelByCountryName(settings);

                configBuilder.AddAzureAppConfiguration(options =>
                {
                    options.ConfigureRefresher(settings, labelCountryPrefix);

                    _refresher = options.GetRefresher();
                });
            });

            webBuilder.ConfigureServices(ConfigureCustomServices);
        });

        return builder;
    }

    private static string GetLabelByCountryName(IConfiguration configuration)
    {
        var currentCountryName = configuration.GetValue<string>(SwoopCountry);

        if (string.IsNullOrEmpty(currentCountryName))
            throw new ArgumentException(SwoopCountry);

        return "UK";
    }

    private static AzureAppConfigurationOptions ConfigureRefresher(this AzureAppConfigurationOptions options, IConfiguration configuration, string labelCountryPrefix)
    {
        options.Connect(configuration.GetConnectionString(AzureAppConfiguration))
                .Select(KeyFilter.Any, labelCountryPrefix)
                .ConfigureRefresh(refreshOptions =>
                {
                    refreshOptions
                    // When this value ["Settings:Sentinel"] changes, on the next refresh operation, the config will update all modified configs since it was last refreshed.
                    .Register(key: $"{SentinelKey}", refreshAll: true)
                    // Set a timeout for the cache so that it will poll the azure config every X timespan. [10 Seconds]
                    .SetCacheExpiration(cacheExpiration: TimeSpan.FromSeconds(10));
                });

        return options;
    }

    private static void ConfigureCustomServices(IServiceCollection services)
    {
        services.AddSingleton(_refresher);
        services.AddAzureAppConfiguration();
    }
}
