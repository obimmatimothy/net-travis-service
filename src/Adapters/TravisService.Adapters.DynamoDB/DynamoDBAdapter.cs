using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TravisService.Adapters.DynamoDB.Infrastructure;
using TravisService.Adapters.DynamoDB.Ports;
using TravisService.Infrastructure;
using TravisService.Ports;

namespace TravisService.Adapters.DynamoDB
{
    public static class DynamoDBAdapter
    {
        public static void AddDynamoDBAdapter(this IServiceCollection serviceCollection, DynamoDBSettings settings, MonitoringEvents monitoringEvents)
        {
            var awsOptions = new AWSOptions
            {
                Profile = settings.Profile,
                Region = Amazon.RegionEndpoint.GetBySystemName(settings.AwsRegion),
            };

            serviceCollection.TryAddSingleton(monitoringEvents);
            serviceCollection.TryAddSingleton(settings);
            serviceCollection.AddDefaultAWSOptions(awsOptions);
            serviceCollection.AddAWSService<IAmazonDynamoDB>();
            serviceCollection.AddScoped<ILookupStore, DynamoDBLookupStore>();
        }
    }
}
