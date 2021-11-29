using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Responses
{
    public class DirList
    {
        public string Name { get; set; }
        public string Modify { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Mime { get; set; }
    }
    public class ListDirResponse : ResultBase
    {
        public List<DirList> DirList { get; set; }
    }
}
