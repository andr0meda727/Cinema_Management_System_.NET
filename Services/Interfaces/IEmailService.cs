using System.Net.Mail;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailWithAttachmentsAsync(string toEmail, string subject, string body, List<Attachment> attachments);
    }
}
