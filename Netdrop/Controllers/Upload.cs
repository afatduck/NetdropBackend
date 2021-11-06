using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        private static Dictionary<string, int> UploadProgress = new Dictionary<string, int>();

        [HttpPost("upload")]
        [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
        public async Task<IActionResult> PostUpload([FromForm] IList<IFormFile> files, [FromForm] string dataJson)
        {
            Dictionary<string, string> data = JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
            List<string> fileNames = new List<string>();
            List<string> localFileNames = new List<string>();
            HashSet<string> dirs = new HashSet<string>();

            foreach (IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine("tmp", DateTime.Now.ToString("hhmmssfffffff") + file.FileName);
                    if (file.FileName.Contains('/')) { dirs.Add(file.FileName.Remove(file.FileName.LastIndexOf('/'))); }
                    fileNames.Add(file.FileName);
                    localFileNames.Add(filePath);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
#pragma warning disable CS4014
                    }
                }
            }

            foreach (string dir in dirs)
            {
                Console.WriteLine(dir);
                if (! await CreateDir("ftp://" + data["host"] + '/' + dir, data["username"], data["password"]))
                {
                    return Ok(new UploadResponse()
                    {
                        Result = false,
                        Errors = new List<string>() { "Something went wrong. "}
                    });
                }
            }

            string code = DateTime.Now.ToString("hhmmssfffffff");
            UploadProgress[code] = 0;
            Task.Run(() => { UploadToFtp(fileNames, localFileNames, data, code); });

            return Ok(new UploadResponse() { 
                Result = true,
                Code = code
            });
        }

        private void UploadToFtp(List<string> filenames, List<string> localFilenames, Dictionary<string, string> data, string code)
        {
            long totalSize = 0L;
            long totalUploaded = 0L;

            foreach (string f in localFilenames.ToList())
            {
                totalSize += new FileInfo(f).Length;
            }

            for (int i = 0; i < filenames.Count; i++)
            {
                try
                {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + data["host"] + '/' + filenames[i]);
                request.Credentials = new NetworkCredential(data["user"], data["pword"]);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.ContentLength = new FileInfo(localFilenames[i]).Length;
                request.UseBinary = true;
                request.UsePassive = true;
                request.Timeout = -1;

                using (Stream fileStream = System.IO.File.OpenRead(localFilenames[i]))
                using (Stream ftpStream = request.GetRequestStream())
                {

                    byte[] buffer = new byte[40961];
                    int read;
                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpStream.Write(buffer, 0, read);
                        UploadProgress[code] = (int)Math.Floor((double)((totalUploaded + fileStream.Position) * 100 / totalSize));
                    }
                    totalUploaded += fileStream.Position;
                    UploadProgress[code] = (int)Math.Floor((double)totalUploaded * 100 / totalSize);
                }
                }
                catch (Exception)
                {
                    UploadProgress[code] = -1;
                    return;
                }
                
            }
            
        }

        [HttpPost("checkupload")]
        public IActionResult PostCheckUpload([FromBody] string jsonString)
        {
            return Ok(UploadProgress.ContainsKey(jsonString) ? UploadProgress[jsonString] : 0);
        }

    }
}
