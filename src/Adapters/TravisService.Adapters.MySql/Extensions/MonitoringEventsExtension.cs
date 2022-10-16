using System;
using TravisService.Infrastructure;

namespace TravisService.Adapters.MySql.Extensions
{
    internal static class MonitoringEventsExtension
    {
        public static void InsertedNewHandheldEvents(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration)
        {
            var tags = new[]
            {
                "success:" + success.ToString().ToLower(),
            };

            monitoringEvents.Metrics.IncrementCounter("mysql.storeHandheldEvents", tags);
            monitoringEvents.Metrics.Timer("mysql.storeHandheldEvents.duration", duration, tags);
        }

        public static void FailedToSaveHandheldEvents(this MonitoringEvents monitoringEvents, Exception exception) =>
            monitoringEvents.Logger.Error($"An exception was thrown while saving events to the database. Exception : {exception.Message}");
    }
}
