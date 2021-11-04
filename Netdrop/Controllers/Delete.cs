using Limilabs.FTP.Client;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
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
        public IActionResult PostDelete([FromBody] FtpRequestWithPath data)
        {
            try
            {
                using (Ftp client = new Ftp())
                {
                    client.Connect(data.Host);
                    client.Login(data.Username, data.Password);
                    if (client.FolderExists(data.Path)) {
                        client.DeleteFolderRecursively(data.Path);
                    }
                    else { client.DeleteFile(data.Path); }
                }
            }
            catch (FtpException ex)
            {
                return Ok(new ResultBase() { 
                    Errors = new List<string>() { ex.Message },
                    Result = false
                });
            }
            return Ok(new ResultBase() { Result = true });
        }
    }
}
