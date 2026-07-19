using System;
using System.Threading.Tasks;
using SilentMoon.Application.DTOs.Email;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);

        Task SendOtpEmailAsync(
      string email,
      string name,
      string otp);

        Task SendReminderEmailAsync(
      string email,
      string name,
      TimeSpan time);

        Task SendForgotPasswordEmailAsync(
      string email,
      string name,
      string otp);
    }
}