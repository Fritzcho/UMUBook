using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UMUBook.Models;

namespace UMUBook.Controllers
{
    public class PostsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult _CreatePost()
        {
            return View(new PostsModel());
        }

        [HttpPost]
        public ActionResult _CreatePost(PostsModel newPost)
        {
            newPost.PosterUsername = User.Identity.Name;
            

            PostsMethod pm = new PostsMethod();
            string error;
            pm.CreatePost(newPost, out error);


            if(error != "")
            {
                TempData["error"] = error;
            }

            if(newPost.ReplyID != 0)
            {
                return RedirectToAction("Post", "Posts", new { PostID = newPost.ReplyID });
            }
            else
            {
                return RedirectToAction("Feed", "User");
            }

        }

        public ActionResult _Post(PostPartialViewModel post)
        {
            return View(post);
        }

        public ActionResult DeletePost(int postID)
        {
            PostsMethod pm = new PostsMethod();
            string error = "";

            PostsModel p = pm.GetPostByID(postID, out error);

            if (!p.PosterUsername.Equals(User.Identity.Name))
            {
                TempData["error"] = "You're not authorized to delete that post.";
                return RedirectToAction("Feed", "User");
            }


            pm.DeletePost(postID, out error);

            TempData["error"] = error;
            return RedirectToAction("Feed", "User");
        }

        public ActionResult Post(int PostID)
        {
            PostsMethod pm = new PostsMethod();
            LikeMethods lm = new LikeMethods();
            string error;

            PostsModel post = pm.GetPostByID(PostID, out error);

            if(error == "")
            {
                post.Responses = pm.GetPosts("", PostID, out error);
                post.Likes = lm.GetLikesForPost(post.PostID, out string error2);
                post.LikedByLoggedIn = lm.UserHasLiked(post.PostID, User.Identity.Name, out string error3);
                post.SharedByLoggedIn = lm.UserHasShared(post.PostID, User.Identity.Name, out string error4);

                foreach (PostsModel p in post.Responses)
                {
                    p.Responses = pm.GetPosts("", p.PostID, out error);
                }
            }

            return View(post);
        }
    }
}