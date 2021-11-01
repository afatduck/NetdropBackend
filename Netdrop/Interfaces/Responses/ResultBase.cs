using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Responses
{
    public class ResultBase
    {
        public List<string> Errors { get; set; }
        public bool Result { get; set; }
    }
}
