using System;
using Newtonsoft.Json;

namespace SlimClient.ServiceBus.Azure.Messages
{
    /// <summary>
    ///     The envelope header contains meta data for the message that is part of the message body <see cref="Body" />.
    /// </summary>
    [JsonObject]
    public class Header
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public Header()
        {
            Initialize(DateTime.Now);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="createdDateTime">Created date and time value.</param>
        public Header(DateTime createdDateTime)
        {
            Initialize(createdDateTime);
        }

        /// <summary>
        ///     Gets or Sets date the message envelope was created
        /// </summary>
        [JsonProperty]
        public virtual string Created { get; set; }

        /// <summary>
        ///     Gets or Sets the content format. This should be mime content standard
        /// </summary>
        [JsonProperty]
        public virtual string Format { get; set; }

        /// <summary>
        ///     Performs message header initialization.
        /// </summary>
        /// <param name="createdDateTime">Created date and time value.</param>
        private void Initialize(DateTime createdDateTime)
        {
            Created = createdDateTime.ToString(EnvelopeDefaults.DefaultDateTimeFormat);
        }
    }
}
