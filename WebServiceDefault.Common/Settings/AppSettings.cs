namespace WebServiceDefault.Common.Settings
{
    public class AppSettings
    {
        public string ApplicationName { get; set; }
        public string Environment { get; set; }
        public int HttpClientTimeoutMinutes { get; set; }
        public int PollyMaxRetries { get; set; }
        public string SwaggerJsonUrl { get; set; }
        public string DownstreamServiceUrl { get; set; }
    }
}
