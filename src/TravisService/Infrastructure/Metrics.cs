using System;
using System.Linq;

namespace TravisService.Infrastructure
{
    public class Metrics
    {
        private readonly Action<string> _writer;
        private readonly Func<long> _unixTimeInSecondsProvider;
        private readonly string _metricPrefix;

        /// <summary>
        /// This constructor initializes a metric that sends all output to STDOUT
        /// </summary>
        public Metrics(string metricPrefix = "")
            : this(Console.WriteLine, () => DateTimeOffset.UtcNow.ToUnixTimeSeconds(), metricPrefix)
        {
        }

        private Metrics(Action<string> writer, Func<long> unixTimeInSecondsProvider, string metricPrefix = "")
        {
            _writer = writer;
            _unixTimeInSecondsProvider = unixTimeInSecondsProvider;
            _metricPrefix = string.IsNullOrEmpty(metricPrefix) ? "" : metricPrefix.TrimEnd('.') + '.';
        }

        /// <summary>
        /// Adds 1 to the metric name specified
        /// </summary>
        public void IncrementCounter(string name, params string[] tags)
        {
            var message = ComposeMetricMessage(value: 1, type: "count", name: name, tags: tags);
            _writer.Invoke(message);
        }

        /// <summary>
        /// Saves a timer for the metric name specified
        /// </summary>
        public void Timer(string name, TimeSpan elapsedTime, params string[] tags)
        {
            var message = ComposeMetricMessage(value: elapsedTime.TotalMilliseconds, type: "timer", name: name, tags: tags);
            _writer.Invoke(message);
        }

        private string ComposeMetricMessage<T>(T value, string type, string name, params string[] tags)
        {
            name = _metricPrefix + name;
            var message = $"MONITORING|{_unixTimeInSecondsProvider()}|{value}|{type}|{name}";

            if (tags.Any())
            {
                message += "|#" + string.Join(",", tags);
            }

            return message;
        }
    }
}
