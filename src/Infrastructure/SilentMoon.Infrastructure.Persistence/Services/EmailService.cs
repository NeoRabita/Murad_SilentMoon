using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class EmailService : IEmailService
    {
        public APIAppSettings _apiSettings;

        public EmailService(IOptions<APIAppSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        public async Task SendAsync(EmailRequest request)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(request.From ?? _apiSettings.MailSettings.EmailFrom, _apiSettings.MailSettings.DisplayName);
                mail.To.Add(request.To);
                mail.Subject = request.Subject;
                var htmlView = AlternateView.CreateAlternateViewFromString(request.Body, null, "text/html");
                mail.IsBodyHtml = true;
                mail.AlternateViews.Add(htmlView);
                using (var smtpClient = new SmtpClient(_apiSettings.MailSettings.SmtpHost, _apiSettings.MailSettings.SmtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_apiSettings.MailSettings.SmtpUser, _apiSettings.MailSettings.SmtpPass);
                    smtpClient.EnableSsl = _apiSettings.MailSettings.SSL;
                    await smtpClient.SendMailAsync(mail);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task SendOtpEmailAsync(
    string email,
    string name,
    string otp)
        {
            await SendAsync(new EmailRequest
            {
                To = email,

                Subject = "Confirm your account",

                Body = $@"
            <h2>Welcome {name}</h2>

            <p>Your verification code:</p>

            <h1>{otp}</h1>

            <p>Expires in 10 minutes.</p>
        "
            });
        }
    }
}