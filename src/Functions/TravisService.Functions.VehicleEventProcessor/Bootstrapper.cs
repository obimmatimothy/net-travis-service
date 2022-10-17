using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using TravisService.Adapters.DynamoDB;
using TravisService.Adapters.DynamoDB.Infrastructure;
using TravisService.Adapters.MySql;
using TravisService.Adapters.MySql.Configurations;
using TravisService.Adapters.Sns;
using TravisService.Adapters.Sns.Infrastructure;
using TravisService.Calculators;
using TravisService.Infrastructure;
using TravisService.UseCases;

namespace TravisService.Functions.VehicleEventProcessor
{
    public static class Bootstrapper
    {
        private const string ApplicationName = "travis-vehicleEvent-processor";

        public static IServiceProvider CreateDefaultApplication()
        {
            var configurationRoot = GetConfiguration();

            var services = new ServiceCollection();

            var monitoringEvents = ConfigureMonitoringAndLogging();
            services.AddSingleton(monitoringEvents);

            var mySqlSettings = configurationRoot.GetSection("MySql").Get<MySqlSettings>();
            services.AddMySqlAdapter(mySqlSettings, monitoringEvents);

            var dynamoDBSettings = configurationRoot.GetSection("DynamoDb").Get<DynamoDBSettings>();
            services.AddDynamoDBAdapter(dynamoDBSettings, monitoringEvents);

            var snsSettings = configurationRoot.GetSection("Sns").Get<SnsSettings>();
            services.AddSnsAdapter(snsSettings, monitoringEvents);

            services.AddDomainLogic();

            return services.BuildServiceProvider();
        }

        private static void AddDomainLogic(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IProcessVehicleEventUseCase, ProcessVehicleEventUseCase>();
            serviceCollection.AddScoped<IDistanceCalculator, DistanceCalculator>();
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
