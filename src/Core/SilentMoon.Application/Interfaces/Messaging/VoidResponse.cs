using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Messaging
{
    public class VoidResponse
    {
        public static VoidResponse Value { get; } = new VoidResponse();
    }

}
