using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
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
        [HttpPost("createdir")]
        public async Task<IActionResult> PostCreateDir([FromBody] BaseFtpRequest data)
        {
            try
            {

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://" + data.Host);
                req.Credentials = new NetworkCredential(data.Username, data.Password);
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                await req.GetResponseAsync();

            }
            catch (WebException ex)
            {
                return Ok(new ResultBase() { 
                    Result = false,
                    Errors = new List<string>() { ex.Message }
                });
            }
            return Ok(new ResultBase()
            {
                Result = true
            });
        }
    }
}
