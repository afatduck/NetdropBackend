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
using Limilabs.FTP.Client;
using System.Net.Security;

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
            RegisterFtps();
        }

        [HttpGet]
        public IActionResult ContactApi()
        {
            return Ok();
        }

        private static void ValidateCertificate(
            object sender,
            ServerCertificateValidateEventArgs e)
        {
            const SslPolicyErrors ignoredErrors =
                SslPolicyErrors.RemoteCertificateChainErrors |
                SslPolicyErrors.RemoteCertificateNameMismatch;

            if ((e.SslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None)
            {
                e.IsValid = true;
                return;
            }
            e.IsValid = false;
        }

        private void RegisterFtps()
        {
            WebRequest.RegisterPrefix("ftps", new FtpsWebRequestCreator());
        }

        private sealed class FtpsWebRequestCreator : IWebRequestCreate
        {
            public WebRequest Create(Uri uri)
            {
                FtpWebRequest webRequest = (FtpWebRequest)WebRequest.Create(uri.AbsoluteUri.Remove(3, 1));
                webRequest.EnableSsl = true;
                return webRequest;
            }
        }
    }
}
