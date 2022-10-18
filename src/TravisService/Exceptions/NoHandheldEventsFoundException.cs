using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class NoHandheldEventsFoundException : Exception
    {
        public NoHandheldEventsFoundException() : base("None or limited events found for handheld device")
        {
        }

        protected NoHandheldEventsFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
