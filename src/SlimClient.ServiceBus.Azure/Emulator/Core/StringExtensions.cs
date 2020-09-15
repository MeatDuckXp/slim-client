using System.Diagnostics.CodeAnalysis;

namespace SlimClient.ServiceBus.Azure.Emulator.Core
{
    [ExcludeFromCodeCoverage]
    public static class StringExtensions
    {
        public static string EnsureToUpper(this string stringValue)
        {
            return stringValue.ToUpperInvariant().Trim();
        }
    }
}
