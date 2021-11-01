﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        [HttpPost("newcred")]
        public async Task<IActionResult> PostNewCredential([FromBody] NewCredentialRequest newCredential)
        {
            ClaimsPrincipal userToken = HttpContext.User;
            string username = userToken.Claims.Where(x => x.Type == "Username").FirstOrDefault()?.Value;

            ApplicationUser user = await _userManager.FindByNameAsync(username);
            await _context.SavedCredentials.AddAsync(new SavedCredentials()
            {
                Host = newCredential.Host,
                Username = newCredential.Username,
                Password = newCredential.Password,
                Secure = newCredential.Secure,
                ApplicationUser = user,
                ApplicationUserId = user.Id
            });

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}