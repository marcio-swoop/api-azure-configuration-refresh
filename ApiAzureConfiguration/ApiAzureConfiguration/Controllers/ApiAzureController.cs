using Microsoft.AspNetCore.Mvc;

namespace ApiAzureConfiguration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiAzureController : ControllerBase
    {
        private readonly ILogger<ApiAzureController> _logger;

        private static Guid _referenceId = Guid.NewGuid();

        public ApiAzureController(ILogger<ApiAzureController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetAzureConfiguration")]
        public object Get([FromServices] IConfiguration configuration, [FromQuery] string? filter)
        {
            _logger.LogInformation("GetAzureConfiguration called. ReferenceId: {ReferenceId}", _referenceId);

            var result = string.IsNullOrEmpty(filter) ? 
                configuration.AsEnumerable()
                .Select(x => new
                {
                    x.Key,
                    x.Value
                }) 
                :
                configuration.AsEnumerable()
                .Where(x => x.Key.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => new
                {
                    x.Key,
                    x.Value
                });

            return new
            {
                CurrentTime = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                ReferenceId = _referenceId.ToString(),
                CoreApi = configuration["CoreApi:CorsSettings:AllowedOrigins"],
                Configurations = result
            };
        }
    }
}