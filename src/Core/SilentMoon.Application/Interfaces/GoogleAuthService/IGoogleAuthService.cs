using SilentMoon.Application.DTOs.GoogleAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.GoogleAuthService
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfo?> ValidateToken(string token);
    }
}
