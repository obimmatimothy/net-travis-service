using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class FailedToPublishAlertException : Exception
    {
        public FailedToPublishAlertException(Exception innerException) : base("An error occured while publishing alert event", innerException)
        {
        }

        protected FailedToPublishAlertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
