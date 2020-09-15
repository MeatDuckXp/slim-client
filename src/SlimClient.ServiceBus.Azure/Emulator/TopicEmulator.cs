using SlimClient.ServiceBus.Azure.Core;
using SlimClient.ServiceBus.Azure.Emulator.Core;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Timers; 

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Represents abstract queue emulator capable of receiving a message and delivering to the registered subscription.
    ///     The message type is currently
    ///     hardcoded Envelope.
    /// </summary>
    /// <remarks>
    ///     In this implementation, the Topic is the top level container, containing instances of subscriptions. The Topic is
    ///     implemented as simple queue. The topic is responsible to accept
    ///     the message, place it in the queue and the periodical queue scanning for new messages. If there is a new message in
    ///     the queue, the topic will pick it up and forward it to the proper subscription, based on the predefined filtering
    ///     rules.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class TopicEmulator : IEntityEmulator
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name">Topic name</param>
        public TopicEmulator(string name)
        {
            Name = name.EnsureToUpper();
            _topic = new Queue<Envelope>();
            _subscriptions = new Dictionary<string, SubscriptionEmulator>();
        }

        /// <summary>
        ///     Gets topic name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Sends message envelope to topic.
        /// </summary>
        /// <param name="messageEnvelope">Envelope</param>
        public void Send(Envelope messageEnvelope)
        {
            messageEnvelope.ValidateIsNotNullThrowsArgumentNullException("MessageEnvelopeIsNull");
            _topic.Enqueue(messageEnvelope);
        }

        /// <summary>
        ///     Forwards the message handler registration to the given subscription.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="entityPath"></param>
        public void RegisterMessageHandler(Action<Envelope, string> handler, string entityPath)
        {
            _subscriptions[entityPath].RegisterMessageHandler(handler);
            OnMessageSenderSet();
        }

        /// <summary>
        ///     Registers the provided subscription to handles the incoming message
        /// </summary>
        /// <param name="subscription">Topic Subscription</param>
        public void AddSubscription(SubscriptionEmulator subscription)
        {
            subscription.ValidateIsNotNullThrowsArgumentNullException("SubscriptionIsNull");
            _subscriptions.Add(subscription.Name, subscription);
        }

        #region Fields 

        private Timer _topicWatchDogTimer;
        private readonly Queue<Envelope> _topic;
        private readonly Dictionary<string, SubscriptionEmulator> _subscriptions;

        #endregion

        #region Private Methods

        /// <summary>
        ///     Execute all action needed when an message sender has been set
        /// </summary>
        private void OnMessageSenderSet()
        {
            _topicWatchDogTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = Constraints.ScanInterval
            };
            _topicWatchDogTimer.Elapsed += CheckTopicForMessage;
        }

        /// <summary>
        ///     Execute the logic required to fetch the next message.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">ElapsedEventArgs</param>
        private void CheckTopicForMessage(object sender, ElapsedEventArgs e)
        {
	        if (_topic.Count == 0)
	        {
		        return;
	        }

            var currentEnvelope = _topic.Dequeue();
            var registeredHandlers = new List<SubscriptionEmulator>();//_subscriptions.Values.Where(item => item.Rule.Compile().Invoke()).ToList();

            foreach (var handler in registeredHandlers)
            {
	            handler.HandleMessageReceived(currentEnvelope);
            }
        }

        #endregion
    }
}