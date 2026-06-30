using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;

namespace SilentMoon.Domain.Entities
{
    public class Otp : BaseEntity
    {
        public int UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string Code { get; set; }

        public DateTime ExpireDate { get; set; }

        public bool IsUsed { get; set; }
    }
}