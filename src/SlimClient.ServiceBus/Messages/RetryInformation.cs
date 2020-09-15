using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SlimClient.ServiceBus.Azure.Messages
{
    /// <summary>
    ///     Container that holds data required to track the message processing retries
    /// </summary>
    public class RetryInformation<T>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public RetryInformation(ushort maxNumberOfRetries, List<T> retryDestinations)
        {
            Reason = new List<RetryReason>();
            RetryDestinations = retryDestinations;
            MaxNumberOfRetries = maxNumberOfRetries;
        }

        /// <summary>
        ///     Gets or Sets list of reasons that the retries were attempted
        /// </summary>
        public List<RetryReason> Reason { get; set; }

        /// <summary>
        ///     Gets or Sets MaxNumberOfRetries
        /// </summary>
        public ushort MaxNumberOfRetries { get; }

        /// <summary>
        ///     Gets or Sets CurrentRetryCount
        /// </summary>
        [JsonProperty]
        public ushort CurrentRetryCount { get; private set; }

        /// <summary>
        ///     Evaluates the current state and if the number of retries has been exceeded, returns false.
        /// </summary>
        public bool HasExceededMaxCount => CurrentRetryCount >= MaxNumberOfRetries;

        /// <summary>
        ///     Gets list of Retry Destinations
        /// </summary>
        public List<T> RetryDestinations { get; }

        /// <summary>
        ///     Has Retry Destinations fact.
        /// </summary>
        /// <returns>True if has retry destinations, else false.</returns>
        public bool HasRetryDestinations => RetryDestinations != null && RetryDestinations.Any();

        /// <summary>
        ///     Updates retry information
        /// </summary>
        public void Update()
        {
            if (CurrentRetryCount < MaxNumberOfRetries)
            {
                ++CurrentRetryCount;
            }
        }

        /// <summary>
        ///     Clear retry destinations.
        /// </summary>
        public void ClearDestinations()
        {
            RetryDestinations.Clear();
        }

        /// <summary>
        ///     Add retry destinations.
        /// </summary>
        /// <param name="retryDestinations">Retry destinations.</param>
        public void AddDestinations(IEnumerable<T> retryDestinations)
        {
            RetryDestinations.AddRange(retryDestinations);
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"Max number of retries: {MaxNumberOfRetries}. Current retry count: {CurrentRetryCount}";
        }
    }
}