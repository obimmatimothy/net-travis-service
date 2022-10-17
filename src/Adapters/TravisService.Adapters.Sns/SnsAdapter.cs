using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TravisService.Adapters.Sns.Infrastructure;
using TravisService.Adapters.Sns.Ports;
using TravisService.Infrastructure;
using TravisService.Ports;

namespace TravisService.Adapters.Sns
{
    public static class SnsAdapter
    {
        public static void AddSnsAdapter(this IServiceCollection services, SnsSettings settings, MonitoringEvents monitoringEvents)
        {
            services.TryAddSingleton(settings);
            services.TryAddSingleton(monitoringEvents);
            services.AddAWSService<IAmazonSimpleNotificationService>();
            services.AddScoped<INotificationService, SnsNotificationService>();
        }
    }
}
