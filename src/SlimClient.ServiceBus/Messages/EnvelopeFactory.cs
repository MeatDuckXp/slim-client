namespace SlimClient.ServiceBus.Azure.Messages
{
    /// <summary>
    ///     Envelope factory methods.
    /// </summary>
    public class EnvelopeFactory
    {
        /// <summary>
        ///     Creates new instance of envelope.
        /// </summary>
        /// <typeparam name="T">Type of content.</typeparam>
        /// <param name="content">Content value.</param>
        /// <returns>Envelope</returns>
        public static Envelope Create<T>(T content)
        {
            var envelope = new Envelope();

            envelope.SetContent(content);

            return envelope;
        }

        /// <summary>
        ///     Creates new instance of envelope.
        /// </summary>
        /// <typeparam name="T">Type of content.</typeparam>
        /// <param name="content">Content value.</param>
        /// <param name="serviceName">Service name.</param>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Envelope.</returns>
        public static Envelope Create<T>(T content, string serviceName, string serviceId)
        {
            var envelope = new Envelope();

            envelope.SetContent(content);

            return envelope;
        }


        /// <summary>
        ///     Creates new instance of envelope based ont he JSON content provided.
        /// </summary>
        /// <typeparam name="T">Type of content.</typeparam>
        /// <param name="content">Content value.</param>
        /// <param name="serviceName">Service name.</param>
        /// <param name="serviceId">Service id.</param>
        /// <param name="format">Mime type format used when serialized.</param>
        /// <returns>Envelope.</returns>
        public static Envelope CreateFromSerializedContent<T>(string content, string serviceName, string serviceId, string format = MimeContentTypes.Json)
        {
	        var envelope = new Envelope();

	        envelope.SetSerializedContent<T>(content);
	        envelope.SetFormat(format);

            return envelope;
        }
    }
}