using ApiAzureConfiguration.Configuration;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace ApiAzureConfiguration.Services
{
    public class CorsRefresherService
    {
        public const string PolicyFrontend = "FrontEnd";

        private readonly CorsOptions _corsOptions;

        public CorsRefresherService(IOptions<CorsOptions> corsOptions, IOptionsMonitor<CorsSettings> corsSettings)
        {
            _corsOptions = corsOptions.Value;
            IOptionsMonitor<CorsSettings> _corsSettings = corsSettings;

            _corsSettings.OnChange(newSettings =>
            {
                var currentPolicy = _corsOptions.GetPolicy(PolicyFrontend);
                if (currentPolicy != default)
                {
                    var allowedOrigins = newSettings.AllowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
                    var policyBuilder = new CorsPolicyBuilder(currentPolicy);
                    policyBuilder.SetIsOriginAllowed(origin => allowedOrigins.Contains(origin));

                    _corsOptions.AddPolicy(PolicyFrontend, policyBuilder.Build());
                }
            });
        }
    }
}
