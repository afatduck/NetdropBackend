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
        [HttpPost("createdir")]
        public async Task<ActionResult<string>> PostCreateDir([FromBody] Dictionary<string, string> data)
        {
            try
            {

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://" + data["host"]);
                req.Credentials = new NetworkCredential(data["user"], data["pword"]);
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                await req.GetResponseAsync();

            }
            catch (WebException ex)
            {

                return ex.Message;
            }
            return "";
        }
    }
}
