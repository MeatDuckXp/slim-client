using SlimClient.ServiceBus.Azure.Core;
using SlimClient.ServiceBus.Azure.Emulator.Core;
using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Interfaces;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; 

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Emulates the behavior of the actual message handler registry
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MessageHandlerRegistryEmulator : IMessageHandlerRegistry
    {
        private readonly Dictionary<string, IMessageHandler> _messageHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        public MessageHandlerRegistryEmulator()
        {
            _messageHandler = new Dictionary<string, IMessageHandler>();
        }

        /// <summary>
        ///     Registers message handler that will be invoked to process incoming message
        /// </summary>
        /// <param name="messageHandler"></param>
        public void RegisterMessageHandler(IMessageHandler messageHandler)
        {
            messageHandler.ValidateIsNotNullThrowsArgumentNullException();
            messageHandler.EntityPath.ValidateIsNotNullThrowsArgumentException();
            SetMessageHandler(messageHandler);
            ServiceBusEmulator.Instance.RegisterMessageHandler(ResolveSenderType(messageHandler.ReceiverClientType), InternalMessageHandler, messageHandler.EntityPath.EnsureToUpper(), messageHandler.SubscriptionName.EnsureToUpper());
        }

        /// <summary>
        ///     Returns number of registered message handlers.
        /// </summary>
        public int MessageHandlerCount => _messageHandler.Count;

        #region Private Methods

        /// <summary>
        ///     Resolves the sender type for the provided receiver client type
        /// </summary>
        /// <param name="receiverClientType">ReceiverClientType</param>
        /// <returns>SenderClientType</returns>
        private static SenderClientType ResolveSenderType(ReceiverClientType receiverClientType)
        {
            switch (receiverClientType)
            {
                case ReceiverClientType.Queue:
                    return SenderClientType.Queue;
                case ReceiverClientType.Subscription:
                    return SenderClientType.Topic;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Forward the received envelope to the registered message handler, if any.
        /// </summary>
        /// <param name="envelope">Envelope</param>
        /// <param name="entityPath">Entity path</param>
        private void InternalMessageHandler(Envelope envelope, string entityPath)
        {
            //This step maps to the real world step in ReceiverClientWrapper.ProcessMessagesAsync step:
            //1.5 All objects are wrapped into envelope currently, thus needs to be unwrapped
            var preparedEnvelope = MaterializeObject(envelope);

            _messageHandler[entityPath]?.ProcessMessage(preparedEnvelope);
        }

        /// <summary>
        ///     Sets message handler based on the current receiver client type instance
        /// </summary>
        /// <param name="messageHandler"></param>
        private void SetMessageHandler(IMessageHandler messageHandler)
        {
            switch (messageHandler.ReceiverClientType)
            {
                //In case of a queue, we send and receive on the same queue, therefore the handler entity path is queue name.
                case ReceiverClientType.Queue:
                    _messageHandler[messageHandler.EntityPath.EnsureToUpper()] = messageHandler;
                    break;
                //in case of a topic, we send to a topic instance, but receive on a subscription instance, therefore in this case we register the handler by subscription name
                case ReceiverClientType.Subscription:
                    _messageHandler[messageHandler.SubscriptionName.EnsureToUpper()] = messageHandler;
                    break;
            }
        }

        /// <summary>
        /// Recreates object state to the initial value
        /// </summary>
        /// <param name="envelope">Serialized message body</param>
        /// <returns>Envelope</returns>
        private static Envelope MaterializeObject(Envelope envelope)
        {
            var messageObject = envelope;

            var objectType = Type.GetType(messageObject.ContentType);
            var objectStateBase64Encoded = messageObject.Content.ToString();
            
            //TODO(Vedran): Refactor this to become implicit statement when getting the content.
            var base64EncodedBytes = System.Convert.FromBase64String(objectStateBase64Encoded);
            var objectState = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            
            messageObject.Body.Content = ObjectStateTransformation.DeserializeJson(objectType, objectState);

            return messageObject;
        }

        #endregion
    }
}