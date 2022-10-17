using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class UnableToRetrieveHandheldEventsException : Exception
    {
        public UnableToRetrieveHandheldEventsException(Exception innerException) : base("Unable to retrieve handheld events", innerException)
        {
        }

        protected UnableToRetrieveHandheldEventsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
