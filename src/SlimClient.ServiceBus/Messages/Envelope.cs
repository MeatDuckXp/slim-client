using System;
using Newtonsoft.Json;

namespace SlimClient.ServiceBus.Azure.Messages
{
    /// <summary>
    ///     Communication channel message envelope wrapper.
    /// </summary>
    /// <remarks>
    ///     This is also known as message envelope wrapper pattern. The ida behind this is to abstract our payload from the
    ///     actual infrastructure message and to provide two distinct parts: a) The <see cref="Header"/> and b) The <see cref="Body"/>.
    /// 
    ///     The Header contains fields that are used by the messaging infrastructure or services in order to make decisions
    ///     about the message without to evaluating the message content. With this in place, the services know about the
    ///     header, but don't necessary know about the payload and its structure.
    ///
    ///     The Body contains the message payload.
    /// </remarks>
    [JsonObject]
    public class Envelope
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Envelope()
        {
            Header = new Header();
            Body = new Body();
        }

        /// <summary>
        ///     Gets or Sets envelope header
        /// </summary>
        [JsonProperty("header")]
        public Header Header { get; set; }

        /// <summary>
        ///     Gets or Sets envelope body
        /// </summary>
        [JsonProperty("body")]
        public Body Body { get; set; }

        /// <summary>
        ///     Gets message mime format.
        /// </summary>
        [JsonIgnore]
        public string Format => Header.Format;

        /// <summary>
        ///     Gets message content.
        /// </summary>
        [JsonIgnore]
        public object Content => Body?.Content;

        /// <summary>
        ///     Gets full content type.
        /// </summary>
        /// <remarks>
        ///     The content type in this case is considered the object's type AssemblyQualifiedName
        /// </remarks>
        [JsonIgnore]
        public string ContentType => Body?.ContentType;

        /// <summary>
        ///     Gets short content type value.
        /// </summary>
        /// <remarks>
        ///     The content type short type in this case is considered the object's type Name
        /// </remarks>
        [JsonIgnore]
        public string ContentTypeShort => Body?.ContentTypeShort;

        /// <summary>
        ///     Gets retry information.
        /// </summary>
        [JsonIgnore]
        public object RetryInformation => Body?.RetryInformation;

        #region Overrided Methods

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ObjectStateTransformation.Serialize(this);
        }

        #endregion

        #region Setter Methods
               
        /// <summary>
        ///     Sets message payload.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="value">Payload.</param>
        public void SetSerializedContent<T>(string value)
        {
	        Body.SetSerializedContent<T>(value);
	        SetContentType(typeof(T));
        }

        /// <summary>
        ///     Sets message payload.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="value">Payload.</param>
        public void SetContent<T>(T value)
        {
            Body.SetContent(value);
            SetContentType(typeof(T));
        }

        /// <summary>
        ///     Sets message payload type.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        public void SetContentType<T>()
        {
	        SetContentType(typeof(T));
        }

        /// <summary>
        ///     Sets message payload type.
        /// </summary>
        /// <param name="contentType">Message content tpe.</param>
        public void SetContentType(Type contentType)
        {
	        Body.SetContentType(contentType);
        }

        /// <summary>
        ///     Sets retry information in the envelope body.
        /// </summary>
        /// <param name="retryInformation">Retry information.</param>
        public void SetRetryInformation(object retryInformation)
        {
            Body.RetryInformation = retryInformation;
        }
                
        /// <summary>
        ///     Sets format of the content used when serialized.
        /// </summary>
        /// <param name="value">Mime format value. <see cref="MimeContentTypes"/></param>
        public void SetFormat(string value)
        {
	        Header.Format = value;
        }

        /// <summary>
        ///     Sets error information to the body.
        /// </summary>
        /// <param name="errorInformation">Error details.</param>
        public void SetErrorInformation(ErrorInformation errorInformation)
        {
	        Body.ErrorInformation = errorInformation;
        }

        /// <summary>
        ///     Sets error information to the body.
        /// </summary>
        /// <param name="source">Error source.</param>
        /// <param name="reason">Error reason.</param>
        public void SetErrorInformation(string source, string reason)
        {
	        var errorInformation = new ErrorInformation
	        {
		        DateTime = DateTime.UtcNow,
		        Source = source,
		        Reason = reason
            };

	        SetErrorInformation(errorInformation);
        }

        #endregion
    }
}