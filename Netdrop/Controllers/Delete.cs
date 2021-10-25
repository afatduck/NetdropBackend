using Limilabs.FTP.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpPost("delete")]
        public string PostDelete([FromBody] Dictionary<string, string> data)
        {
            try
            {
                using (Ftp client = new Ftp())
                {
                    client.Connect(data["host"]);
                    client.Login(data["user"], data["pword"]);
                    if (client.FolderExists(data["path"])) {
                        client.DeleteFolderRecursively(data["path"]);
                    }
                    else { client.DeleteFile(data["path"]); }
                }
            }
            catch (FtpException ex)
            {
                return ex.Message;
            }
            return "";
        }
    }
}
