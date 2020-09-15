namespace SlimClient.ServiceBus.Azure.Interfaces
{
    /// <summary>
    /// Defines delivery mode. The message handler can request that the message content is delivered as content only or content and metadata.
    /// </summary>
    public enum DeliveryMode
    {
        /// <summary>
        /// The content only
        /// </summary>
        Content, 

        /// <summary>
        /// Envelope plus content
        /// </summary>
        Message
    }
}