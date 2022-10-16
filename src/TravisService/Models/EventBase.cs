using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravisService.Models
{
    public abstract class EventBase
    {
        public double Longitude { get; internal set; }

        public double Latitude { get; internal set; }

        public DateTime Timestamp { get; internal set; }
    }
}
