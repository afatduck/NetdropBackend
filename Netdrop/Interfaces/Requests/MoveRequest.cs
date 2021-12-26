using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Requests
{
    public class MoveRequest : BaseFtpRequest
    {
        public string Destination { get; set; }
        public string Source { get; set; }
    }
}
