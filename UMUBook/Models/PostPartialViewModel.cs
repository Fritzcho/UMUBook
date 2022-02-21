using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class PostPartialViewModel
    {
        public PostPartialViewModel() { }

        public PostsModel Post { get; set; }
        public int RepliesShown { get; set; }
    }
}
