using System;
using System.Globalization;
using Newtonsoft.Json;

namespace SlimClient.ServiceBus.Azure.Messages
{
    /// <summary>
    ///     Message envelope payload wrapper. This section of the <see cref="Envelope"/> contains information related to the actual message
    ///     payload.
    /// </summary>
    [JsonObject]
    public class Body
    {
        /// <summary>
        ///     Gets or Sets message content.
        /// </summary>
        [JsonProperty("content")]
        public virtual object Content { get; set; }

        /// <summary>
        ///     Gets or Sets content type.
        /// </summary>
        [JsonProperty("contentType")]
        public virtual string ContentType { get; set; }

        /// <summary>
        ///     Gets or Sets short content type.
        /// </summary>
        [JsonProperty("contentTypeShort")]
        public virtual string ContentTypeShort { get; set; }

        /// <summary>
        ///     Gets or Sets retry information.
        /// </summary>
        /// <remarks>
        ///     This to be used by any retry service to hold the actual retry data.
        /// </remarks>
        [JsonProperty("retryInformation")]
        public object RetryInformation { get; set; }

        /// <summary>
        ///     Gets or Sets error information.
        /// </summary>
        /// <remarks>
        ///     This to be used by any service that resolves the error messages.
        /// </remarks>
        [JsonProperty("errorInformation")]
        public ErrorInformation ErrorInformation { get; set; }

        /// <summary>
        ///     Sets already serialized message content.
        /// </summary>
        /// <typeparam name="T">Type of the message content.</typeparam>
        /// <param name="value">Serialized message content.</param>
        public void SetSerializedContent<T>(string value)
        {  
	        Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        ///     Sets message content.
        /// </summary>
        /// <typeparam name="T">Type of the message content.</typeparam>
        /// <param name="value">Message content.</param>
        public void SetContent<T>(T value)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                Culture = CultureInfo.InvariantCulture,
                NullValueHandling = NullValueHandling.Ignore
            };

            var contentString = JsonConvert.SerializeObject(value, Formatting.None, jsonSerializerSettings);
            Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(contentString));
        }

        /// <summary>
        ///     Sets message content type.
        /// </summary> 
        /// <param name="contentType">Message content type.</param>
        public void SetContentType(Type contentType)
        {
            ContentType = contentType.AssemblyQualifiedName;
            ContentTypeShort = contentType.Name;
        }
    }
}