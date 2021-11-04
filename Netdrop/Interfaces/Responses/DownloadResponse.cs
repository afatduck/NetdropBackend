using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Responses
{
    public class DownloadResponse : ResultBase
    {
        public string Url { get; set; }
        public string Mime { get; set; }
        public long Size { get; set; }
    }
}
