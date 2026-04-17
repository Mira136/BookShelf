using System.Net;
using System.Net.Mail;

namespace BookShelf.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var email = _config["EmailSettings:Email"];
            var password = _config["EmailSettings:Password"];

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };

            var message = new MailMessage(email, toEmail, subject, body);
            message.IsBodyHtml = true;

            smtp.Send(message);
        }
    }
}