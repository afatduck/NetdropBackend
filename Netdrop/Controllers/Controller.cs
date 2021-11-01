using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Netdrop.Configuration;
using Microsoft.Extensions.Options;
using Netdrop.Models;

namespace Netdrop.Controllers
{

    [Route("api")]
    [ApiController]
    public partial class NetdropController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly NetdropContext _context;

        public NetdropController(UserManager<ApplicationUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, NetdropContext context)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
        }

        [HttpGet]
        public IActionResult ContactApi()
        {
            return Ok();
        }
    }
}
