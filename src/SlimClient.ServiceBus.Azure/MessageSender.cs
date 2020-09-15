using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Interfaces;
using SlimClient.ServiceBus.Azure.Messages;
using SlimClient.ServiceBus.Azure.MessageTranslation;

namespace SlimClient.ServiceBus.Azure
{
    /// <summary>
    ///     Provides set of methods that can be used to send a message to service bus queue or topic.
    /// </summary>
    public class MessageSender : IMessageSender
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="messagingClientFactory">Service bus connection manager.</param>
        public MessageSender(IMessagingClientFactory messagingClientFactory)
        {
            _messagingClientFactory = messagingClientFactory ?? throw new ArgumentNullException(nameof(messagingClientFactory));
        }

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="messageBody">Serialized message body.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="sourceName">Creator service name</param>
        /// <param name="sourceId">Creator service id</param>
        public void Send(SenderClientType clientType, string messageBody, string entityPath, string sourceName, string sourceId)
        {
            ValidateEnvelopeAsStringParameter(messageBody);
            InternalSend(clientType, EnvelopeFactory.Create(messageBody), entityPath).Wait();
        }

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Service bus resource name</param>
        public void Send(SenderClientType clientType, Envelope message, string entityPath)
        {
            InternalSend(clientType, message, entityPath).Wait();
        }

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="messageBody">Serialized message body.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="sourceName">Creator service name</param>
        /// <param name="sourceId">Creator service id</param>
        public async Task SendAsync(SenderClientType clientType, string messageBody, string entityPath, string sourceName, string sourceId)
        {
            ValidateEnvelopeAsStringParameter(messageBody);

            Task sendTask = InternalSend(clientType, EnvelopeFactory.Create(messageBody), entityPath);
            await sendTask.ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        public async Task SendAsync(SenderClientType clientType, Envelope message, string entityPath)
        {
            Task sendTask = InternalSend(clientType, message, entityPath);
            await sendTask.ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="timeToLive">Message time to live.</param>
        public async Task SendAsync(SenderClientType clientType, Envelope message, string entityPath, int timeToLive)
        {
            Task sendTask = InternalSend(clientType, message, entityPath, timeToLive);
            await sendTask.ConfigureAwait(false);
        }

        /// <summary>
        ///     Schedules message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="scheduleEnqueueTimeUtc">Schedule enqueue time UTC.</param>
        public async Task ScheduleMessageAsync(SenderClientType clientType, Envelope message, string entityPath, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            Task sendTask = InternalScheduleMessage(clientType, message, entityPath, scheduleEnqueueTimeUtc);
            await sendTask.ConfigureAwait(false);
        }

        #region Private Methods

        /// <summary>
        ///     Contains the common logic required to prepare and send the message to a queue or topic.
        /// 
        ///     The preparation process consists of several validation steps, that can result in exceptions.
        /// </summary>
        /// <param name="clientType">Sender client type</param>
        /// <param name="envelope">Envelope</param>
        /// <param name="entityPath">EntityPath</param>
        /// <returns>Task</returns>
        private async Task InternalSend(SenderClientType clientType, Envelope envelope, string entityPath)
        {
            ValidateEnvelopeParameter(envelope);
            ValidateResourceParameter(entityPath);

            var serviceBusMessage = InternalPrepareMessage(envelope, ServiceBusDefault.DefaultMessageTimeToLiveMinutes);

            var senderClient = _messagingClientFactory.CreateSenderClient(clientType, entityPath);
            await senderClient.SendAsync(serviceBusMessage).ConfigureAwait(false);
        }

        /// <summary>
        ///     Contains the common logic required to prepare and send the message to a queue or topic.
        /// 
        ///     The preparation process consists of several validation steps, that can result in exceptions.
        /// </summary>
        /// <param name="clientType">Sender client type</param>
        /// <param name="envelope">Envelope</param>
        /// <param name="entityPath">EntityPath</param>
        /// <param name="timeToLive">Message time to live.</param>
        /// <returns>Task</returns>
        private async Task InternalSend(SenderClientType clientType, Envelope envelope, string entityPath, int timeToLive)
        {
            ValidateEnvelopeParameter(envelope);
            ValidateResourceParameter(entityPath);

            var serviceBusMessage = InternalPrepareMessage(envelope, timeToLive);

            var senderClient = _messagingClientFactory.CreateSenderClient(clientType, entityPath);
            await senderClient.SendAsync(serviceBusMessage).ConfigureAwait(false);
        }

        /// <summary>
        ///     Contains the common logic required to prepare and schedule the message to a queue or topic.
        /// 
        ///     The preparation process consists of several validation steps, that can result in exceptions.
        /// </summary>
        /// <param name="clientType">Sender client type</param>
        /// <param name="envelope">Envelope</param>
        /// <param name="entityPath">EntityPath</param>
        /// <param name="scheduleEnqueueTimeUtc">Schedule enqueue time UTC.</param>
        /// <returns>Task</returns>
        private async Task InternalScheduleMessage(SenderClientType clientType, Envelope envelope, string entityPath, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            ValidateEnvelopeParameter(envelope);
            ValidateResourceParameter(entityPath);

            var serviceBusMessage = InternalPrepareMessage(envelope, ServiceBusDefault.DefaultMessageTimeToLiveMinutes);

            var senderClient = _messagingClientFactory.CreateSenderClient(clientType, entityPath);
            await senderClient.ScheduleMessageAsync(serviceBusMessage, scheduleEnqueueTimeUtc).ConfigureAwait(false);
        }

        /// <summary>
        ///     Prepares a service bus message from the given envelope.
        /// </summary>
        /// <param name="envelope">Envelope.</param>
        /// <param name="timeToLive">Message time to live.</param>
        /// <returns>Service bus message</returns>
        private Message InternalPrepareMessage(Envelope envelope, int timeToLive)
        {
            var serializedEnvelope = EnvelopeTransformation.Serialize(envelope);
            ValidateSerializedEnvelope(serializedEnvelope);

            var serviceBusMessage = new Message(ServiceBusDefault.Encoding.GetBytes(serializedEnvelope))
            {
                TimeToLive = TimeSpan.FromMinutes(timeToLive),
                ContentType = ObjectStateTransformation.Type                
            };

            return serviceBusMessage;
        }

        #endregion

        #region Private Validation Methods

        private static void ValidateSerializedEnvelope(string serializedEnvelope)
        {
            if (string.IsNullOrWhiteSpace(serializedEnvelope))
            {
                throw new ArgumentException("SerializedEnvelopeIsNullOrWhiteSpace");
            }
        }

        private static void ValidateEnvelopeAsStringParameter(string envelope)
        {
            if (string.IsNullOrWhiteSpace(envelope))
            {
                throw new ArgumentException("EnvelopeIsNullOrWhiteSpace");
            }
        }

        private static void ValidateResourceParameter(string entityPath)
        {
            if (string.IsNullOrEmpty(entityPath) || entityPath.Length > ServiceBusDefault.EntityPathMaximumLength)
            {
                throw new ArgumentException("EntityPathIsNullOrEmptyOrOverMaxValue");
            }
        }

        private static void ValidateEnvelopeParameter(Envelope envelope)
        {
            if (envelope == default(Envelope))
            {
                throw new ArgumentException("EnvelopeObjectIsNull");
            }
        }

        #endregion

        #region Fields

        private readonly IMessagingClientFactory _messagingClientFactory;

        #endregion
    }
}
