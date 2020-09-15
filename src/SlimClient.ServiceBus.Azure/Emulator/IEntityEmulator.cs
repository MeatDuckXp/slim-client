using SlimClient.ServiceBus.Azure.Messages;
using System; 

namespace SlimClient.ServiceBus.Azure.Emulator
{
    /// <summary>
    ///     Common interface for queue and topic service bus resources
    /// </summary>
    public interface IEntityEmulator
    {
        /// <summary>
        ///     Gets entity path/name
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Sends envelope to the given resource
        /// </summary>
        /// <param name="messageEnvelope"></param>
        void Send(Envelope messageEnvelope);

        /// <summary>
        ///     Registers the message handler that will react to the message(s)
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="entityPath">Entity Path</param>
        void RegisterMessageHandler(Action<Envelope, string> handler, string entityPath);
    }
}