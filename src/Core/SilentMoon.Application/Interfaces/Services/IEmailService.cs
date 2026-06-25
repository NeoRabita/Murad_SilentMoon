using System;
using System.Threading.Tasks;
using SilentMoon.Application.DTOs.Email;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}