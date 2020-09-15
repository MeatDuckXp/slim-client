using SlimClient.ServiceBus.Azure.Core;
using SlimClient.ServiceBus.Azure.Emulator.Core;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions; 

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Topic subscription client emulator.
    /// </summary>
    /// <remarks>
    ///     This class is intended to hold the reference to the message handler, the subscription name and the rule that will
    ///     be evaluated when a message is received by topic.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public class SubscriptionEmulator
    {
        private Action<Envelope, string> _messageHandler;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name">Subscription name</param>
        /// <param name="rule">Filtering rule</param>
        public SubscriptionEmulator(string name, Expression<Func<string, string, bool>> rule)
        {
            Name = name.ValidateIsStringNullOrEmptyThrowsArgumentNullException("SubscriptionNameIsNullOrEmpty").EnsureToUpper();
            Rule = rule.ValidateIsNotNullThrowsArgumentNullException("SubscriptionRuleIsNull");
        }

        /// <summary>
        ///     Gets subscription name
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Subscription delivery rule.
        /// </summary>
        /// <remarks>
        ///     Based on the value that this rule returns, once evaluated the subscription will receive the message or not.
        /// </remarks>
        public Expression<Func<string, string, bool>> Rule { get; }

        /// <summary>
        ///     Registers message handler for the messages the subscription will receive
        /// </summary>
        /// <param name="handler">Handler instance</param>
        public void RegisterMessageHandler(Action<Envelope, string> handler)
        {
            _messageHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>
        ///     Forwards the message to the handler.
        /// </summary>
        /// <param name="envelope">Envelope instance.</param>
        public void HandleMessageReceived(Envelope envelope)
        {
            _messageHandler?.Invoke(envelope, Name);
        }
    }
}