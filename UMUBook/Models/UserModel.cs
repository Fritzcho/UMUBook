using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class UserModel
    {
        public UserModel() { }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
