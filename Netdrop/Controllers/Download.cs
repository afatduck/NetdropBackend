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
        public static Dictionary<string, List<long>> DownloadDirectory = new Dictionary<string, List<long>>();

        [HttpPost("download")]
        public async Task<IActionResult> PostDownload([FromBody] BaseFtpRequest data)
        {

            try
            {

                string filename = data.Path.Substring(data.Path.LastIndexOf('/') + 1);
                filename = "tmp/" + DateTime.Now.ToString("hhmmssfffffff") + filename.Replace(".", String.Empty);

                DownloadComplete[filename] = false;

                FtpClient client = GetFtpClient(data);
                bool isFolder = await client.DirectoryExistsAsync(data.Path);
                long totalToDownload = 1;

                if (isFolder) 
                {
                    totalToDownload = 0;
                    DownloadDirectory[filename] = new List<long>() { 0 };

                    foreach (FtpListItem item in await client.GetListingAsync(data.Path, FtpListOption.Recursive))
                    {
                        totalToDownload += item.Size;
                        DownloadDirectory[filename].Add(totalToDownload);
                    }
                }

                Action<FtpProgress> progress = delegate (FtpProgress p) {

                    DownloadSpeed[filename] = p.TransferSpeed;

                    if (!isFolder)
                    {
                        DownloadProgress[filename] = (short)p.Progress;
                        
                        return;
                    }

                    DownloadProgress[filename] = (short)((double)(DownloadDirectory[filename][p.FileIndex] + p.TransferredBytes) / totalToDownload * 100);

                    
                };

                Task.Run(() =>
                {
                    try
                    {
                        if (isFolder)
                        {
                            client.DownloadDirectory(filename, data.Path, FtpFolderSyncMode.Mirror, FtpLocalExists.Skip, FtpVerify.Retry, null, progress);
                            ZipFile.CreateFromDirectory(filename, filename + ".zip");
                        }
                        else
                        {
                            client.DownloadFile(filename, data.Path, FtpLocalExists.Resume, FtpVerify.Retry, progress);
                        }

                        DownloadComplete[filename] = true;
                    }
                    catch (FtpException ex) 
                    {
                        DownloadProgress[filename] = -1;
                        DownloadError[filename] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    }

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
