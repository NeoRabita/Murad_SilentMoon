using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.VerifyEmail
{
    public class VerifyEmailCommand : ICommand
    {
        public string Email { get; set; }

        public string Code { get; set; }
    }
}
