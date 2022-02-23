using FluentFTP;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("rename")]
        public async Task<IActionResult> PostRename([FromBody] RenameRequest data)
        {
            try
            {

                FtpClient client = GetFtpClient(data);
                await client.RenameAsync(data.Path, data.Path.Substring(0, data.Path.LastIndexOf('/') + 1) + data.Name);
                client.DisconnectAsync();

            }
            catch (WebException ex)
            {

                return Ok(new ResultBase() 
                { 
                    Result = false,
                    Errors = new List<string>() { ex.InnerException != null ? ex.InnerException.Message : ex.Message }
                });
            }
            return Ok(new ResultBase() { Result = true });
        }
    }
}
