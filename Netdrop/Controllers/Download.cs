using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{

    public partial class NetdropController : ControllerBase
    {

        [HttpPost("download")]
        public IActionResult PostDownload([FromBody] BaseFtpRequest data)
        {

            try
            {

                string filename = data.Host.Substring(data.Host.LastIndexOf('/') + 1);
                filename = "tmp/" + DateTime.Now.ToString("hhmmssfffffff") + filename;

                Uri uri = new Uri($"ftp{(data.Secure ? "s" : "")}://{data.Host}:{data.Port}{data.Path}");

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(data.Username, data.Password);
                    client.DownloadDataCompleted += DownloadComplete;
                    client.DownloadProgressChanged += DownloadChange;
                    client.DownloadDataAsync(uri, filename);
                }

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create($"ftp{(data.Secure ? "s" : "")}://{data.Host}:{data.Port}{data.Path}");
                req.Credentials = new NetworkCredential(data.Username, data.Password);
                req.EnableSsl = data.Secure;
                req.Method = WebRequestMethods.Ftp.GetFileSize;

                return Ok(new DownloadResponse()
                {
                    Result = true,
                    Url = filename,
                    Size = req.GetResponse().ContentLength,
                    Mime = MimeUtility.GetMimeMapping(filename)
                });

            }
            catch (Exception ex)
            {

                Ok(new DownloadResponse()
                {
                    Result = false,
                    Errors = new List<string>() { ex.Message }
                });

            }

            return BadRequest();

        }
    }

}
