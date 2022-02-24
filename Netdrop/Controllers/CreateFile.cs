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
        [HttpPost("createfile")]
        public async Task<IActionResult> PostCreateFile([FromBody] BaseFtpRequest data)
        {
            try
            {

                FtpClient client = GetFtpClient(data);
                await client.UploadAsync(new byte[0], data.Path, FtpRemoteExists.Skip, false);
                if (!data.Save) client.Dispose();

            }
            catch (Exception ex)
            {
                return Ok(new ResultBase() { 
                    Result = false,
                    Errors = new List<string>() { ex.InnerException.Message }
                });
            }
            return Ok(new ResultBase()
            {
                Result = true
            });
        }

    }
}
