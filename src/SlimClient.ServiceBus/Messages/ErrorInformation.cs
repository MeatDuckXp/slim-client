using System;

namespace SlimClient.ServiceBus.Azure.Messages
{
    public class ErrorInformation
    {
        public DateTime DateTime { get; set; }

        public string Reason { get; set; }

        public string Source { get; set; } 
    }
}