﻿using Data.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CertificatesWebApp.Users.Services
{
    public interface ISMSService
    {
        public Task SendActivationSMS(User user, int code);
        public Task SendPasswordResetSMS(User user, int code);
        public Task SendTwoFactorSMS(User user, int code);

    }

    public class SMSService : ISMSService
    {
        private readonly ILogger<SMSService> _logger;
        public SMSService(ILogger<SMSService> logger) 
        {
            _logger = logger;
        }

        public async Task SendActivationSMS(User user, int code)
        {
            StreamReader sr = new StreamReader("twilio_credentials.txt");
            var accountSid = sr.ReadLine();
            var authToken = sr.ReadLine();
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(user.Telephone));
            messageOptions.From = new PhoneNumber("+12707479566");
            messageOptions.Body = "Click here to activate your account: " + "http://localhost:5173/activateAccount?code=" + code;

            var message = await MessageResource.CreateAsync(messageOptions);
            if (message.Status == MessageResource.StatusEnum.Queued) {
                _logger.LogError("Activation SMS sent to user {@Id} successfully", user.Id);
            }
        }

        public async Task SendPasswordResetSMS(User user, int code)
        {
            StreamReader sr = new StreamReader("twilio_credentials.txt");
            var accountSid = sr.ReadLine();
            var authToken = sr.ReadLine();
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(user.Telephone));
            messageOptions.From = new PhoneNumber("+12707479566");
            messageOptions.Body = "Click here to reset your password: " + "http://localhost:5173/passwordReset?code=" + code;

            var message = await MessageResource.CreateAsync(messageOptions);
            if (message.Status == MessageResource.StatusEnum.Queued)
            {
                _logger.LogError("Password reset SMS sent to user {@Id} successfully", user.Id);
            }
        }

        public async Task SendTwoFactorSMS(User user, int code)
        {
            StreamReader sr = new StreamReader("twilio_credentials.txt");
            var accountSid = sr.ReadLine();
            var authToken = sr.ReadLine();
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(user.Telephone));
            messageOptions.From = new PhoneNumber("+12707479566");
            messageOptions.Body = "Your two factor authentication code is: " + code;

            var message = await MessageResource.CreateAsync(messageOptions);
            if (message.Status == MessageResource.StatusEnum.Queued)
            {
                _logger.LogError("Two factor verification SMS sent to user {@Id} successfully", user.Id);
            }
        }
    }
}
