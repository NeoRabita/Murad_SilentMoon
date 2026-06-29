using Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.Login
{
    public class LoginCommand : ICommand<LoginResponse>
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
