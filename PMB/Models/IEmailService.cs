namespace PMB.Models
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
