using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpGet("logout")]
        public IActionResult GetLogout()
        {
            Response.Cookies.Delete("jwt");
            return Ok();
        }
    }
}
