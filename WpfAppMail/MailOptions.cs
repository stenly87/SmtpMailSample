
using System.Security;

namespace WpfAppMail
{
    public class MailOptions
    {
        public bool EnableSsl { get; set; }
        public int SmtpPort { get; set; }
        public string? From { get; set; }
        public string? SmtpServer { get; set; }
        public string? Password { get; set; }
        public string? Footer { get; set; }
    }
}