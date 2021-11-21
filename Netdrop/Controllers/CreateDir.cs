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

                FtpClient client = GetFtpClient(data);
                await client.ConnectAsync();
                await client.CreateDirectoryAsync(data.Path);
                client.Dispose();

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

    }
}
