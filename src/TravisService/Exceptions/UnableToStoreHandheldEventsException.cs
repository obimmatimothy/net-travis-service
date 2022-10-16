using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class UnableToStoreHandheldEventsException : Exception
    {
        public UnableToStoreHandheldEventsException(Exception innerException) : base("Unable to store handheld events", innerException)
        {
        }

        protected UnableToStoreHandheldEventsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
