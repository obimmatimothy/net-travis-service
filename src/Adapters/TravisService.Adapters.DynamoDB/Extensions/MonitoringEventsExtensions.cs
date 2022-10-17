using System;
using TravisService.Infrastructure;

namespace TravisService.Adapters.DynamoDB.Extensions
{
    internal static class MonitoringEventsExtensions
    {
        public static void GetHandheldId(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration) =>
            monitoringEvents.UpdateMetricAndCounter("getHandheldEvents", success, duration);

        public static void FailedToGetHandheldId(this MonitoringEvents monitoringEvents, string vehicleId, Exception exception) =>
            monitoringEvents.Logger.Error(exception, $"An exception was thrown while getting handheldId for vehicleId: {vehicleId}");

        private static void UpdateMetricAndCounter(this MonitoringEvents monitoringEvents, string metricName, bool success, TimeSpan duration)
        {
            var tags = new[]
            {
                "success:" + success.ToString().ToLower(),
            };

            monitoringEvents.Metrics.IncrementCounter($"dynamoDB.{metricName}", tags);
            monitoringEvents.Metrics.Timer($"dynamoDB.{metricName}.duration", duration, tags);
        }
    }
}
