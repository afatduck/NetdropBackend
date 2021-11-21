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

        public static Dictionary<string, short> DownloadProgress = new Dictionary<string, short>();
        public static Dictionary<string, double> DownloadSpeed = new Dictionary<string, double>();
        public static Dictionary<string, string> DownloadError = new Dictionary<string, string>();

        [HttpPost("download")]
        public async Task<IActionResult> PostDownload([FromBody] BaseFtpRequest data)
        {

            try
            {

                string filename = data.Path.Substring(data.Path.LastIndexOf('/') + 1);
                filename = "tmp/" + DateTime.Now.ToString("hhmmssfffffff") + filename;

                Action<FtpProgress> progress = delegate (FtpProgress p){
                    DownloadProgress[filename] = (short)p.Progress;
                    DownloadSpeed[filename] = p.TransferSpeed;
                };

                FtpClient client = GetFtpClient(data);
                await client.ConnectAsync();

                Task.Run(() =>
                {
                    try
                    {
                        client.DownloadFile(filename, data.Path, FtpLocalExists.Resume, FtpVerify.None, progress);
                    }
                    catch (FtpException ex) 
                    {
                        DownloadProgress[filename] = -1;
                        DownloadError[filename] = ex.InnerException.Message;
                    }
                    
                    client.Dispose();
                });

                return Ok(new DownloadResponse()
                {
                    Result = true,
                    Url = filename,
                    Mime = MimeUtility.GetMimeMapping(filename)
                });

            }
            catch (FtpException ex)
            {

                return Ok(new DownloadResponse()
                {
                    Result = false,
                    Errors = new List<string>() { ex.InnerException.Message }
                });

            }

        }

        [HttpPost("downloadprogress")]
        public IActionResult PostProgress([FromBody] string file)
        {
            short progress = DownloadProgress.ContainsKey(file) ? DownloadProgress[file] : (short)0;
            double speed = DownloadSpeed.ContainsKey(file) ? DownloadSpeed[file] : 0;
            string error = DownloadError.ContainsKey(file) ? DownloadError[file] : String.Empty;

            return Ok(new ProgressResponse()
            {
                Result = progress != -1,
                Done = progress,
                Speed = speed,
                Errors = new List<string>() { error }

            });
        }
    }

}
