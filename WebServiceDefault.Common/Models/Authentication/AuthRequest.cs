namespace WebServiceDefault.Common.Models.Authentication
{
    public class AuthRequest
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string username { get; set; }

        public string password { get; set; }
    }
}
