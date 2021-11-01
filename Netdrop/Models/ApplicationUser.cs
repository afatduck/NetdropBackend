using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Netdrop.Models
{
    public class ApplicationUser : IdentityUser
    {
        [InverseProperty("ApplicationUser")]
        public virtual List<SavedCredentials> SavedCredentials { get; set; }

    }
}
