using Data.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace CertificatesWebApp.Users.Services
{
    public interface ISendGridService
    {
        public Task sendActivationMailAsync(User user, String code);
       
    }

    public class SendGridService : ISendGridService
    {
        public SendGridService() { }

        public async Task sendActivationMailAsync(User user, String code) {
            String apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            SendGridClient client = new SendGridClient(apiKey);
            EmailAddress from = new EmailAddress("test@example.com", "Example User");
            String subject = "Activation Mail";
            EmailAddress to = new EmailAddress(user.Email, user.Name + " " + user.Surname);
            String plainTextContent = "nz sta je ovo";
            String htmlContent = "<strong>" + code + "</strong>";
            SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            Response response = await client.SendEmailAsync(msg);
        }

    }
}
