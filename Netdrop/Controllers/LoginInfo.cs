using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netdrop.Interfaces;
using Netdrop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("logininfo")]
        public async Task<IActionResult> GetLoginInfo()
        {
            ClaimsPrincipal usertoken = HttpContext.User;
            string username = usertoken.Claims.Where(x => x.Type == "Username").FirstOrDefault()?.Value;
            string passwordHash = usertoken.Claims.Where(y => y.Type == "Password").FirstOrDefault()?.Value;

            ApplicationUser user = await _userManager.FindByNameAsync(username);

            if (user == null || passwordHash != user?.PasswordHash)
            {
                return BadRequest();
            }

            return Ok(new UserData() { 
                Username = username,
                Credentials = user.SavedCredentials
            });
        }
    }
}
