using System.Diagnostics.CodeAnalysis;

namespace SlimClient.ServiceBus.Azure.Emulator.Core
{
    [ExcludeFromCodeCoverage]
    public class Constraints
    {
        /// <summary>
        /// Defined overall default interval between consecutive scans.
        /// </summary>
        public const double ScanInterval = 5000;
    }
}
