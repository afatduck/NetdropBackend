using Microsoft.AspNetCore.Mvc;
using MimeMapping;
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
        public string PostDownload([FromBody] Dictionary<string, string> data)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();

            try
            {

                string filename = data["path"].Substring(data["path"].LastIndexOf('/') + 1);
                filename = "tmp/" + DateTime.Now.ToString("mmfffffff") + filename;

                Uri uri = new Uri("ftp://" + data["path"]);

                using (WebClient req = new WebClient())
                {
                    req.Credentials = new NetworkCredential(data["user"], data["pword"]);
                    req.DownloadDataCompleted += DownloadComplete;
                    req.DownloadProgressChanged += DownloadChange;
                    req.DownloadDataAsync(uri, filename);
                }

                FtpWebRequest req2 = (FtpWebRequest)WebRequest.Create(uri);
                req2.Credentials = new NetworkCredential(data["user"], data["pword"]);
                req2.Method = WebRequestMethods.Ftp.GetFileSize;

                response["url"] = filename;
                response["size"] = req2.GetResponse().ContentLength.ToString();
                response["mime"] = MimeUtility.GetMimeMapping(filename);

                return JsonSerializer.Serialize(response);


            }
            catch (WebException ex)
            {

                response["error"] = ex.Message;
                return JsonSerializer.Serialize(response);

            }
        }
    }
}
