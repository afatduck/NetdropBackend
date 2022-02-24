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
                FtpClient client = GetFtpClient(data);
                if (!await client.DirectoryExistsAsync(data.Path)) { await client.DeleteFileAsync(data.Path); }
                else { await client.DeleteDirectoryAsync(data.Path); }
                if (!data.Save) client.Dispose();
            }
            catch (Exception ex)
            {
                return Ok(new ResultBase() { 
                    Errors = new List<string>() { ex.InnerException != null ? ex.InnerException.Message : ex.Message },
                    Result = false
                });
            }
            return Ok(new ResultBase() { Result = true });
        }
    }
}
