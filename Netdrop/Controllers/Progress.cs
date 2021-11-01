using Microsoft.AspNetCore.Mvc;
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
        public string PostProgress([FromBody] string file)
        {
            return JsonSerializer.Serialize(downloadProgress.ContainsKey(file) ? downloadProgress[file] : 0);
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
            catch (TargetInvocationException) {}
        }

        protected void DownloadChange(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress[(string)e.UserState] = e.BytesReceived - 1;
        }
    }
}
