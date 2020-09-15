using System;
using System.Collections.Generic;

using SlimClient.ServiceBus.Azure.Interfaces;

namespace SlimClient.ServiceBus.Azure
{
    /// <summary>
    /// Handles the creation and caching of registered message handler.
    /// </summary>
    public class MessageHandlerRegistry : IMessageHandlerRegistry
    {
        #region Fields
        private readonly IMessagingClientFactory _messagingClientFactory;
        private readonly IList<ReceiverClientWrapper> _serviceBusClients;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messagingClientFactory">Queue Client Manager</param>
        public MessageHandlerRegistry(IMessagingClientFactory messagingClientFactory)
        { 
            _messagingClientFactory = messagingClientFactory ?? throw new ArgumentNullException(nameof(messagingClientFactory));
            _serviceBusClients = new List<ReceiverClientWrapper>();
        }

        /// <summary>
        /// Registers the provided instance od IMessageHandler to subscribe to a queue or topic subscription. The Instance and the client are cached during the
        /// entire lifecycle.
        /// </summary>
        /// <param name="messageHandler">Message handler implementation</param>
        public void RegisterMessageHandler(IMessageHandler messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            } 

            var queueClient = _messagingClientFactory.CreateReceiverClient(messageHandler.ReceiverClientType, messageHandler.EntityPath, messageHandler.SubscriptionName);
            _serviceBusClients.Add(ReceiverClientWrapper.Create(queueClient, messageHandler));
        }

        /// <summary>
        /// Returns number of registered message handlers. 
        /// </summary>
        public int MessageHandlerCount => _serviceBusClients.Count;
    }
}
