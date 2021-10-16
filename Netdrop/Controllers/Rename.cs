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
        [HttpPost("rename")]
        public async Task<ActionResult<string>> PostRename([FromBody] Dictionary<string, string> data)
        {
            try
            {

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://" + data["host"] + data["path"]);
                req.Credentials = new NetworkCredential(data["user"], data["pword"]);
                req.Method = WebRequestMethods.Ftp.Rename;
                req.RenameTo = data["path"].Substring(0, data["path"].LastIndexOf('/') + 1) + data["name"];
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
