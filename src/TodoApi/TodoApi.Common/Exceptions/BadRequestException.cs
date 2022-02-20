using System.Runtime.Serialization;

namespace TodoApi.Common.Exceptions
{
    [Serializable]
    public class BadRequestException : ApplicationException
    {
        public BadRequestException()
        {
        }

        public BadRequestException(string message) : base(message)
        {

        }

        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
