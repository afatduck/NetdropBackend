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
        [HttpPost("login")]
        public async Task<IActionResult> PostLogin([FromBody] LoginRequest user)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new LoginResponse() {
                    Result = false,
                    Errors = new List<string>() { "Login form not valid." }
                });
            }

            ApplicationUser existitngUser = await _userManager.FindByNameAsync(user.Username);

            if (existitngUser == null)
            {
                return Ok(new LoginResponse()
                {
                    Result = false,
                    Errors = new List<string>() { "User not found. "}
                });
            }

            bool isCorrect = await _userManager.CheckPasswordAsync(existitngUser, user.Password);

            if (isCorrect)
            {
                string jwtToken = GenerateJwtToken(existitngUser);
                Response.Cookies.Append("jwt", jwtToken, new CookieOptions() {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.MaxValue
                });
                return Ok(new LoginResponse() {
                    Result = true,
                    UserData = new UserData()
                    {
                        Username = user.Username,
                        Credentials = existitngUser.SavedCredentials
                    }
                });
            }

            return Ok(new LoginResponse()
            {
                Result = false,
                Errors = new List<string>() { "Incorrect password." }
            });
        }
    }
}
