using System;
using System.Runtime.Serialization;

namespace TravisService.Exceptions
{
    [Serializable]
    public class FailedToProcessVehicheEventException : Exception
    {
        public FailedToProcessVehicheEventException(Exception innerException) : base("An error occured while processing vehicle event", innerException)
        {
        }

        protected FailedToProcessVehicheEventException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
