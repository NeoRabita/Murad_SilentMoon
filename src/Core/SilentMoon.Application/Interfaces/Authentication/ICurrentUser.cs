using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Authentication
{
    public interface ICurrentUser
    {
        int UserId { get; }

        string UserName { get; }
    }
}
