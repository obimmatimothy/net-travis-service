using System;

namespace TravisService.Models
{
    public sealed class HandheldEvent : EventBase
    {
        public HandheldEvent(string handheldId, double longitude, double latitude, DateTime timestamp) :
            base(longitude, latitude, timestamp)
        {
            HandheldId = handheldId;
        }

        public string HandheldId { get; }
    }
}
