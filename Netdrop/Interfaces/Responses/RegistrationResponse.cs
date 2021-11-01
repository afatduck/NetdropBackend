using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Responses
{
    public class RegistrationResponse : ResultBase
    {
        public UserData UserData { get; set; }
    }
}
