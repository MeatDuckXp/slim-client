using SlimClient.ServiceBus.Azure.Core;
using SlimClient.ServiceBus.Azure.Emulator.Core;
using SlimClient.ServiceBus.Azure.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Timers; 

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Represents abstract queue emulator capable of receiving a message and delivering it to the registered message handler. The message type is currently
    ///     hardcoded Envelope.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class QueueEmulator : IEntityEmulator
    {
        #region Fields 
        private Action<Envelope, string> _messageHandler;
        private Timer _queueWatchDogTimer;
        private readonly Queue<Envelope> _queue;
        #endregion 
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Queue name</param>
        public QueueEmulator(string name)
        {
            Name = name.EnsureToUpper();
            _queue = new Queue<Envelope>(); 
        }

        /// <summary>
        /// Gets queue name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Sends message envelope
        /// </summary>
        /// <param name="messageEnvelope">Envelope instance</param>
        public void Send(Envelope messageEnvelope)
        {
            messageEnvelope.ValidateIsNotNullThrowsArgumentNullException("MessageEnvelopeIsNull"); 
            _queue.Enqueue(messageEnvelope); 
        }

        /// <summary>
        /// Registers message received handler
        /// </summary>
        /// <param name="handler">Message handler instance</param>
        /// <param name="entityPath">Entity path</param>
        public void RegisterMessageHandler(Action<Envelope, string> handler, string entityPath)
        {
            _messageHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            OnMessageSenderSet();
        }

        #region Private Methods

        /// <summary>
        ///     Execute all action needed when an message sender has been set
        /// </summary>
        private void OnMessageSenderSet()
        { 
            _queueWatchDogTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = Constraints.ScanInterval

            };
            _queueWatchDogTimer.Elapsed += CheckQueueForMessage; 
        }

        /// <summary>
        ///     Execute the logic required to fetch the next message.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">ElapsedEventArgs</param>
        private void CheckQueueForMessage(object sender, ElapsedEventArgs e)
        { 
            if (_queue.Count == 0)
            {
                return;
            }

            var currentEnvelope = _queue.Dequeue();
            _messageHandler?.Invoke(currentEnvelope, Name);
        }

        #endregion
    }
}
