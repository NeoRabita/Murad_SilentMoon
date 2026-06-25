using System;

namespace SilentMoon.Application.DTOs.JWT
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public DateTimeOffset Expires { get; set; }

        public JwtTokenDto(string token, DateTimeOffset expires)
        {
            Token = token;
            Expires = expires;
        }
    }
}