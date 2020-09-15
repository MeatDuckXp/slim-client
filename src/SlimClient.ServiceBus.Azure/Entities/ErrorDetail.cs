using System;

namespace SlimClient.ServiceBus.Azure.Entities
{
    /// <summary>
    /// Defines details about the exception occured while reading the message from queue
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <param name="action">Action</param>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="entityPath">Entity path</param>
        /// <param name="clientId">Client Id</param>
        // ReSharper disable once TooManyDependencies
        public ErrorDetail(Exception exception, string action, string endpoint, string entityPath, string clientId)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Action = action ?? throw new ArgumentNullException(nameof(action));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            EntityPath = entityPath ?? throw new ArgumentNullException(nameof(entityPath));
            ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        }

        /// <summary>
        /// The Exception associated with the event.
        /// </summary>
        public Exception Exception { get;  }

        /// <summary>
        /// Gets the action associated with the event.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// The namespace name used when this exception occurred.
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// The entity path used when this exception occurred.
        /// </summary>
        public string EntityPath { get; }

        /// <summary>
        /// The Client Id associated with the sender, receiver or session when this exception occurred.
        /// </summary>
        public string ClientId { get; }
    }
}
