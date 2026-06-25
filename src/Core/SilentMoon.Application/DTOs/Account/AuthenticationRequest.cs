using System;
using System.ComponentModel.DataAnnotations;

namespace SilentMoon.Application.DTOs.Account
{
    public class AuthenticationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}