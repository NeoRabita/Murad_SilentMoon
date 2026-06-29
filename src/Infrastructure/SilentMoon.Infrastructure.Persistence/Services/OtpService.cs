using SilentMoon.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Services
{
    public class OtpService : IOtpService
    {
        public string Generate()
        {
            var random = new Random();

            return random.Next(100000, 999999)
                .ToString();
        }
    }
}
