using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Requests;
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
        [HttpPost("delete")]
        public async Task<IActionResult> PostDelete([FromBody] BaseFtpRequest data)
        {
            try
            {
                FtpClient client = new(data.Host, data.Port, data.Username, data.Password);
                await client.ConnectAsync();
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.EncryptionMode = data.Secure ? FtpEncryptionMode.Auto : FtpEncryptionMode.None;
                client.ValidateAnyCertificate = true;
                if (!await client.DirectoryExistsAsync(data.Path)) { await client.DeleteFileAsync(data.Path); }
                else { await client.DeleteDirectoryAsync(data.Path); }
            }
            catch (Exception ex)
            {
                return Ok(new ResultBase() { 
                    Errors = new List<string>() { ex.Message },
                    Result = false
                });
            }
            return Ok(new ResultBase() { Result = true });
        }
    }
}
