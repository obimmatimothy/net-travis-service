namespace TravisService.Models
{
    public class LocationCoordinate
    {
        public LocationCoordinate(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; }

        public double Latitude { get; }
    }
}
