using System;
using TravisService.Infrastructure;
using TravisService.Models;

namespace TravisService.Adapters.Sns.Extensions
{
    internal static class MonitoringEventsExtensions
    {
        public static void PublishAlertEvent(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration) =>
            monitoringEvents.UpdateMetricAndCounter("getHandheldEvents", success, duration);

        public static void FailedToPublishAlertEvent(this MonitoringEvents monitoringEvents, AlertEvent alert, Exception exception) =>
            monitoringEvents.Logger.Error(exception, $"An exception was thrown while publishing an alert events from the database. Alert: {@alert}");

        private static void UpdateMetricAndCounter(this MonitoringEvents monitoringEvents, string metricName, bool success, TimeSpan duration)
        {
            var tags = new[]
            {
                "success:" + success.ToString().ToLower(),
            };

            monitoringEvents.Metrics.IncrementCounter($"sns.{metricName}", tags);
            monitoringEvents.Metrics.Timer($"sns.{metricName}.duration", duration, tags);
        }
    }
}
