using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using SandyBridge.Infrastructure;
using TravisService.Adapters.DynamoDB.Extensions;
using TravisService.Adapters.DynamoDB.Infrastructure;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Ports;

namespace TravisService.Adapters.DynamoDB.Ports
{
    internal sealed class DynamoDBLookupStore : ILookupStore
    {
        private readonly IAmazonDynamoDB _dynamoDBClient;
        private readonly DynamoDBSettings _settings;
        private readonly MonitoringEvents _monitoringEvents;

        public DynamoDBLookupStore(IAmazonDynamoDB dynamoDBClient, DynamoDBSettings settings, MonitoringEvents monitoringEvents)
        {
            _dynamoDBClient = dynamoDBClient ?? throw new ArgumentNullException(nameof(dynamoDBClient));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _monitoringEvents = monitoringEvents ?? throw new ArgumentNullException(nameof(monitoringEvents));
        }

        public async Task<string> GetHandheldIdAsync(string vehicleId)
        {
            var request = new GetItemRequest
            {
                TableName = _settings.LookupTableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    ["VehicleId"] = new AttributeValue { S = vehicleId },
                },
            };

            try
            {
                var response = await Run.TimedAsync(
                    () => _dynamoDBClient.GetItemAsync(request),
                    (success, duration) => _monitoringEvents.GetHandheldId(success, duration));

                return response?.Item?.GetHandheldId();
            }
            catch (Exception exception)
            {
                _monitoringEvents.FailedToGetHandheldId(vehicleId, exception);

                throw new UnableToRetrieveHandheldException(exception);
            }
        }
    }
}
