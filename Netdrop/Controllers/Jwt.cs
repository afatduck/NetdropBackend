using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Netdrop.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Netdrop.Controllers
{
    public partial class NetdropController : ControllerBase
    {
        private string GenerateJwtToken(ApplicationUser user)
        {
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable(_jwtConfig.Secret));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new []{
                        new Claim("Id", user.Id),
                        new Claim("Username", user.UserName),
                        new Claim("Password", user.PasswordHash)
                    }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string tokenString = jwtTokenHandler.WriteToken(token);

            return tokenString;

        }
    }
}
