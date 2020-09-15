using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

using SlimClient.ServiceBus.Azure.Exceptions;

namespace SlimClient.ServiceBus.Azure.Messages
{
    public class ObjectStateTransformation
    {
        public static string Type => MimeContentTypes.Json;

        public static T Deserialize<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public static object DeserializeJson(Type type, string message)
        {
	        return JsonConvert.DeserializeObject(message, type);
        }

        public static object DeserializeXml(Type type, string message)
        {
            var xmlContent = XElement.Parse(message);
            object serializedObject;

            try
            {
                using (var stringReader = xmlContent.CreateReader())
                {
                    var serializer = new XmlSerializer(type);
                    serializedObject = serializer.Deserialize(stringReader);
                }
            }
            catch (InvalidOperationException e)
            {
                var exceptionMessage = string.Join(" ",e.Message, e.InnerException?.Message);
                throw new XmlDeserializationException(exceptionMessage);
            }

	        return serializedObject;
        }

        public static string Serialize<T>(T messageInstance)
        {
            return JsonConvert.SerializeObject(messageInstance);
        }

        public static bool CanDeserialize(string message)
        {
            return !string.IsNullOrWhiteSpace(message);
        }

        public static bool CanSerialize<T>(T messageInstance)
        {
            return messageInstance != null;
        }
    }
}
