using SlimClient.ServiceBus.Azure.Entities;
using System;
using System.Threading.Tasks; 

namespace SlimClient.ServiceBus.Azure.Interfaces
{
    /// <summary>
    /// Implements common Message Handler properties, allowing the client to care only about the process message method implementation. 
    /// </summary>
    public abstract class BaseMessageHandler : IMessageHandler
    {
        /// <summary>
        /// Gets client type that is the message handler subscribing to.
        /// </summary>
        public ReceiverClientType ReceiverClientType { get; protected set; }

        /// <summary>
        /// Gets the preferred delivery mode. If the handler will access only the payload, the DeliveryMode.Content should be used. 
        /// </summary>
        public DeliveryMode Mode { get; protected set; }

        /// <summary>
        /// Gets or Sets entity path (Queue/Topic name)
        /// </summary>
        public string EntityPath { get; protected set; }

        /// <summary>
        /// Gets subscription name in case subscription is our target to fetch the messages from.
        /// </summary>
        public string SubscriptionName { get; protected set; }

        #region Constructors

        /// <summary>
        ///Constructor
        /// </summary>
        /// <param name="receivingType">Receiver client type.</param>
        /// <param name="receivingMode">Service receiving mode.</param>
        /// <param name="entityPath">Entity path.</param>
        /// <param name="subscriptionName">Subscription name.</param>
        protected BaseMessageHandler(ReceiverClientType receivingType, DeliveryMode receivingMode, string entityPath, string subscriptionName)
        { 
            ReceiverClientType = receivingType;
            Mode = receivingMode;
            EntityPath = entityPath;
            SubscriptionName = subscriptionName;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="receivingType">Receiver client type.</param>
        /// <param name="receivingMode">Service receiving mode.</param>
        /// <param name="entityPath">Entity path.</param>
        protected BaseMessageHandler(ReceiverClientType receivingType, DeliveryMode receivingMode, string entityPath) : this(receivingType, receivingMode, entityPath, string.Empty)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="receivingType">Receiver client type.</param>
        /// <param name="receivingMode">Service receiving mode.</param>
        protected BaseMessageHandler(ReceiverClientType receivingType, DeliveryMode receivingMode) : this(receivingType, receivingMode, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseMessageHandler() 
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method is invoked when an message is received. It should contain logic that is needed to process the message.
        /// </summary>
        /// <typeparam name="T">Type of T</typeparam>
        /// <param name="message">Message instance</param>
        /// <returns>If successfully processed True, otherwise False.</returns>
        public abstract Task<bool> ProcessMessage<T>(T message);

        /// <summary>
        /// Is invoked in case an error occurs while processing the message.
        /// </summary>
        /// <param name="errorDetail">Error detail</param>
        public virtual void ProcessError(ErrorDetail errorDetail)
        {
        }

        /// <summary>
        /// Is invoked when the message fails the deserialization process.
        /// </summary>
        /// <param name="deserializationErrorDetail">Error details.</param>
        /// <returns>True if the service handled the error, otherwise false.</returns>
        public virtual bool ProcessDeserializationError(DeserializationErrorDetail deserializationErrorDetail)
        {
            //If this method does ot get overriden, then we failed to handle the situation and this is signaled as such. If this gets executed, thn the message will be moved to the dead letter queue.
            return false;
        }

        #endregion

        /// <summary>
        ///     Flatten exception and inner exception messages.
        /// </summary>
        /// <remarks>
        ///     This is used to prepare the error description when an exception is thrown during processing.
        /// </remarks>
        /// <param name="exception">Exception.</param>
        /// <returns>Flattened exception messages.</returns>
        protected static string FlattenExceptionMessages(Exception exception)
        {
            return exception.InnerException == null ?
                exception.Message :
                string.Join(",", exception.Message, FlattenExceptionMessages(exception.InnerException));
        }
    }
}
