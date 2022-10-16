using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class FailedToProcessHandheldEventsException : Exception
    {
        public FailedToProcessHandheldEventsException(Exception innerException) : base("An error occured while processing handheld events", innerException)
        {
        }

        protected FailedToProcessHandheldEventsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
