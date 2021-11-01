using Netdrop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Netdrop.Interfaces
{
    public class UserData
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public virtual List<SavedCredentials> Credentials { get; set; }

    }
}
