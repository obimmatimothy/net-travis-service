using System;
using TravisService.Models;

namespace TravisService.Functions.VehicleEventProcessor.Models
{
    internal sealed class VehicleEventDto
    {
        public string VehicleId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime Timestamp { get; set; }

        public VehicleEvent ToDomain() => new(VehicleId, Longitude, Latitude, Timestamp);
    }
}
