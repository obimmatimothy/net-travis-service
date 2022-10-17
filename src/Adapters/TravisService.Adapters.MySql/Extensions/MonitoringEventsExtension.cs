using System;
using TravisService.Infrastructure;

namespace TravisService.Adapters.MySql.Extensions
{
    internal static class MonitoringEventsExtension
    {
        public static void InsertedNewHandheldEvents(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration) =>
            monitoringEvents.UpdateMetricAndCounter("storeHandheldEvents", success, duration);

        public static void FailedToSaveHandheldEvents(this MonitoringEvents monitoringEvents, Exception exception) =>
            monitoringEvents.Logger.Error($"An exception was thrown while saving events to the database. Exception : {exception.Message}");

        public static void FetchHandheldEvents(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration) =>
            monitoringEvents.UpdateMetricAndCounter("getHandheldEvents", success, duration);

        public static void FailedToGetHandheldEvents(this MonitoringEvents monitoringEvents, Exception exception) =>
            monitoringEvents.Logger.Error($"An exception was thrown while retrieving events from the database. Exception : {exception.Message}");

        private static void UpdateMetricAndCounter(this MonitoringEvents monitoringEvents, string metricName, bool success, TimeSpan duration)
        {
            var tags = new[]
            {
                "success:" + success.ToString().ToLower(),
            };

            monitoringEvents.Metrics.IncrementCounter($"mysql.{metricName}", tags);
            monitoringEvents.Metrics.Timer($"mysql.{metricName}.duration", duration, tags);
        }
    }
}
