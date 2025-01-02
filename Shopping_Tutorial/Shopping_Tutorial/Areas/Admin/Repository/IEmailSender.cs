namespace Shopping_Tutorial.Areas.Admin.Repository
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message); //ham` gui? email. SendEmailAsync(mail cần gửi, Tiêu đề, nội dung)
    }
}
