namespace SlimClient.ServiceBus.Azure.Configuration
{
    public class ServiceBusConfiguration
    {
        public string ServiceBusConnectionString { get; set; }

        public bool VerboseResourceDetails { get; set; } = false;
    }
}
