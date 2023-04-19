using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace ApiAzureConfiguration.Extensions;

public static class AzureAppConfigurationExtensions
{
    public const string AzureAppConfiguration = "AppConfiguration";
    public const string CoreApi = "CoreApi";

    /// <summary>
    /// Method that will configure an refresher in AppConfiguration settings in Azure.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns></returns>
    public static WebApplicationBuilder ConfigureAzureAppConfigRefresher(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(builder.Configuration.GetConnectionString(AzureAppConfiguration))
                .Select($"*")
                //.Select(KeyFilter.Any, "UK")
                .ConfigureRefresh(refreshOptions =>
                {
                    // When this value ["CoreApi:Settings:Sentinel"] changes, on the next refresh operation, the config will update all modified configs since it was last refreshed.
                    refreshOptions.Register(key: $"{CoreApi}:Settings:Sentinel", LabelFilter.Null, refreshAll: true);

                    // Set a timeout for the cache so that it will poll the azure config every X timespan. [15 Seconds]
                    refreshOptions.SetCacheExpiration(cacheExpiration: TimeSpan.FromSeconds(15));
                });
        });

        builder.Services.AddAzureAppConfiguration();

        return builder;
    }
}
