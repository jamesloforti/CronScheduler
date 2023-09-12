namespace WebServiceDefault.Common.Settings
{
    public class AuthSettings
    {
        public string AuthUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int HttpClientTimeoutMinutes { get; set; }
        public int PollyMaxRetries { get; set; }
        public string Password { get; set; }
        public string SecurityToken { get; set; }
        public string Username { get; set; }
    }
}
