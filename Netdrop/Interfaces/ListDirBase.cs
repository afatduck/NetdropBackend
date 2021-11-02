using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces
{
    public class ListDirBase
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Secure { get; set; }
    }
}
