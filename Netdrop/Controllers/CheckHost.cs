using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Limilabs.FTP.Client;
using MimeMapping;

namespace Netdrop.Controllers
{

    [Route("api")]
    [ApiController]
    public partial class NetdropController : ControllerBase
    {

        [HttpPost("host")]
        public async Task<ActionResult<string>> PostHost([FromBody]string hostname)
        {
            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://" + hostname);
                req.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
                await req.GetResponseAsync();
            }
            catch (WebException ex)
            {

                return ex.Message == "The remote name could not be resolved" ? "Cannot connect to the host." : "";

            } catch ( Exception ex ) { return ex.Message; }

            return "";
        }

    }
}
