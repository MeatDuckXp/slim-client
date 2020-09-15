using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SlimClient.ServiceBus.Azure
{
    /// <summary>
    /// Defines known defaults and constraints related to azure service bus platform
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class ServiceBusDefault
    {
        /// <summary>
        /// Maximum size of any messaging entity path: queue or topic
        /// </summary>
        /// <remarks>
        ///     https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-quotas
        /// </remarks>
        public static int EntityPathMaximumLength => 260;

        /// <summary>
        /// Gets maximum message size in bytes for premium account. 
        /// </summary>
        public static int MaxMessageSizeBytes => 1048576;

        /// <summary>
        /// Gets default time to live for the messages in queue in minutes
        /// </summary>
        public static int DefaultMessageTimeToLiveMinutes = 720;

        /// <summary>
        /// Gets default encoding used to serialize and deserialize the content of the messages.
        /// </summary>
        public static Encoding Encoding => Encoding.UTF8;
    }
} 