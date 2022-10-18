using System;
using TravisService.Infrastructure;

namespace TravisService.Extensions
{
    internal static class MonitoringEventsExtenions
    {
        public static void NoCorrespondingHandheld(this MonitoringEvents monitoringEvents, string vehicleId)
        {
            monitoringEvents.Logger.Information($"Skipping comparison for vehicle {vehicleId} as no corresponding handheldId can be found");

            var tags = new[]
            {
                "vehicleId:" + vehicleId.ToLower(),
            };
            monitoringEvents.Metrics.IncrementCounter("noHandheldFound", tags);
        }

        public static void NotEnoughHandheldEvents(this MonitoringEvents monitoringEvents, string vehicleId, DateTime timestamp)
        {
            monitoringEvents.Logger.Warning($"Expected more handheld events than found for vehicle {vehicleId}");

            var tags = new[]
            {
                "vehicleId:" + vehicleId.ToLower(),
                "timestamp:" + timestamp.ToString().ToLower(),
            };
            monitoringEvents.Metrics.IncrementCounter("expectedMoreHandheldEvents", tags);
        }

        public static void ExceptionOccured(this MonitoringEvents monitoringEvents, Exception exception, string vehicleId) =>
            monitoringEvents.Logger.Error(exception, $"An error occured while processing vehicle event for {vehicleId}");
    }
}
