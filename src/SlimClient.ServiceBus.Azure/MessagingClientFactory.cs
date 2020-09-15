using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;

using SlimClient.ServiceBus.Azure.Configuration;
using SlimClient.ServiceBus.Azure.Entities;
using SlimClient.ServiceBus.Azure.Interfaces;

namespace SlimClient.ServiceBus.Azure
{
    /// <summary>
    /// Defines needed operations to perform azure service bus client creation
    /// </summary>
    /// <remarks>
    /// Currently the service bus clients are divided into receiving and sending clients to reduce the number of create methods.
    /// </remarks>
    public class MessagingClientFactory : IMessagingClientFactory
    {
        #region Fields

        private readonly ConcurrentDictionary<string, QueueClient> _queueClients;
        private readonly ConcurrentDictionary<string, TopicClient> _topicClients;
        private readonly ServiceBusConfiguration _configuration;
        private readonly object _createSenderLock;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">ServiceBusConfiguration</param>
        public MessagingClientFactory(ServiceBusConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            ValidateConnectionStringValue();

            _queueClients = new ConcurrentDictionary<string, QueueClient>();
            _topicClients = new ConcurrentDictionary<string, TopicClient>();
            _createSenderLock = new object();
        }

        /// <summary>
        /// Creates instance of message receiving clients (Queue or Subscription). The instances are not cached as they are intented to be used as part of message handling subscriptions. 
        /// </summary> 
        /// <param name="type">ReceiverClientType</param>
        /// <param name="messagingEntityPath">Messaging Entity Path (Topic/Queue name)</param>
        /// <param name="subscriptionName">For Subscriptions - subscription name</param>
        /// <returns>IReceiverClient</returns>
        /// <remarks>
        /// ExcludeFromCodeCoverage added, since creating MessageReceiver or SubscriptionClient with fake connection strings
        /// will result in exception and thus are not suitable to unit test.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        public IReceiverClient CreateReceiverClient(ReceiverClientType type, string messagingEntityPath, string subscriptionName = null)
        {
            if (string.IsNullOrWhiteSpace(messagingEntityPath))
            {
                throw new ArgumentNullException(nameof(messagingEntityPath));
            }

            switch (type)
            {
                case ReceiverClientType.Queue:
                    {
                        TraceEntityResourceDetails(type, messagingEntityPath);

                        return new MessageReceiver(_configuration.ServiceBusConnectionString, messagingEntityPath);
                    }
                case ReceiverClientType.Subscription:
                    {
                        if (string.IsNullOrWhiteSpace(subscriptionName))
                        {
                            throw new ArgumentNullException(nameof(subscriptionName));
                        }

                        TraceEntityResourceDetails(type, messagingEntityPath, subscriptionName);

                        return new SubscriptionClient(_configuration.ServiceBusConnectionString, messagingEntityPath, subscriptionName);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        /// Creates instance of message message sending clients (Topic or Subscription). The instances are cached and reused.
        /// </summary>
        /// <param name="clientTypeType">Sender Client Type</param>
        /// <param name="messagingEntityPath">Messaging Entity Path (Topic/Queue name)</param>
        /// <returns>ISenderClient</returns>
        /// <remarks>
        /// ExcludeFromCodeCoverage added, since creating QueueClient or TopicClient with fake connection strings
        /// will result in exception and thus are not suitable to unit test.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        public ISenderClient CreateSenderClient(SenderClientType clientTypeType, string messagingEntityPath)
        {
            if (string.IsNullOrWhiteSpace(messagingEntityPath))
            {
                throw new ArgumentNullException(nameof(messagingEntityPath));
            }

            switch (clientTypeType)
            {
                case SenderClientType.Queue:
                    if (!_queueClients.ContainsKey(messagingEntityPath))
                    {
                        lock (_createSenderLock)
                        {
                            if (!_queueClients.ContainsKey(messagingEntityPath))
                            {
                                TraceEntityResourceDetails(clientTypeType, messagingEntityPath);
                                _queueClients[messagingEntityPath] = new QueueClient(_configuration.ServiceBusConnectionString, messagingEntityPath, ReceiveMode.PeekLock, null);
                            }
                        }
                    }
                    return _queueClients[messagingEntityPath];
                case SenderClientType.Topic:
                    if (!_topicClients.ContainsKey(messagingEntityPath))
                    {
                        lock (_createSenderLock)
                        {
                            if (!_topicClients.ContainsKey(messagingEntityPath))
                            {
                                TraceEntityResourceDetails(clientTypeType, messagingEntityPath);
                                _topicClients[messagingEntityPath] = new TopicClient(_configuration.ServiceBusConnectionString, messagingEntityPath);
                            }
                        }
                    }
                    return _topicClients[messagingEntityPath];
                default:
                    throw new ArgumentOutOfRangeException(nameof(clientTypeType), clientTypeType, null);
            }
        }

        #region Tracing methods

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="type"></param>
        /// <param name="messagingEntityPath"></param>
        /// <param name="subscriptionName"></param>
        [ExcludeFromCodeCoverage]
        private void TraceEntityResourceDetails(ReceiverClientType type, string messagingEntityPath, string subscriptionName = default(string))
        {
            if (!_configuration.VerboseResourceDetails)
            {
                return;
            }

            var info = new StringBuilder();
            var managementClient = new ManagementClient(_configuration.ServiceBusConnectionString);

            if (ReceiverClientType.Queue == type)
            {
                var queueDetail = managementClient.GetQueueAsync(messagingEntityPath).Result;

                info.Append($"Azure Service Bus Queue Details: ")
                    .Append("\n\tPath: ").Append(queueDetail.Path)
                    .Append("\n\tLock Duration (Total Minutes): ").Append(queueDetail.LockDuration.TotalMinutes)
                    .Append("\n\tMaxSize In MB: ").Append(queueDetail.MaxSizeInMB)
                    .Append("\n\tRequires Duplicate Detection: ").Append(queueDetail.RequiresDuplicateDetection)
                    .Append("\n\tRequires Session: ").Append(queueDetail.RequiresSession)
                    .Append("\n\tDefault Message Time To Live (Total Minutes):  ").Append(queueDetail.DefaultMessageTimeToLive.TotalMinutes)
                    .Append("\n\tAutoDelete On Idle (Total Minutes): ").Append(queueDetail.AutoDeleteOnIdle.TotalMinutes)
                    .Append("\n\tDead Lettering Enabled On Message Epire: ").Append(queueDetail.EnableDeadLetteringOnMessageExpiration)
                    .Append("\n\tStatus: ").Append(queueDetail.Status)
                    .Append("\n\tDuplicate Detection History Time Window: ").Append(queueDetail.DuplicateDetectionHistoryTimeWindow)
                    .Append("\n\tMax Delivery Count: ").Append(queueDetail.MaxDeliveryCount)
                    .Append("\n\tEnable Batched Operations: ").Append(queueDetail.EnableBatchedOperations)
                    .Append("\n\tForward Dead Lettered Messages To: ").Append(queueDetail.ForwardDeadLetteredMessagesTo)
                    .Append("\n\tForward To: ").Append(queueDetail.ForwardTo)
                    .Append("\n\tEnable Partitioning: ").Append(queueDetail.EnablePartitioning)
                    .Append("\n\tUser Metadata: ").Append(queueDetail.UserMetadata)
                    .Append("\n\tAuthorization Rules: ").Append(queueDetail.AuthorizationRules);
            }
            else if (ReceiverClientType.Subscription == type)
            {
                var subscriptionDetail = managementClient.GetSubscriptionAsync(messagingEntityPath, subscriptionName).Result;

                info.Append($"Azure Service Bus Subscription Details: ")
                    .Append("\n\tLock Duration (Total Minutes): ").Append(subscriptionDetail.LockDuration.TotalMinutes)
                    .Append("\n\tRequires Session: ").Append(subscriptionDetail.RequiresSession)
                    .Append("\n\tDefault Message Time To Live (Total Minutes):  ").Append(subscriptionDetail.DefaultMessageTimeToLive.TotalMinutes)
                    .Append("\n\tAutoDelete On Idle (Total Minutes): ").Append(subscriptionDetail.AutoDeleteOnIdle.TotalMinutes)
                    .Append("\n\tDead Lettering Enabled On Message Epiration: ").Append(subscriptionDetail.EnableDeadLetteringOnMessageExpiration)
                    .Append("\n\tDead Lettering On Filter Evaluation Exceptions Enabled: ").Append(subscriptionDetail.EnableDeadLetteringOnFilterEvaluationExceptions)
                    .Append("\n\tPath: ").Append(subscriptionDetail.TopicPath)
                    .Append("\n\tSubscription Name: ").Append(subscriptionDetail.SubscriptionName)
                    .Append("\n\tMax Delivery Count: ").Append(subscriptionDetail.MaxDeliveryCount)
                    .Append("\n\tStatus: ").Append(subscriptionDetail.Status)
                    .Append("\n\tForward To: ").Append(subscriptionDetail.ForwardTo)
                    .Append("\n\tForward Dead Lettered Messages To: ").Append(subscriptionDetail.ForwardDeadLetteredMessagesTo)
                    .Append("\n\tEnable Batched Operations: ").Append(subscriptionDetail.EnableBatchedOperations)
                    .Append("\n\tUser Metadata: ").Append(subscriptionDetail.UserMetadata);
            }

            //handle this
            var entityResourceDetails = info.ToString();
        }

        /// <summary>
        /// TBD
        /// </summary>
        /// <param name="type"></param>
        /// <param name="messagingEntityPath"></param>
        [ExcludeFromCodeCoverage]
        private void TraceEntityResourceDetails(SenderClientType type, string messagingEntityPath)
        {
            if (!_configuration.VerboseResourceDetails)
            {
                return;
            }

            var info = new StringBuilder();
            var managementClient = new ManagementClient(_configuration.ServiceBusConnectionString);

            if (SenderClientType.Queue == type)
            {
                var queueDetail = managementClient.GetQueueAsync(messagingEntityPath).Result;

                info.Append($"Azure Service Bus Queue Details: ")
                    .Append("\n\tPath: ").Append(queueDetail.Path)
                    .Append("\n\tLock Duration (Total Minutes): ").Append(queueDetail.LockDuration.TotalMinutes)
                    .Append("\n\tMaxSize In MB: ").Append(queueDetail.MaxSizeInMB)
                    .Append("\n\tRequires Duplicate Detection: ").Append(queueDetail.RequiresDuplicateDetection)
                    .Append("\n\tRequires Session: ").Append(queueDetail.RequiresSession)
                    .Append("\n\tDefault Message Time To Live (Total Minutes):  ").Append(queueDetail.DefaultMessageTimeToLive.TotalMinutes)
                    .Append("\n\tAutoDelete On Idle (Total Minutes): ").Append(queueDetail.AutoDeleteOnIdle.TotalMinutes)
                    .Append("\n\tDead Lettering Enabled On Message Epire: ").Append(queueDetail.EnableDeadLetteringOnMessageExpiration)
                    .Append("\n\tStatus: ").Append(queueDetail.Status)
                    .Append("\n\tDuplicate Detection History Time Window: ").Append(queueDetail.DuplicateDetectionHistoryTimeWindow)
                    .Append("\n\tMax Delivery Count: ").Append(queueDetail.MaxDeliveryCount)
                    .Append("\n\tEnable Batched Operations: ").Append(queueDetail.EnableBatchedOperations)
                    .Append("\n\tForward Dead Lettered Messages To: ").Append(queueDetail.ForwardDeadLetteredMessagesTo)
                    .Append("\n\tForward To: ").Append(queueDetail.ForwardTo)
                    .Append("\n\tEnable Partitioning: ").Append(queueDetail.EnablePartitioning)
                    .Append("\n\tUser Metadata: ").Append(queueDetail.UserMetadata)
                    .Append("\n\tAuthorization Rules: ").Append(queueDetail.AuthorizationRules);
            }
            else if (SenderClientType.Topic == type)
            {
                var subscriptionDetail = managementClient.GetTopicAsync(messagingEntityPath).Result;

                info.Append($"Azure Service Bus Topic Details: ")
                    .Append("\n\tDefault Message Time To Live (Total Minutes):  ").Append(subscriptionDetail.DefaultMessageTimeToLive.TotalMinutes)
                    .Append("\n\tAutoDelete On Idle (Total Minutes): ").Append(subscriptionDetail.AutoDeleteOnIdle.TotalMinutes)
                    .Append("\n\tMax Size In MB: ").Append(subscriptionDetail.MaxSizeInMB)
                    .Append("\n\tStatus: ").Append(subscriptionDetail.Status)
                    .Append("\n\tEnable Batched Operations: ").Append(subscriptionDetail.EnableBatchedOperations)
                    .Append("\n\tRequires Duplicate Detection: ").Append(subscriptionDetail.RequiresDuplicateDetection)
                    .Append("\n\tDuplicate Detection History Time Window: ").Append(subscriptionDetail.DuplicateDetectionHistoryTimeWindow)
                    .Append("\n\tEnable Partitioning: ").Append(subscriptionDetail.EnablePartitioning)
                    .Append("\n\tSupportOrdering: ").Append(subscriptionDetail.SupportOrdering)
                    .Append("\n\tUser Metadata: ").Append(subscriptionDetail.UserMetadata)
                    .Append("\n\tPath: ").Append(subscriptionDetail.Path);
            }

            //handle this
            var entityResourceDetails = info.ToString();
        }

        #endregion

        #region Private Valdiation methods

        /// <summary>
        /// Validates the configuration, verifying that the ServiceBusConnectionString propery is set. Otherwise it will throw ArgumentException
        /// </summary>
        private void ValidateConnectionStringValue()
        {
            if (string.IsNullOrWhiteSpace(_configuration.ServiceBusConnectionString))
            {
                throw new ArgumentException("ServiceBusConnectionStringIsNullOrEmpty");
            }
        }

        #endregion
    }
}
