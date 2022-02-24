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
        [HttpPost("move")]
        public async Task<IActionResult> PostMove([FromBody] MoveRequest data)
        {
            try
            {
                FtpClient client = GetFtpClient(data);
                if (!await client.DirectoryExistsAsync(data.Source)) { await client.MoveFileAsync(data.Source, data.Destination, FtpRemoteExists.Overwrite); }
                else { await client.MoveDirectoryAsync(data.Source, data.Destination, FtpRemoteExists.Overwrite); }
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
