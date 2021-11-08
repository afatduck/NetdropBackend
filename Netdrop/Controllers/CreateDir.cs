using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpPost("createdir")]
        public async Task<IActionResult> PostCreateDir([FromBody] BaseFtpRequest data)
        {
            try
            {

                FtpClient client = new(data.Host, data.Port, data.Username, data.Password);
                await client.ConnectAsync();
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.EncryptionMode = data.Secure ? FtpEncryptionMode.Auto : FtpEncryptionMode.None;
                client.ValidateAnyCertificate = true;
                await client.CreateDirectoryAsync(data.Path);

            }
            catch (Exception ex)
            {
                return Ok(new ResultBase() { 
                    Result = false,
                    Errors = new List<string>() { ex.Message }
                });
            }
            return Ok(new ResultBase()
            {
                Result = true
            });
        }

        public async Task CreateDir(string url, string username, string password)
        {

            try
            {
                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(url);
                req.Credentials = new NetworkCredential(username, password);
                req.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse res = (FtpWebResponse) await req.GetResponseAsync();

            }
            catch (WebException ex)
            {
                FtpWebResponse res = (FtpWebResponse)ex.Response;
                if (res.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable) { await CreateDir(url.Remove(url.LastIndexOf('/')), username, password); }
            }

        }

    }
}
