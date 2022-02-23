using FluentFTP;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpPost("textsave")]
        public async Task<IActionResult> PostTextSave([FromBody] TextSaveRequest data)
        {
            try
            {

                FtpClient client = GetFtpClient(data);
                byte[] textData = UnicodeEncoding.UTF8.GetBytes(data.Text);
                await client.UploadAsync(textData, data.Path, FtpRemoteExists.Overwrite);


            }
            catch (Exception ex)
            {
                return Ok(new ResultBase() { 
                    Result = false,
                    Errors = new List<string>() { ex.InnerException != null ? ex.InnerException.Message : ex.Message }
                });
            }
            return Ok(new ResultBase()
            {
                Result = true
            });
        }

    }
}
