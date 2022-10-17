namespace TravisService.Models
{
    public class AlertEvent
    {
        public AlertEvent(string handheldId, string vehicleId, double longitude, double latitude)
        {
            AlertType = "50mApartDelivery";
            HandheldId = handheldId;
            VehicleId = vehicleId;
            Longitude = longitude;
            Latitude = latitude;
        }

        public string AlertType { get; }

        public string HandheldId { get; }

        public string VehicleId { get; }

        public double Longitude { get; }

        public double Latitude { get; }
    }
}
