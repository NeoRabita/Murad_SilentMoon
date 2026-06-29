using SilentMoon.Domain.Common;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
