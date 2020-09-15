namespace SlimClient.ServiceBus.Azure.Interfaces
{
    /// <summary>
    /// Defines set of operations related to message handler registration.
    /// </summary>
    public interface IMessageHandlerRegistry
    {
        /// <summary>
        /// Registers message handler that will be invoked to process incoming message
        /// </summary>
        /// <param name="messageHandler"></param>
        void RegisterMessageHandler(IMessageHandler messageHandler);

        /// <summary>
        /// Returns number of registered message handlers. 
        /// </summary>
        int MessageHandlerCount { get; }
    }
}
