using System;
using TravisService.Infrastructure;

namespace TravisService.Functions.VehicleEventProcessor.Extensions
{
    internal static class MonitoringEventsExtensions
    {
        public static void LambdaExecution(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration)
        {
            var tags = new[]
            {
                "success:" + success.ToString().ToLower(),
            };

            monitoringEvents.Metrics.IncrementCounter("processVehicleEvent", tags);
            monitoringEvents.Metrics.Timer("processVehicleEvent.duration", duration, tags);
        }

        public static void LambdaExecutionFailed(this MonitoringEvents monitoringEvents, Exception exception) =>
            monitoringEvents.Logger.Error(exception, "Lambda failed.");

        public static void RequeingEvent(this MonitoringEvents monitoringEvents, string @event) =>
            monitoringEvents.Logger.Information($"Requeing event {@event} to be retried later");
    }
}
