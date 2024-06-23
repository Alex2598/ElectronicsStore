namespace Store.Web.Models
{
    public class EmailHelperOptions
    {
        public string Subject { get; set; }
        public string From { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
    }
}
