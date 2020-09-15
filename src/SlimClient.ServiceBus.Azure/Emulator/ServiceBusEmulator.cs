using SlimClient.ServiceBus.Azure.Core;
using SlimClient.ServiceBus.Azure.Emulator.Core;
using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Represents abstract service bus emulator capable of receiving a message and delivering it to the registered message
    ///     handler. The message type is currently
    ///     hardcoded Envelope.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ServiceBusEmulator
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ServiceBusEmulator()
        {
            _azureMessagingResources = new Dictionary<SenderClientType, Dictionary<string, IEntityEmulator>>();
        }

        /// <summary>
        ///     Gets Service bus emulator instance.
        /// </summary>
        public static ServiceBusEmulator Instance
        {
            get
            {
                lock (LockObject)
                {
                    return _serviceBus ?? (_serviceBus = new ServiceBusEmulator());
                }
            }
        }

        /// <summary>
        ///     Registers the topic within service bus.
        /// </summary>
        /// <param name="topicInstance">Topic instance</param>
        public void RegisterTopic(TopicEmulator topicInstance)
        {
            topicInstance.ValidateIsNotNullThrowsArgumentNullException("TopicWasNull");
            if (!_azureMessagingResources.ContainsKey(SenderClientType.Topic))
            {
                _azureMessagingResources.Add(SenderClientType.Topic, new Dictionary<string, IEntityEmulator>());
            }
            _azureMessagingResources[SenderClientType.Topic].Add(topicInstance.Name.EnsureToUpper(), topicInstance);
        }

        /// <summary>
        ///     Registers the queue within service bus.
        /// </summary>
        /// <param name="queueInstance">Queue instance</param>
        public void RegisterQueue(QueueEmulator queueInstance)
        {
            queueInstance.ValidateIsNotNullThrowsArgumentNullException("QueueIsNull");
            if (!_azureMessagingResources.ContainsKey(SenderClientType.Queue))
            {
                _azureMessagingResources.Add(SenderClientType.Queue, new Dictionary<string, IEntityEmulator>());
            }
            _azureMessagingResources[SenderClientType.Queue].Add(queueInstance.Name.EnsureToUpper(), queueInstance);
        }

        /// <summary>
        ///     Sends the message envelope to the service bus resource.
        /// </summary>
        /// <param name="clientType">Sender Client Type</param>
        /// <param name="messageEnvelope">Envelope</param>
        /// <param name="entityPath">Entity Path</param>
        public void Send(SenderClientType clientType, Envelope messageEnvelope, string entityPath)
        {
            messageEnvelope.ValidateIsNotNullThrowsArgumentNullException("MessageEnvelopeIsNull");
            entityPath.ValidateIsStringNullOrEmptyThrowsArgumentNullException("EntityPathIsNullOrEmpty");
            entityPath = entityPath.ToUpperInvariant();

            lock (LockObject)
            {
                ValidateResourceExists(clientType, entityPath);
                _azureMessagingResources[clientType][entityPath].Send(messageEnvelope);
            }
        }

        /// <summary>
        ///     Registers the message handler within the service bus.
        /// </summary>
        /// <param name="clientType">Sender Client Type</param>
        /// <param name="handler">Message handler</param>
        /// <param name="entityPath">Entity Path</param>
        /// <param name="subscriptionPath">Subscription Path</param>
        public void RegisterMessageHandler(SenderClientType clientType, Action<Envelope, string> handler, string entityPath, string subscriptionPath)
        {
            handler.ValidateIsNotNullThrowsArgumentNullException("MessageHandlerIsNull");
            lock (LockObject)
            {
                ValidateResourceExists(clientType, entityPath);
                _azureMessagingResources[clientType][entityPath].RegisterMessageHandler(handler, subscriptionPath);
            }
        }

        #region Private Methods

        /// <summary>
        ///     Performs validation does the required resource exists within the service bus.
        /// </summary>
        /// <param name="clientType">Sender client type</param>
        /// <param name="entityPath">Entity path</param>
        private void ValidateResourceExists(SenderClientType clientType, string entityPath)
        {
            if (!_azureMessagingResources[clientType].ContainsKey(entityPath))
            {
                throw new InvalidOperationException($"Resource: {entityPath} is missing.");
            }
        }

        #endregion

        #region Fields

        private static ServiceBusEmulator _serviceBus;
        private static readonly object LockObject = new object();
        private readonly Dictionary<SenderClientType, Dictionary<string, IEntityEmulator>> _azureMessagingResources;

        #endregion
    }
}