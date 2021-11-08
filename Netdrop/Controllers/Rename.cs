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
        [HttpPost("rename")]
        public async Task<IActionResult> PostRename([FromBody] RenameRequest data)
        {
            try
            {

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create($"ftp://{data.Host}:{data.Port}{data.Path}");
                req.Credentials = new NetworkCredential(data.Username, data.Password);
                req.Method = WebRequestMethods.Ftp.Rename;
                req.RenameTo = data.Path.Substring(0, data.Path.LastIndexOf('/') + 1) + data.Name;
                await req.GetResponseAsync();

            }
            catch (WebException ex)
            {

                return Ok(new ResultBase() 
                { 
                    Result = false,
                    Errors = new List<string>() { ex.Message }
                });
            }
            return Ok(new ResultBase() { Result = true });
        }
    }
}
