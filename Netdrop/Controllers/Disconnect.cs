using FluentFTP;
using Microsoft.AspNetCore.Http;
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
        [HttpPost("disconnect")]
        public IActionResult PostDisconnect([FromBody] string con)
        {

            if (!string.IsNullOrEmpty(con))
            {
                FtpClient client = SavedClients.ContainsKey(con) ? SavedClients[con] : null;
                if (client != null) client.Dispose();

                if (con == Request.Cookies["connection"]) Response.Cookies.Append("connection", "", new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.Now.AddDays(-2),
                });

            }

            return Ok();
        }
    }
}
