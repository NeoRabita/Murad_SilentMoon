using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace SilentMoon.Application.Features.Accounts.Commands.ResendOtp
{
    public class ResendOtpCommand : ICommand
    {
        public string Email { get; set; }
    }
}
