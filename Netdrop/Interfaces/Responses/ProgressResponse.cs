using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Responses
{
    public class ProgressResponse : ResultBase
    {
        public long Done { get; set; }
        public double Speed { get; set; }

        public bool Complete { get; set; }

    }
}
