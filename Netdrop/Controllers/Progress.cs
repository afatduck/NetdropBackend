using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {

        public static Dictionary<string, long> downloadProgress = new Dictionary<string, long>();

        [HttpPost("progress")]
        public IActionResult PostProgress([FromBody] string file)
        {
            long progress = downloadProgress.ContainsKey(file) ? downloadProgress[file] : 0;
            return Ok(new ProgressResponse()
            {
                Result = progress != -1,
                Done = progress
            });
        }

        static async void DownloadComplete(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                byte[] raw = e.Result;
                using (var fs = new FileStream((string)e.UserState, FileMode.Create, FileAccess.Write))
                {
                    await fs.WriteAsync(raw, 0, raw.Length);
                }
                downloadProgress[(string)e.UserState]++;
            }
            catch (Exception) { downloadProgress[(string)e.UserState] = -1; }
        }

        protected void DownloadChange(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress[(string)e.UserState] = e.BytesReceived - 1;
        }
    }
}
