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

        private FtpClient GetFtpClient(BaseFtpRequest data)
        {
            FtpClient client = new(data.Host, data.Port, data.Username, data.Password);
            client.DataConnectionEncryption = true;
            client.SslProtocols = System.Security.Authentication.SslProtocols.None;
            client.EncryptionMode = data.Secure ? FtpEncryptionMode.Implicit : FtpEncryptionMode.None;
            client.ValidateAnyCertificate = true;
            client.SocketPollInterval = 1000;
            client.ConnectTimeout = 2000;
            client.ReadTimeout = 2000;
            client.DataConnectionConnectTimeout = 2000;
            client.DataConnectionReadTimeout = 2000;
            client.DataConnectionType = FtpDataConnectionType.AutoPassive;
            FtpTrace.LogToFile = "resiazure.txt";
            return client;
        }

    }
}
