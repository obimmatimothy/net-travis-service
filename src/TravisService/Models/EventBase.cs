using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravisService.Models
{
    public abstract class EventBase
    {
        protected EventBase(double longitude, double latitude, DateTime timestamp)
        {
            Longitude = longitude;
            Latitude = latitude;
            Timestamp = timestamp;
        }

        public double Longitude { get; }

        public double Latitude { get; }

        public DateTime Timestamp { get; }
    }
}
