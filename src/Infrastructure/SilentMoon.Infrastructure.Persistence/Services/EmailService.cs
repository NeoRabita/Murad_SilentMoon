using Microsoft.Extensions.Options;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using SilentMoon.SharedKernel.Resources;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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

        private static string LoadTemplate(string fileName)
        {
            var assembly = typeof(Messages).Assembly;
            var resourceName = $"SilentMoon.SharedKernel.Resources.Emails.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Email template not found: {resourceName}");

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static string BuildEmailHtml(string contentHtml)
        {
            return LoadTemplate("_Layout.html")
                .Replace("{{CONTENT}}", contentHtml)
                .Replace("{{YEAR}}", DateTime.UtcNow.Year.ToString());
        }

        public async Task SendOtpEmailAsync(string email, string name, string otp)
        {
            var content = LoadTemplate("OtpVerification.html")
                .Replace("{{NAME}}", name)
                .Replace("{{OTP}}", otp);

            await SendAsync(new EmailRequest
            {
                To = email,

                Subject = "SilentMoon — Hesab təsdiqi",

                Body = BuildEmailHtml(content)
            });
        }

        public async Task SendReminderEmailAsync(string email, string name, TimeSpan time)
        {
            var content = LoadTemplate("Reminder.html")
                .Replace("{{NAME}}", name)
                .Replace("{{TIME}}", time.ToString(@"hh\:mm"));

            await SendAsync(new EmailRequest
            {
                To = email,

                Subject = "SilentMoon — Xatırlatma",

                Body = BuildEmailHtml(content)
            });
        }

        public async Task SendForgotPasswordEmailAsync(string email, string name, string otp)
        {
            var content = LoadTemplate("ForgotPassword.html")
                .Replace("{{NAME}}", name)
                .Replace("{{OTP}}", otp);

            await SendAsync(new EmailRequest
            {
                To = email,

                Subject = "SilentMoon — Şifrə sıfırlama",

                Body = BuildEmailHtml(content)
            });
        }
    }
}
