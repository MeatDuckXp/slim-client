using System;

namespace SlimClient.ServiceBus.Azure.Messages
{
    public class RetryReason
    {
        public RetryReason(string reason, string activity)
        {
            DateTime = DateTime.UtcNow;
            Reason = reason;
            Activity = activity;
        }

        public DateTime DateTime { get; set; }

        public string Reason { get; set; }

        public string Activity { get; set; }
    }
}