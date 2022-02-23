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
using FluentFTP;
using Netdrop.Interfaces;
using FluentFTP.Helpers;
using Netdrop.Interfaces.Responses;

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
            string clientKey = Request.Cookies["connection"];

            try
            {
                if (!string.IsNullOrEmpty(clientKey) && SavedClients.ContainsKey(clientKey))
                {
                    FtpClient client = SavedClients[clientKey];
                    if (client != null && !client.IsDisposed)
                    {
                        return Ok(new ContactResponse()
                        {
                            ConnectionFound = true,
                            Host = client.Host,
                            Username = client.Credentials.UserName,
                            Password = client.Credentials.Password,
                            Port = client.Port,
                            Secure = client.EncryptionMode == FtpEncryptionMode.Implicit
                        });
                    }
                    
                }
            }
            catch (FtpException) { }

            return Ok(new ContactResponse()
            {
                ConnectionFound = false
            });

        }

    }
}
