using System;
using TravisService.Models;

namespace TravisService.Functions.HandheldEventProcessor.Models
{
    internal sealed class HandheldEventDto
    {
        public string HandheldId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime Timestamp { get; set; }

        public HandheldEvent ToDomain() => new(HandheldId, Longitude, Latitude, Timestamp);
    }
}
