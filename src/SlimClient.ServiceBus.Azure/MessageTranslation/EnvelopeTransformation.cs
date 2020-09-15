using SlimClient.ServiceBus.Azure.Messages;
using System;

namespace SlimClient.ServiceBus.Azure.MessageTranslation
{
    /// <summary>
    /// This class defined set of static operations related to the envelope transformation process.
    /// </summary>
    public static class EnvelopeTransformation
    {
        /// <summary>
        ///     Deserializes envelope.
        /// </summary>
        /// <param name="envelope">Serialized envelope.</param>
        /// <returns>Envelope.</returns>
        public static Envelope Deserialize(string envelope)
        {
            var messageObject = ObjectStateTransformation.Deserialize<Envelope>(envelope);
            return messageObject;
        }

        /// <summary>
        ///     Recreates body content object state to the initial value.
        /// </summary>
        /// <param name="envelope">Envelope.</param>
        public static void MaterializeContent(Envelope envelope)
        {
            var objectType = Type.GetType(envelope.ContentType);
            var objectStateBase64Encoded = envelope.Content.ToString();

            //TODO(Vedran): Refactor this to become implicit statement when getting the content.
            var base64EncodedBytes = System.Convert.FromBase64String(objectStateBase64Encoded);
            var objectState = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

            switch (envelope.Format)
            {
                case MimeContentTypes.Json:
                    envelope.Body.Content = ObjectStateTransformation.DeserializeJson(objectType, objectState);
                    break;
                case MimeContentTypes.Xml:
                    envelope.Body.Content = ObjectStateTransformation.DeserializeXml(objectType, objectState);
                    break;
                default:
                    throw new ArgumentException($"Invalid Envelope format MIME type {nameof(envelope.Format)}");
            }
        }

        /// <summary>
        /// Serializes the Envelope instance.
        /// </summary>
        /// <param name="envelope">Envelope.</param>
        /// <returns>Serialized envelope.</returns>
        public static string Serialize(Envelope envelope)
        {
            var serializedEnvelope = ObjectStateTransformation.Serialize(envelope);
            return serializedEnvelope;
        }
    }
}
