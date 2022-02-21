using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class ProfileModel
    {
        public ProfileModel() { }

        public ProfileModel(UserModel user)
        {
            this.Username = user.Username;
            this.Displayname = user.Username;
        }

        [Required]
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Displayname { get; set; }
        public string ProfilePicturePath { get; set; }
        public bool isfollowing { get; set; }
    }
}
