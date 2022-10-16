using System;
using Serilog;

namespace TravisService.Infrastructure
{
    public class MonitoringEvents
    {
        public MonitoringEvents(ILogger logger, Metrics metrics)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        }

        /// <summary>
        /// The configured logger instance.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// The configured metrics implementation.
        /// </summary>
        public Metrics Metrics { get; }
    }
}
