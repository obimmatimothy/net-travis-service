using TravisService.Infrastructure;

namespace TravisService.Functions.HandheldEventProcessor.Extensions
{
    internal static class MonitoringEventsExtensions
    {
        public static void LambdaExecution(this MonitoringEvents monitoringEvents, bool success, TimeSpan duration)
        {
            var tags = new[]
            {
                "success:" + success.ToString().ToLower(),
            };

            monitoringEvents.Metrics.IncrementCounter("processHandheldEvents", tags);
            monitoringEvents.Metrics.Timer("processHandheldEvents.duration", duration, tags);
        }

        public static void LambdaExecutionFailed(this MonitoringEvents monitoringEvents, Exception exception) =>
            monitoringEvents.Logger.Error(exception, "Lambda failed.");
    }
}
