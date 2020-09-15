namespace SlimClient.ServiceBus.Azure.Messages
{
    /// <summary>
    /// https://www.ietf.org/rfc/rfc2045.txt
    /// https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Complete_list_of_MIME_types
    /// </summary>
    public class MimeContentTypes
    {
        public const string Xml = "application/xml";

        public const string Json = "application/json";

        public const string Text = "text/plain";
    }
}