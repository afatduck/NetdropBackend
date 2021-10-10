using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Netdrop.Models
{
    public class Users
    {
        [Key]

        public int Id { get; set; }

        [Required]

        public string Username { get; set; }

        public string Password { get; set; }

    }
}
