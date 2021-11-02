using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Netdrop.Models
{
    public class SavedCredentials
    {

        private string _password;

        [Key]
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Host { get; set; }
        
        public string Username { get; set; }

        public string Password {
            get => EncryptField.Decrypt(_password);
            set { _password = EncryptField.Encrypt(value); }
        }

        [Required]
        public bool Secure { get; set; } = false;

        [JsonIgnore]
        public string ApplicationUserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}
