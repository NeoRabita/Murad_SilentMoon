using SilentMoon.Domain.Common;
using System;
using System.Collections.Generic;

namespace SilentMoon.Domain.Entities
{
    namespace SilentMoon.Domain.Entities
    {
        public class ApplicationUser : BaseEntity
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string UserName { get; set; }

            public string Email { get; set; }

            public string PasswordHash { get; set; }

            public bool IsEmailConfirmed { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }

            public List<Otp> Otps { get; set; }


            public RefreshToken RefreshToken { get; set; }
        }
    }

}
