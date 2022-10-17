using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class UnableToRetrieveHandheldException : Exception
    {
        public UnableToRetrieveHandheldException(Exception innerException) : base("Unable to retrieve handheldID from the store", innerException)
        {
        }

        protected UnableToRetrieveHandheldException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
