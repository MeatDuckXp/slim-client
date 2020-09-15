using System;
using System.Diagnostics.CodeAnalysis;

namespace SlimClient.ServiceBus.Azure.Core
{
    [ExcludeFromCodeCoverage]
    public static class ValidationOperation
    {
        public static T ValidateIsNotNullThrowsArgumentNullException<T>(this T objectInstance, string exceptionMessage = null) where T : class
        {
            if (default(T) == objectInstance)
            {
                throw new ArgumentNullException(exceptionMessage ?? nameof(objectInstance));
            }
            return objectInstance;
        }

        public static T ValidateIsNotNullThrowsArgumentException<T>(this T objectInstance, string exceptionMessage = null) where T : class
        {
            if (default(T) == objectInstance)
            {
                throw new ArgumentException(exceptionMessage ?? nameof(objectInstance));
            }
            return objectInstance;
        }

        public static string ValidateIsStringNullOrEmptyThrowsArgumentException(this string objectInstance, string message = null)
        {
            if (string.IsNullOrWhiteSpace(objectInstance))
            {
                throw new ArgumentException(message ?? nameof(objectInstance));
            }
            return objectInstance;
        }

        public static string ValidateIsStringNullOrEmptyThrowsArgumentNullException(this string objectInstance, string message = null)
        {
            if (string.IsNullOrWhiteSpace(objectInstance))
            {
                throw new ArgumentNullException(message ?? nameof(objectInstance));
            }
            return objectInstance;
        }
    }
}