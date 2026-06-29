using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.User.Commands.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.GoogleLogin
{
    public class GoogleLoginCommand : ICommand<LoginResponse>
    {
        public string IdToken { get; set; }
    }
}
