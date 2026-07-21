using SilentMoon.Application.Interfaces.Security;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        public bool Verify(string password,string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(
                password,
                passwordHash);
        }
    }
}

