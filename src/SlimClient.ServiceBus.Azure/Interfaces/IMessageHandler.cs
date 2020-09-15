using SlimClient.ServiceBus.Azure.Entities;
using System.Threading.Tasks;

namespace SlimClient.ServiceBus.Azure.Interfaces
{
    /// <summary>
    /// Message handler operation definitions. This 
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Gets client type that is the message handler subscribing to.
        /// </summary>
        ReceiverClientType ReceiverClientType { get; }

        /// <summary>
        /// Gets the preferred delivery mode. If the handler will access only the payload, the DeliveryMode.Content should be used. 
        /// </summary>
        DeliveryMode Mode { get; }

        /// <summary>
        /// Gets or Sets entity path (Queue/Topic name)
        /// </summary>
        string EntityPath { get; }

        /// <summary>
        /// Gets subscription name in case subscription is our target to fetch the messages from.
        /// </summary>
        string SubscriptionName { get; }

        /// <summary>
        /// This method is invoked when an message is received. It should contain logic that is needed to process the message.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="message">Message instance</param>
        /// <returns>If successfully processed True, otherwise False.</returns>
        Task<bool> ProcessMessage<T>(T message);

        /// <summary>
        /// Is invoked in case an error occurs while processing the message.
        /// </summary>
        /// <param name="errorDetail">Error details.</param>
        void ProcessError(ErrorDetail errorDetail);

        /// <summary>
        /// Is invoked when the message fails the deserialization process.
        /// </summary>
        /// <param name="deserializationErrorDetail">Error details.</param>
        /// <returns>True if the service handled the error, otherwise false.</returns>
        bool ProcessDeserializationError(DeserializationErrorDetail deserializationErrorDetail);
    }
}
