using Data.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace CertificatesWebApp.Users.Services
{
    public interface IMailService
    {
        public Task SendActivationMail(User user, int code);
        public Task SendPasswordResetMail(User user, int code);
       
    }

    public class MailService : IMailService
    {
        public MailService() { }

        public async Task SendActivationMail(User user, int code) {
            StreamReader sr = new StreamReader("sendgrid_api_key.txt");
            String sendgridApiKey = sr.ReadLine();
            SendGridClient client = new SendGridClient(sendgridApiKey);
            SendGridMessage msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("certificateswebapp@gmail.com", "Certificates Web app"));
            msg.AddTo(new EmailAddress(user.Email, user.Name + " " + user.Surname));
            msg.SetTemplateId("d-3527be23b08d4f0582cd87fe0a00314e");

            var dynamicTemplateData = new
            {
                url_page = "https://localhost:7018/api/User/activateAccount" + "?code=" + code,
                user_name = user.Name
            };

            msg.SetTemplateData(dynamicTemplateData);
            Response response = await client.SendEmailAsync(msg);
        }

        public async Task SendPasswordResetMail(User user, int code) {
            StreamReader sr = new StreamReader("sendgrid_api_key.txt");
            String sendgridApiKey = sr.ReadLine();
            SendGridClient client = new SendGridClient(sendgridApiKey);
            SendGridMessage msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("certificateswebapp@gmail.com", "Certificates Web app"));
            msg.AddTo(new EmailAddress(user.Email, user.Name + " " + user.Surname));
            msg.SetTemplateId("d-a7ce9baeb73e4846bbefa17fc38cdeec");

            var dynamicTemplateData = new
            {
                url_page = "https://localhost:7018/api/User/resetPassword" + "?code=" + code,
                user_name = user.Name
            };

            msg.SetTemplateData(dynamicTemplateData);
            Response response = await client.SendEmailAsync(msg);
        }
    }
}
