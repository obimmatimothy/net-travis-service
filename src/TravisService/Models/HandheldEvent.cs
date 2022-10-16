using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravisService.Models
{
    public sealed class HandheldEvent : EventBase
    {
        public HandheldEvent(string handHeldId, double longitude, double latitude, DateTime timestamp):
            base(longitude, latitude, timestamp)
        {
            HandheldId = handHeldId;
        }

        public string HandheldId { get;}
    }
}
