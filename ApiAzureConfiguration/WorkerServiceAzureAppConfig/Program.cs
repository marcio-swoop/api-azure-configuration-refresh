using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WorkerServiceAzureAppConfig.Extensions;

namespace WorkerServiceAzureAppConfig;

public static class Program
{
    public static void Main(string[] args)
    {
        AzureCreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder AzureCreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAzureAppConfigurationWorker()

                .ConfigureWebHostDefaults(webBuilder => 
                {
                    webBuilder.ConfigureServices(ConfigureCustomServices);

                    webBuilder.UseStartup<Startup>();
                });

    public static void ConfigureCustomServices(IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.Configure<WorkerSettings>(configuration.GetSection("worker"));

        services.AddLogging();
        services.AddHostedService<Worker>();
    }
}


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public static void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {

        });
    }
}
