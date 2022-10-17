using TravisService.Models;

namespace TravisService.Adapters.Sns.Models
{
    internal sealed class AlertEventDto
    {
        public string AlertType { get; set; }

        public string HandheldId { get; set; }

        public string VehicleId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public static AlertEventDto FromDomain(AlertEvent alertEvent) =>
            new()
            {
                AlertType = alertEvent.AlertType,
                HandheldId = alertEvent.HandheldId,
                VehicleId = alertEvent.VehicleId,
                Longitude = alertEvent.Longitude,
                Latitude = alertEvent.Latitude,
            };
    }
}
