using SlimClient.ServiceBus.Azure.Core;
using SlimClient.ServiceBus.Azure.Emulator.Core;
using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Interfaces;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks; 

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Provides set of methods that can be used to send a message to the service bus emulator queue or topic.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MessageSenderEmulator : IMessageSender
    {
        /// <summary>
        ///     Sends message to service bus emulator queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="messageBody">Serialized message body.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="sourceName">Creator service name</param>
        /// <param name="sourceId">Creator service id</param>
        public void Send(SenderClientType clientType, string messageBody, string entityPath, string sourceName, string sourceId)
        {
            messageBody.ValidateIsStringNullOrEmptyThrowsArgumentException("EnvelopeIsNullOrWhiteSpace");
            InternalSend(clientType, EnvelopeFactory.Create(messageBody, sourceName, sourceId), entityPath);
        }

        /// <summary>
        ///     Sends message to service bus emulator queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        public void Send(SenderClientType clientType, Envelope message, string entityPath)
        {
            InternalSend(clientType, message, entityPath);
        }

        /// <summary>
        ///     Sends message to service bus emulator queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="messageBody">Serialized message body.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="sourceName">Creator service name</param>
        /// <param name="sourceId">Creator service id</param>
        public Task SendAsync(SenderClientType clientType, string messageBody, string entityPath, string sourceName, string sourceId)
        {
            messageBody.ValidateIsStringNullOrEmptyThrowsArgumentException("EnvelopeIsNullOrWhiteSpace");
            InternalSend(clientType, EnvelopeFactory.Create(messageBody, sourceName, sourceId), entityPath);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sends message to service bus emulator queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        public Task SendAsync(SenderClientType clientType, Envelope message, string entityPath)
        {
            InternalSend(clientType, message, entityPath);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="timeToLive">Message time to live.</param>
        public Task SendAsync(SenderClientType clientType, Envelope message, string entityPath, int timeToLive)
        {
            InternalSend(clientType, message, entityPath);
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Sends message to service bus emulator queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="scheduleEnqueueTimeUtc">Schedule enqueue time UTC.</param>
        public Task ScheduleMessageAsync(SenderClientType clientType, Envelope message, string entityPath, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            var millisecondsToDeferMessage = scheduleEnqueueTimeUtc.Subtract(DateTime.Now).TotalMilliseconds;

            if (millisecondsToDeferMessage > 0)
            {
                var timer = new System.Timers.Timer();
                timer.AutoReset = false;
                timer.Interval = millisecondsToDeferMessage;
                timer.Elapsed += (s,e) => InternalSend(clientType, message, entityPath);
                timer.Start();
            }
            else
            {
                InternalSend(clientType, message, entityPath);
            }

            return Task.CompletedTask;
        }

        #region Private Methods

        /// <summary>
        ///     Internal Sends message to service bus emulator queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        private void InternalSend(SenderClientType clientType, Envelope message, string entityPath)
        {
            message.ValidateIsNotNullThrowsArgumentException("EnvelopeIsNull");
            entityPath.ValidateIsStringNullOrEmptyThrowsArgumentException("EntityPathIsNullOrEmpty");
            ServiceBusEmulator.Instance.Send(clientType, message, entityPath.EnsureToUpper());
        }

        #endregion
    }
}