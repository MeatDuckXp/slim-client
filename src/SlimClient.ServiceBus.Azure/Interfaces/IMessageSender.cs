using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Threading.Tasks; 

namespace SlimClient.ServiceBus.Azure.Interfaces
{
    /// <summary>
    ///     Defines set of methods that can be used to send a message to service bus queue or topic.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="messageBody">Serialized message body.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="sourceName">Creator service name</param>
        /// <param name="sourceId">Creator service id</param>
        void Send(SenderClientType clientType, string messageBody, string entityPath, string sourceName, string sourceId);

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        void Send(SenderClientType clientType, Envelope message, string entityPath);

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="messageBody">Serialized message body.</param> 
        /// <param name="entityPath">Entity path</param>
        /// <param name="sourceName">Creator service name</param>
        /// <param name="sourceId">Creator service id</param>
        Task SendAsync(SenderClientType clientType, string messageBody, string entityPath, string sourceName, string sourceId);

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        Task SendAsync(SenderClientType clientType, Envelope message, string entityPath);

        /// <summary>
        ///     Sends message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="timeToLive">Message time to live.</param>
        Task SendAsync(SenderClientType clientType, Envelope message, string entityPath, int timeToLive);

        /// <summary>
        ///     Schedules message to service bus queue or topic.
        /// </summary>
        /// <param name="clientType">Defines type of sender intended to be used.</param>
        /// <param name="message">Message object.</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="scheduleEnqueueTimeUtc">Schedule enqueue time UTC.</param>
        Task ScheduleMessageAsync(SenderClientType clientType, Envelope message, string entityPath, DateTimeOffset scheduleEnqueueTimeUtc);
    }
}