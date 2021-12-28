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
        [HttpPost("cun")]
        public async Task<IActionResult> PostChangeUsername([FromBody] ChangeRequest data)
        {
            ClaimsPrincipal userToken = HttpContext.User;
            string username = userToken.Claims.Where(x => x.Type == "Username").FirstOrDefault()?.Value;
            string passwordHash = userToken.Claims.Where(y => y.Type == "Password").FirstOrDefault()?.Value;

            ApplicationUser user = await _userManager.FindByNameAsync(username);

            string error = string.Empty;

            if (user == null || passwordHash != user?.PasswordHash)
            {
                ApplicationUser otherUser = await _userManager.FindByNameAsync(data.New);

                if (otherUser == null)
                {
                    if (await _userManager.CheckPasswordAsync(user, data.Old))
                    {
                        IdentityResult res = await _userManager.SetUserNameAsync(user, data.New);
                        if (!res.Succeeded) { error = res.Errors.Select(x => x.Description).First(); }
                        else
                        {
                            string jwtToken = GenerateJwtToken(user);
                            Response.Cookies.Append("jwt", jwtToken, new CookieOptions()
                            {
                                HttpOnly = true,
                                SameSite = SameSiteMode.None,
                                Expires = DateTime.MaxValue,
                                Secure = true
                            });
                        }
                    }
                    else { error = "Password incorrect."; }
                }

                else { error = "Username already taken."; }
                
            }
            else { error = "Something went wrong, try realoading."; }

            return Ok(new ResultBase()
            {
                Result = error == string.Empty,
                Errors = error == string.Empty ? null : new List<string>() { error }
            });
        }
    }
}
