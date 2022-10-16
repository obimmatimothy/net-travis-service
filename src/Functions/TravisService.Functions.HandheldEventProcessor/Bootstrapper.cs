using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using TravisService.Adapters.MySql;
using TravisService.Adapters.MySql.Configurations;
using TravisService.Infrastructure;

namespace TravisService.Functions.HandheldEventProcessor
{
    public static class Bootstrapper
    {
        private const string ApplicationName = "travis-handheldEvent-processor";

        public static IServiceProvider CreateDefaultApplication()
        {
            var configurationRoot = GetConfiguration();

            var services = new ServiceCollection();

            var monitoringEvents = ConfigureMonitoringAndLogging();
            services.AddSingleton(monitoringEvents);

            var mySqlSettings = configurationRoot.GetSection("MySql").Get<MySqlSettings>();
            services.AddMySqlAdapter(mySqlSettings, monitoringEvents);

            services.AddDomainLogic(monitoringEvents);

            return services.BuildServiceProvider();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("Environment");

            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());

            return configurationBuilder
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static MonitoringEvents ConfigureMonitoringAndLogging()
        {
            ILogger logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(new JsonFormatter())
                .CreateLogger();

            return new MonitoringEvents(logger, new Metrics(ApplicationName));
        }
    }
}
