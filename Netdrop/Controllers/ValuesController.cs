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
    public class NetdropController : ControllerBase
    {

        public static Dictionary<string, long> downloadProgress = new Dictionary<string, long>();

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



        [HttpPost("listdir")]
        public string PostListdir([FromBody] Dictionary<string, string> data)
        {

            try
            {
                using (Ftp client = new Ftp())
                {
                    client.Connect(data["host"]);
                    client.Login(data["user"], data["pword"]);

                    List<FtpItem> items = client.GetList(data.ContainsKey("path") ? data["path"] : "");
                    List<Dictionary<string, string>> ordered = new List<Dictionary<string, string>>();

                    foreach (FtpItem item in items)
                    {
                        Dictionary<string, string> toAdd = new Dictionary<string, string>();
                        toAdd.Add("name", item.Name);
                        toAdd.Add("modify", item.ModifyDate.ToString("MM/dd/yyyy H:mm"));
                        toAdd.Add("size", item.Size.ToString());
                        toAdd.Add("type", item.IsFolder ? "dir" : "file");
                        ordered.Add(toAdd);
                    }

                    return  JsonSerializer.Serialize(ordered);
                }
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(ex.Message);
            }
        }


        [HttpPost("download")]
        public string PostDownload([FromBody]Dictionary<string, string> data)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();

            try
            {

                string filename = data["path"].Substring(data["path"].LastIndexOf('/')+1);
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

        [HttpPost("progress")]
        public string PostProgress([FromBody] string file)
        {
            return JsonSerializer.Serialize(downloadProgress.ContainsKey(file) ? downloadProgress[file] : 0);
        }

        static async void DownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            byte[] raw = e.Result;
            using (var fs = new FileStream((string)e.UserState, FileMode.Create, FileAccess.Write))
            {
                await fs.WriteAsync(raw, 0, raw.Length);
            }
        }

        protected void DownloadChange(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress[(string)e.UserState] = e.BytesReceived;
        }

    }
}
