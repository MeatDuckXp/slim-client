using System;

namespace SlimClient.ServiceBus.Azure.Entities
{
    /// <summary>
    ///     Defines details about deserialization error that occured while materializing the message
    /// </summary>
    public class DeserializationErrorDetail
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="exception">Exception that occured during the deserialization process.</param>
        /// <param name="message">Raw message state that failed the deserialization process.</param>
        public DeserializationErrorDetail(Exception exception, string message)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Message = message ?? throw new ArgumentNullException(nameof(message)); 
        }

        /// <summary>
        ///     The Exception associated with the event.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     The message that failed the deserialization process.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     The correlation ID of the message that failed the deserialization process.
        /// </summary>
        public string CorrelationId { get; }

        #region Static methods

        /// <summary>
        ///     Simple factory method to create the <see cref="DeserializationErrorDetail"/> object.
        /// </summary>
        /// <param name="exception">Exception that occured during the deserialization process.</param>
        /// <param name="message">Raw message state that failed the deserialization process.</param> 
        /// <returns>DeserializationErrorDetail</returns>
        public static DeserializationErrorDetail Create(Exception exception, string message)
        {
            var instance = new DeserializationErrorDetail(exception, message);
            return instance;
        }

        #endregion
    }
}