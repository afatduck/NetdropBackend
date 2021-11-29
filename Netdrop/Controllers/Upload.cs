using FluentFTP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        private static Dictionary<string, short> UploadProgress = new Dictionary<string, short>();
        private static Dictionary<string, string> UploadError = new Dictionary<string, string>();
        private static Dictionary<string, double> UploadSpeed = new Dictionary<string, double>();

        [HttpPost("upload")]
        [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
        public async Task<IActionResult> PostUpload([FromForm] IList<IFormFile> files, [FromForm] string dataJson)
        {
            try
            {
                BaseFtpRequest data = JsonSerializer.Deserialize<BaseFtpRequest>(dataJson);
                List<string> remoteFiles = new List<string>();
                List<string> localFiles = new List<string>();
                List<long> fileSizes = new List<long>();

                string code = DateTime.Now.ToString("hhmmssfffffff");
                long soFar = 0;

                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
                    {
                        remoteFiles.Add(data.Path + '/' + file.FileName);

                        string filePath = Path.Combine("tmp", DateTime.Now.ToString("hhmmssfffffff") + file.FileName.Substring(file.FileName.LastIndexOf('/') + 1));
                        localFiles.Add(filePath);

                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        fileSizes.Add(new FileInfo(filePath).Length);

                    }
                }

                FtpClient client = GetFtpClient(data);
                await client.ConnectAsync();

                Task.Run(() => {

                    for (short i = 0; i < remoteFiles.Count; i++)
                    {

                        if (i != 0) { soFar += fileSizes[i-1]; }

                        Action<FtpProgress> progress = delegate (FtpProgress p) {
                            UploadProgress[code] = (short)((soFar + p.Progress * fileSizes[i] / 100) / fileSizes.Sum() * 100);
                            if (p.TransferSpeed != 0)
                            {
                                UploadSpeed[code] = p.TransferSpeed;
                            }
                        };           
                        
                            try
                            {
                                var ftpStatus = client.UploadFile(localFiles[i], remoteFiles[i], FtpRemoteExists.Overwrite, true, FtpVerify.None, progress);
                                if (i == localFiles.Count - 1) { client.Dispose(); }
                            }
                            catch (FtpException ex)
                            {
                                UploadProgress[code] = -1;
                                UploadError[code] = ex.InnerException.Message;
                            }

                    }

                });

                return Ok(new UploadResponse()
                {
                    Result = true,
                    Code = code
                });

            }
            catch (FtpException ex)
            {
                return Ok(new UploadResponse() {
                    Result = false,
                    Errors = new List<string>() { ex.Message }
                });
            }

        }

        [HttpPost("uploadprogress")]
        public IActionResult PostCheckUpload([FromBody] string code)
        {
            short progress = UploadProgress.ContainsKey(code) ? UploadProgress[code] : (short)0;
            double speed = UploadSpeed.ContainsKey(code) ? UploadSpeed[code] : 0;
            string error = UploadError.ContainsKey(code) ? UploadError[code] : string.Empty;

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
