﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces.Requests
{
    public class ListDirRequest : NewCredentialRequest
    {
        public string Path { get; set; }
    }
}