using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class NoHandheldEventsFoundException : Exception
    {
        public NoHandheldEventsFoundException(Exception innerException) : base("No events found for handheld device", innerException)
        {
        }

        protected NoHandheldEventsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
