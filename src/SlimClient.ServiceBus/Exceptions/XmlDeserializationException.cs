using System;
using System.Runtime.Serialization;

namespace SlimClient.ServiceBus.Azure.Exceptions
{
    [Serializable]
    internal class XmlDeserializationException : Exception
    {
        public XmlDeserializationException()
        {
        }

        public XmlDeserializationException(string message) : base(message)
        {
        }

        public XmlDeserializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XmlDeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}