using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public string Bio { get; set; }
        public string Displayname { get; set; }
        public string ProfilePicturePath { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
