using System;

namespace TravisService.Models
{
    public sealed class VehicleEvent : EventBase
    {
        public VehicleEvent(string vehicleId, double longitude, double latitude, DateTime timestamp) :
            base(longitude, latitude, timestamp)
        {
            VehicleId = vehicleId;
        }

        public string VehicleId { get; }
    }
}
