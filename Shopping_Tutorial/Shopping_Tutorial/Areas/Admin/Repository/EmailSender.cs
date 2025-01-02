using System.Net.Mail;
using System.Net;

namespace Shopping_Tutorial.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("lam422023@gmail.com", "tgxkedpzblfhbcbm")
            };

            return client.SendMailAsync(
                new MailMessage(from: "lam422023@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
