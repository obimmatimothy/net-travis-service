using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TravisService.Adapters.MySql.Configurations;
using TravisService.Adapters.MySql.Infrastructure;
using TravisService.Adapters.MySql.Ports;
using TravisService.Infrastructure;
using TravisService.Ports;

namespace TravisService.Adapters.MySql
{
    public static class MySqlAdapter
    {
        public static void AddMySqlAdapter(this IServiceCollection serviceCollection, MySqlSettings mySqlSettings, MonitoringEvents monitoringEvents)
        {
            serviceCollection.TryAddSingleton(monitoringEvents);
            serviceCollection.TryAddSingleton(mySqlSettings);
            serviceCollection.AddScoped<IDbConnectionFactory, MySqlConnectionFactory>();
            serviceCollection.AddScoped<IHandheldEventStore, MySqlHandheldEventStore>();
        }
    }
}
