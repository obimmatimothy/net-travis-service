using System;
using TravisService.Models;

namespace TravisService.Adapters.MySql.Models
{
    internal sealed class HandheldEventStorageModel
    {
        public string HandheldId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime Timestamp { get; set; }

        public static HandheldEventStorageModel FromDomain(HandheldEvent handheldEvent) =>
            new()
            {
                HandheldId = handheldEvent.HandheldId,
                Latitude = handheldEvent.Latitude,
                Longitude = handheldEvent.Longitude,
                Timestamp = handheldEvent.Timestamp,
            };

        public HandheldEvent ToDomain() =>
            new(HandheldId, Longitude, Latitude, Timestamp);
    }
}
