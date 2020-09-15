using Microsoft.Azure.ServiceBus.Core;
using SlimClient.ServiceBus.Azure.Entities;

namespace SlimClient.ServiceBus.Azure.Interfaces
{
    /// <summary>
    /// Defines needed operations to perform azure service bus client creation.
    /// </summary>
    /// <remarks>
    /// Currently the service bus clients are divided into receiving and sending clients to reduce the number of create methods.
    /// </remarks>
    public interface IMessagingClientFactory
    {
        /// <summary>
        /// Creates instance of message receiving clients (Queue or Subscription). The instances are not cached as they are intented to be used as part of message handling subscriptions. 
        /// </summary> 
        /// <param name="type">ReceiverClientType</param>
        /// <param name="messagingEntityPath">Messaging Entity Path (Topic/Queue name)</param>
        /// <param name="subscriptionName">For Subscriptions - subscription name</param>
        /// <returns>IReceiverClient</returns>
        IReceiverClient CreateReceiverClient(ReceiverClientType type, string messagingEntityPath, string subscriptionName = null);

        /// <summary>
        /// Creates instance of message message sending clients (Topic or Subscription). The instances are cached and reused.
        /// </summary>
        /// <param name="clientTypeType">Sender Client Type</param>
        /// <param name="messagingEntityPath">Messaging Entity Path (Topic/Queue name)</param>
        /// <returns>ISenderClient</returns>
        ISenderClient CreateSenderClient(SenderClientType clientTypeType, string messagingEntityPath);
    }
}
