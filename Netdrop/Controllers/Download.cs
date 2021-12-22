using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using MimeMapping;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public static Dictionary<string, bool> DownloadComplete = new Dictionary<string, bool>();

        [HttpPost("download")]
        public async Task<IActionResult> PostDownload([FromBody] BaseFtpRequest data)
        {

            try
            {

                string filename = data.Path.Substring(data.Path.LastIndexOf('/') + 1);
                filename = "tmp/" + DateTime.Now.ToString("hhmmssfffffff") + filename;

                DownloadComplete[filename] = false;

                Action<FtpProgress> progress = delegate (FtpProgress p){
                    DownloadProgress[filename] = (short)p.Progress;
                    DownloadSpeed[filename] = p.TransferSpeed;
                };

                FtpClient client = GetFtpClient(data);
                await client.ConnectAsync();

                long total = 0;

                foreach (FtpListItem item in await client.GetListingAsync(data.Path, FtpListOption.Recursive))
                {
                    total += item.Size;
                }

                Console.WriteLine(total);

                bool isFolder = await client.DirectoryExistsAsync(data.Path);

                Task.Run(() =>
                {
                    try
                    {
                        if (isFolder)
                        {
                            client.DownloadDirectory(filename, data.Path, FtpFolderSyncMode.Mirror, FtpLocalExists.Skip, FtpVerify.None, null, progress);
                            ZipFile.CreateFromDirectory(filename, filename + ".zip");
                        }
                        else
                        {
                            client.DownloadFile(filename, data.Path, FtpLocalExists.Resume, FtpVerify.OnlyChecksum, progress);
                        }

                        DownloadComplete[filename] = true;
                    }
                    catch (FtpException ex) 
                    {
                        DownloadProgress[filename] = -1;
                        DownloadError[filename] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    }
                    
                    client.Dispose();
                });

                return Ok(new DownloadResponse()
                {
                    Result = true,
                    Url = filename,
                    Mime = isFolder ? "application/zip" : MimeUtility.GetMimeMapping(filename)
                });

            }
            catch (FtpException ex)
            {

                return Ok(new DownloadResponse()
                {
                    Result = false,
                    Errors = new List<string>() { ex.InnerException != null ? ex.InnerException.Message : ex.Message }
                });

            }

        }

        [HttpPost("downloadprogress")]
        public IActionResult PostProgress([FromBody] string file)
        {
            short progress = DownloadProgress.ContainsKey(file) ? DownloadProgress[file] : (short)0;
            double speed = DownloadSpeed.ContainsKey(file) ? DownloadSpeed[file] : 0;
            string error = DownloadError.ContainsKey(file) ? DownloadError[file] : String.Empty;
            bool complete = DownloadComplete.ContainsKey(file) ? DownloadComplete[file] : false;

            return Ok(new ProgressResponse()
            {
                Result = progress != -1,
                Done = progress,
                Speed = speed,
                Complete = complete,
                Errors = new List<string>() { error }

            });
        }
    }

}
