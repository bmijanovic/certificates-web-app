using Data.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CertificatesWebApp.Users.Services
{
    public interface IMailService
    {
        public Task SendActivationMail(User user, int code);
        public Task SendPasswordResetMail(User user, int code);
        public Task SendTwoFactorMail(User user, int code);
       
    }

    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        public MailService(ILogger<MailService> logger)
        {
            _logger = logger;
        }

        public async Task SendActivationMail(User user, int code) {
            StreamReader sr = new StreamReader("sendgrid_api_key.txt");
            String sendgridApiKey = sr.ReadLine();
            SendGridClient client = new SendGridClient(sendgridApiKey);
            SendGridMessage msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("certificateswebapp@gmail.com", "Certificates Web app"));
            msg.AddTo(new EmailAddress(user.Email, String.Concat(user.Name, " ", user.Surname)));
            msg.Subject = "Activation mail";
            msg.SetTemplateId("d-3527be23b08d4f0582cd87fe0a00314e");

            var dynamicTemplateData = new
            {
                url_page = "http://localhost:5173/activateAccount?code=" + code,
                user_name = user.Name
            };

            msg.SetTemplateData(dynamicTemplateData);
            Response response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Activation mail not sent to user {@Id} due to mail server error", user.Id);
                throw new Exception("Mail error");
            }
            _logger.LogError("Activation mail sent to user {@Id} successfully", user.Id);
        }

        public async Task SendPasswordResetMail(User user, int code) {
            StreamReader sr = new StreamReader("sendgrid_api_key.txt");
            String sendgridApiKey = sr.ReadLine();
            SendGridClient client = new SendGridClient(sendgridApiKey);
            SendGridMessage msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("certificateswebapp@gmail.com", "Certificates Web app"));
            msg.AddTo(new EmailAddress(user.Email, String.Concat(user.Name, " ", user.Surname)));
            msg.Subject = "Password reset request";
            msg.SetTemplateId("d-a7ce9baeb73e4846bbefa17fc38cdeec");

            var dynamicTemplateData = new
            {
                url_page = "http://localhost:5173/passwordReset?code=" + code,
                user_name = user.Name
            };

            msg.SetTemplateData(dynamicTemplateData);
            Response response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Password reset mail not sent to user {@Id} due to mail server error", user.Id);
                throw new Exception("Mail error");
            }
            _logger.LogError("Password reset mail sent to user {@Id} successfully", user.Id);
        }

        public async Task SendTwoFactorMail(User user, int code)
        {
            StreamReader sr = new StreamReader("sendgrid_api_key.txt");
            String sendgridApiKey = sr.ReadLine();
            SendGridClient client = new SendGridClient(sendgridApiKey);
            SendGridMessage msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("certificateswebapp@gmail.com", "Certificates Web app"));
            msg.AddTo(new EmailAddress(user.Email, String.Concat(user.Name, " ", user.Surname)));
            msg.Subject = "Two factor authentication";
            msg.SetTemplateId("d-225b1c4cf39244eaa2f7e302e226be68");

            var dynamicTemplateData = new
            {
                user_name = user.Name,
                url_page = "http://localhost:5173/twoFactor",
                code = code
            };

            msg.SetTemplateData(dynamicTemplateData);
            Response response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Two factor verification not sent to user {@Id} due to mail server error", user.Id);
                throw new Exception("Mail error");
            }
            _logger.LogError("Two factor verification mail sent to user {@Id} successfully", user.Id);
        }
    }
}
