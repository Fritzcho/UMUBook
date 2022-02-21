using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class PostsModel
    {
        public PostsModel() { }

        public int PostID { get; set; }
        public int ReplyID { get; set; }
        public string PosterUsername { get; set; }
        public string PostText { get; set; }
        public string PostDate { get; set; }
        public ProfileModel Profile { get; set; }
        public List<PostsModel> Responses { get; set; }
        public int Likes { get; set; }
        public bool LikedByLoggedIn { get; set; }
        public bool SharedByLoggedIn { get; set; }
        public string SharerUsername { get; set; }
    }
}
