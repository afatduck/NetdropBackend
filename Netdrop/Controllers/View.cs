using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{

    public partial class NetdropController : ControllerBase
    {

        [HttpPost("view")]
        public async Task<IActionResult> PostView([FromBody] BaseFtpRequest data)
        {

            try
            {

                string filename = data.Path.Substring(data.Path.LastIndexOf('/') + 1);
                filename = "tmp/" + DateTime.Now.ToString("hhmmssfffffff") + filename.Replace(".", String.Empty);

                FtpClient client = GetFtpClient(data);

                await client.DownloadFileAsync(filename, data.Path, FtpLocalExists.Resume, FtpVerify.None);
                if (!data.Save) client.Dispose();

                return Ok(new ViewResponse()
                {
                    Result = true,
                    Url = filename
                });

            }
            catch (FtpException ex)
            {

                return Ok(new ViewResponse()
                {
                    Result = false,
                    Errors = new List<string>() { ex.InnerException.Message }
                });

            }

        }

    }

}
