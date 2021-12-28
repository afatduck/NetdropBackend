using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
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
        [HttpPost("del")]
        public async Task<IActionResult> PostDeleteAccount([FromBody] ChangeRequest data)
        {
            ClaimsPrincipal userToken = HttpContext.User;
            string username = userToken.Claims.Where(x => x.Type == "Username").FirstOrDefault()?.Value;
            string passwordHash = userToken.Claims.Where(y => y.Type == "Password").FirstOrDefault()?.Value;

            ApplicationUser user = await _userManager.FindByNameAsync(username);

            List<string> error = null;

            if (user != null || passwordHash != user?.PasswordHash)
            {
                if (await _userManager.CheckPasswordAsync(user, data.Old))
                {
                    IdentityResult res = await _userManager.DeleteAsync(user);
                    if (!res.Succeeded) { error = res.Errors.Select(x => x.Description).ToList(); }
                    else { Response.Cookies.Delete("jwt"); }
                }
                else { error = new List<string>() { "Password incorrect." }; }

            }
            else { error = new List<string>() { "Something went wrong, try realoading." }; }

            return Ok(new ResultBase()
            {
                Result = error == null,
                Errors = error
            });
        }
    }
}
