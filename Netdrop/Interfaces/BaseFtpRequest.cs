using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces
{
    public class BaseFtpRequest
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Secure { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public bool Save { get; set; }
    }
}
