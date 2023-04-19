using ApiAzureConfiguration.Configuration;
using ApiAzureConfiguration.Services;

namespace ApiAzureConfiguration.Extensions;

/// <summary>
/// Configuration related to CORS
/// </summary>
public static class CorsExtensions
{
    public const string CoreApi = "CoreApi";

    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        var corsSettingsSection = builder.Configuration.GetSection($"{CoreApi}:{CorsSettings.SettingsKey}");
        builder.Services.Configure<CorsSettings>(corsSettingsSection);

        var allowedOrigins = corsSettingsSection.GetValue<string>(nameof(CorsSettings.AllowedOrigins));
        var origins = allowedOrigins?.Split(",", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        if (origins.Any())
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsRefresherService.PolicyFrontend, corsBuilder =>
                {
                    corsBuilder = corsBuilder.WithOrigins(origins);

                    if (builder.Environment.IsDevelopment())
                        corsBuilder = corsBuilder.SetIsOriginAllowedToAllowWildcardSubdomains();

                    corsBuilder = corsBuilder
                        .AllowAnyHeader()
                        .AllowAnyMethod();

                    corsBuilder.Build();
                });
            });

            builder.Services.AddSingleton<CorsRefresherService>();
        }

        return builder;
    }
}



