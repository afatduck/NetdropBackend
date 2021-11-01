using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces;
using Netdrop.Interfaces.Requests;
using Netdrop.Interfaces.Responses;
using Netdrop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> PostRegister([FromBody] ReqistrationRequest user)
        {
            if (!ModelState.IsValid) {
                return Ok(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>() { "Request form not valid."}
                });
            }

            ApplicationUser existingUser = await _userManager.FindByNameAsync(user.Username);
            if (existingUser != null)
            {
                return Ok(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>() { "Username already taken." }
                });
            }

            ApplicationUser newUser = new ApplicationUser() { UserName = user.Username };
            IdentityResult isCreated = await _userManager.CreateAsync(newUser, user.Password);

            if (isCreated.Succeeded)
            {
                string jwtToken = GenerateJwtToken(newUser);
                Response.Cookies.Append("jwt", jwtToken, new CookieOptions()
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.MaxValue
                });
                return Ok(new RegistrationResponse()
                {
                    Result = true,
                    UserData = new UserData()
                    {
                        Username = user.Username,
                        Credentials = newUser.SavedCredentials
                    }
                });
            }

            return new JsonResult(new RegistrationResponse() {
                Result = false,
                Errors = isCreated.Errors.Select(x => x.Description).ToList()
            })
            { StatusCode = 500 };

        }
    }
}
