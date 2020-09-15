using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Interfaces;
using SlimClient.ServiceBus.Azure.Messages;
using SlimClient.ServiceBus.Azure.MessageTranslation;

namespace SlimClient.ServiceBus.Azure
{
    /// <summary>
    /// Wrapper around the message receiver client and the corresponding registered handler.
    /// </summary>
    internal class ReceiverClientWrapper
    {
        #region Fields

        private readonly IReceiverClient _receiverClient;
        private readonly IMessageHandler _messageHandler;
        private readonly MessageHandlerOptions _messageHandlerOptions;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="receiverClient">IReceiverClient instance</param>
        /// <param name="messageHandler">IMessageHandler instance</param>
        public ReceiverClientWrapper(IReceiverClient receiverClient, IMessageHandler messageHandler)
        {
            _receiverClient = receiverClient ?? throw new ArgumentNullException(nameof(receiverClient));
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));

            //as the time progresses, this will have to be configurable
            _messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 5,
                AutoComplete = false
            };

            _receiverClient.RegisterMessageHandler(ProcessMessagesAsync, _messageHandlerOptions);
            _receiverClient.PrefetchCount = 1;
        }

        #region Private Methods

        /// <summary>
        /// Message handler that is receiving messages continuously from the entity.
        /// </summary>
        /// <param name="message">Received message</param>
        /// <param name="token">Cancellation Token</param>
        /// <remarks>
        /// Its is possible to set up the message processing to prefetch message, in order to increase the receive frequency.
        /// </remarks>
        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            //1. Convert to string.
            var messageBody = ServiceBusDefault.Encoding.GetString(message.Body);

            //1.5 All objects are wrapped into envelope currently, thus needs to be unwrapped
            var messageEnvelope = MaterializeObject(messageBody);

            if (messageEnvelope == default)
            {
                //1.6 Event we have handled the situation gracefully, we have to remove the message from the input channel.
                if (!_messageHandlerOptions.AutoComplete)
                {
                    await _receiverClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                }
            }
            else
            {
                //2. Invoke the registered handler.
                bool isMessageProcessed = await _messageHandler.ProcessMessage(_messageHandler.Mode == DeliveryMode.Content ? messageEnvelope.Content : messageEnvelope).ConfigureAwait(false);

                //3. Examine the value returned, if the message was successfully processed and only then
                if (isMessageProcessed)
                {
                    //3.5 Check the handler configuration, if the AutoComplete is set to false, then engage the following logic. Otherwise
                    // calling CompleteAsync when AutoComplete is set to true will result in MessageLockLostException: The lock supplied is invalid. Either the lock expired, or the message has already been removed from the queue
                    if (!_messageHandlerOptions.AutoComplete)
                    {
                        //4. Mark the message as completed, and it will be deleted from the entity
                        await _receiverClient.CompleteAsync(message.SystemProperties.LockToken).ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        ///Recreates the message object state.
        /// </summary>
        /// <param name="messageBody">Serialized message body</param>
        /// <returns>Envelope</returns>
        private Envelope MaterializeObject(string messageBody)
        {
            var envelope = default(Envelope);

            try
            {
                // The body content is more likely to fail deserialization than the envelope itself,
                // so separate deserialization of content so that we can try to retrieve the correlation ID.
                envelope = EnvelopeTransformation.Deserialize(messageBody);
                EnvelopeTransformation.MaterializeContent(envelope);
            }
            catch (Exception exception)
            {
                // Let's attempt to get the correlation ID.
                var errorDetail = DeserializationErrorDetail.Create(exception, messageBody); 
                var hasBeenSanitized = _messageHandler.ProcessDeserializationError(errorDetail);

                if (!hasBeenSanitized)
                {
                    throw;
                }

                //indicate that we were not successful
                return default(Envelope);
            }

            return envelope;
        }

        /// <summary>
        /// Handles any exceptional situation when processing a message.
        /// </summary>
        /// <param name="exception">Exception details.</param>
        /// <returns>Task</returns>
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exception)
        {
            //1. Create our system internal error detail object
            var errorDetail = new ErrorDetail(exception.Exception,
                exception.ExceptionReceivedContext.Action,
                exception.ExceptionReceivedContext.Endpoint,
                exception.ExceptionReceivedContext.EntityPath,
                exception.ExceptionReceivedContext.ClientId);

            //2. Signal handler to process it
            _messageHandler.ProcessError(errorDetail);

            return Task.CompletedTask;
        }

        #endregion

        #region Static Content

        /// <summary>
        /// Creates new instance of ReceiverClientWrapper
        /// </summary>
        /// <param name="queueClient">IReceiverClient</param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public static ReceiverClientWrapper Create(IReceiverClient queueClient, IMessageHandler messageHandler)
        {
            return new ReceiverClientWrapper(queueClient, messageHandler);
        }

        #endregion
    }
}
