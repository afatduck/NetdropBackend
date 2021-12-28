using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Netdrop.Interfaces.Requests;
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
        [HttpPost("removecred")]
        public async Task<IActionResult> PostRemoveCredential([FromBody] string cred)
        {
            ClaimsPrincipal userToken = HttpContext.User;
            string username = userToken.Claims.Where(x => x.Type == "Username").FirstOrDefault()?.Value;
            string passwordHash = userToken.Claims.Where(y => y.Type == "Password").FirstOrDefault()?.Value;

            ApplicationUser user = await _userManager.FindByNameAsync(username);

            if (user == null || passwordHash != user?.PasswordHash)
            {
                return BadRequest();
            }

            SavedCredentials credObject = _context.SavedCredentials
                .Where(x => x.Name == cred)
                .FirstOrDefault();

            if (credObject == null) { return BadRequest(); }

            _context.SavedCredentials.Remove(credObject);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
