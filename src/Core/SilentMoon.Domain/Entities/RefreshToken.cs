using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilentMoon.Domain.Entities
{
    [Table("RefreshTokens")]
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public int ApplicationUserId { get; set; }
    }
}