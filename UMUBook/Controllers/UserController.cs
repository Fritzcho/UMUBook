using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using UMUBook.Models;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UMUBook.Controllers
{
    public class UserController : Controller
    {
        private readonly IHostingEnvironment webHostEnvironment;

        public UserController(IHostingEnvironment hostEnvironment)
        {
            webHostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(UserModel newUser)
        {

            newUser.Password = EncryptPassword(newUser.Password);

            UserMethods um = new UserMethods();
            ProfileMethods pm = new ProfileMethods();
            string error;
            um.RegisterUser(newUser, out error);
            pm.CreateStandardProfile(newUser, out string error2);

            if(error == "")
            {
                ViewBag.success = "New user Registered!";
                return RedirectToAction("LoginUser"); //Redirect to login
            }
            else
            {
                ViewBag.error = error;
                return View();
            }
        }

        [HttpGet]
        public ActionResult LoginUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Feed));
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginUser(UserModel user)
        {
            UserMethods um = new UserMethods();
            string error;

            user.Password = EncryptPassword(user.Password);

            UserModel currentUser = um.LoginUser(user, out error);

            ProfileMethods pm = new ProfileMethods();
            ProfileModel profile = pm.SelectProfile(user.Username, out error);

            HttpContext.Response.Cookies.Append("user_pp", profile.ProfilePicturePath);


            if(currentUser == null)
            {
                ViewBag.error = error;
                return View();
            }
            else
            {
                //HttpContext.Session.SetString("currentUser", user.Username);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, "User"),
                };
                
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    // Refreshing the authentication session should be allowed.

                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(60),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                return RedirectToAction("Feed");
            }
        }

        public async Task<ActionResult> LogoutUser()
        {
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(LoginUser));
        }

        public ActionResult Feed()
        {
            HttpContext.Session.SetInt32("offset", 3);
            string error;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(LoginUser));
            }

            if (!HttpContext.Request.Cookies.ContainsKey("user_pp"))
            {
                ProfileMethods prfm = new ProfileMethods();
                ProfileModel p = prfm.SelectProfile(User.Identity.Name, out error);
                HttpContext.Response.Cookies.Append("user_pp", p.ProfilePicturePath);
            }

            PostsMethod pm = new PostsMethod();
            LikeMethods lm = new LikeMethods(); //för att få antal likes på varje post

            List<PostsModel> posts = pm.GetPosts("", 0, User.Identity.Name, out error); //ReplyID = 0

            foreach(PostsModel p in posts)
            {
                p.Responses = pm.GetPosts("", p.PostID, out error);
                p.Likes = lm.GetLikesForPost(p.PostID, out string error2);
                p.LikedByLoggedIn = lm.UserHasLiked(p.PostID, User.Identity.Name, out string error3);
                p.SharedByLoggedIn = lm.UserHasShared(p.PostID, User.Identity.Name, out string error4);
            }

            ViewBag.posts = posts;
            ViewBag.error = error;
            return View();
        }

        public ActionResult LikePost(int post)
        {
            string username = User.Identity.Name;
            LikeMethods lm = new LikeMethods();
            if (lm.UserHasLiked(post, username, out string errorCheckingLike))
            {
                lm.RemoveLike(post, username, out string errorRemovingLike);
            }
            else
            {
                lm.AddLike(post, username, out string errorAddingLike);
            }
            return RedirectToAction("Feed");
        }

        public ActionResult SharePost(int post)
        {
            string username = User.Identity.Name;
            LikeMethods lm = new LikeMethods();
            if (lm.UserHasShared(post, username, out string errorCheckingLike))
            {
                lm.RemoveShare(post, username, out string errorRemovingLike);
            }
            else
            {
                lm.AddShare(post, username, out string errorAddingLike);
            }
            return RedirectToAction("Feed");
        }

        public ActionResult Profile(string username)
        {
            ProfileMethods methods = new ProfileMethods();
            string error;
            ProfileModel profile = methods.SelectProfile(username, out error);

            FollowingMethods methodsFollowing = new FollowingMethods();
            List<ProfileModel> following = methodsFollowing.ViewFollowing(username, out error);
            List<ProfileModel> followers = methodsFollowing.ViewFollowers(username, out error);
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(LoginUser));

            }
            if (!methodsFollowing.isFollowed(User.Identity.Name, username, out error))
            {
                
                ViewBag.followingstatus = "Follow";

            }
            else
            {
                ViewBag.followingstatus = "Unfollow";
            }

            int followercount = followers.Count;
            int followingcount = following.Count;
            ViewData["followcount"] = followercount;
            ViewData["followingcount"] = followingcount;
            ViewData["followers"] = followers;
            ViewData["following"] = following;

            //_____________
            //Get user posts:

            PostsMethod pm = new PostsMethod(); 
            LikeMethods lm = new LikeMethods();

            List<PostsModel> posts = pm.GetPosts(username, 0, out error);
            
            if(posts == null)
            {
                TempData["error"] = error;
                return RedirectToAction("Feed", "User");
            }

            foreach (PostsModel p in posts)
            {
                p.Responses = pm.GetPosts("", p.PostID, out error);
                p.Likes = lm.GetLikesForPost(p.PostID, out string error2);
                p.LikedByLoggedIn = lm.UserHasLiked(p.PostID, User.Identity.Name, out string error3);
                p.SharedByLoggedIn = lm.UserHasShared(p.PostID, User.Identity.Name, out string error4);
            }


            ViewBag.posts = posts;
            

            return View(profile);
        }

        public ActionResult ProfileFollowing(string username)
        {
            string error;
            FollowingMethods methodsFollowing = new FollowingMethods();
            List<ProfileModel> following = methodsFollowing.ViewFollowing(username, out error);
            List<ProfileModel> followers = methodsFollowing.ViewFollowers(username, out error);
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(LoginUser));

            }

            if (!methodsFollowing.isFollowed(User.Identity.Name, username, out error))
            {
                methodsFollowing.FollowUser(User.Identity.Name, username, out error);
            }
            else
            {
                methodsFollowing.UnFollowUser(User.Identity.Name, username, out error);
            }

            TempData["error"] = error;
            return RedirectToAction("Profile", new { username = username });
        }

        public ActionResult EditProfile(string username)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(LoginUser));
            }
            ProfileMethods methods = new ProfileMethods();
            string error;
            ProfileModel profile = methods.SelectProfile(User.Identity.Name, out error);
            ViewBag.error = error;
            if (profile.ProfilePicturePath == null)
                profile.ProfilePicturePath = " ";

            ProfileViewModel profileViewModel = new ProfileViewModel
            {
                Username = profile.Username,
                Displayname = profile.Displayname,
                Bio = profile.Bio,
                ProfilePicturePath = profile.ProfilePicturePath
            };

            return View(profileViewModel);
        }

        [HttpPost]
        public ActionResult EditProfile(string username, ProfileViewModel profileViewModel)
        {

            string error;
            string uniqueFileName = UploadedFile(profileViewModel);
            if (profileViewModel.Bio == null)
                profileViewModel.Bio = "";
            if (profileViewModel.Displayname == null)
                profileViewModel.Displayname = profileViewModel.Username;
            if (uniqueFileName != null)
                profileViewModel.ProfilePicturePath = uniqueFileName;
            ProfileMethods methods = new ProfileMethods();
            ProfileModel profile = new ProfileModel
            {
                Username = profileViewModel.Username,
                Bio = profileViewModel.Bio,
                Displayname = profileViewModel.Displayname,
                ProfilePicturePath = profileViewModel.ProfilePicturePath
            };
            methods.UpdateProfile(profile, out error);
            ViewBag.error = error;

            HttpContext.Response.Cookies.Append("user_pp", profileViewModel.ProfilePicturePath.ToString());
            return RedirectToAction("Profile", new { username = profileViewModel.Username });
        }

        public ActionResult Search(string search)
        {
            ProfileMethods pm = new ProfileMethods();
            SearchViewModel viweModel = new SearchViewModel();
            string errmsg;
            viweModel.Profiles = pm.ListProfiles(out errmsg);
            ViewData["SearchString"] = search;
            if (!String.IsNullOrEmpty(search))
            {
                viweModel.Profiles = viweModel.Profiles.Where(p => p.Username.Contains(search, StringComparison.OrdinalIgnoreCase) || p.Displayname.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            ViewBag.error = errmsg;
            return View(viweModel);
        }

        public string EncryptPassword(string password)
        {
            using (SHA256 sha256hash = SHA256.Create())
            {
                byte[] bytes = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                password = "";

                foreach (byte b in bytes)
                {
                    password += b.ToString("x2");
                }
            }

            return password;
        }

        private string UploadedFile(ProfileViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public IActionResult _Feed(int o)
        {
            string error;
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(LoginUser));
            }

            int? offset = HttpContext.Session.GetInt32("offset");

            PostsMethod pm = new PostsMethod();
            LikeMethods lm = new LikeMethods(); //för att få antal likes på varje post

            List<PostsModel> posts = pm.GetPosts("", 0, User.Identity.Name, out error, offset); //ReplyID = 0
            offset = offset + 3;
            HttpContext.Session.SetInt32("offset", (int)offset);


            foreach (PostsModel p in posts)
            {
                p.Responses = pm.GetPosts("", p.PostID, out error);
                p.Likes = lm.GetLikesForPost(p.PostID, out string error2);
                p.LikedByLoggedIn = lm.UserHasLiked(p.PostID, User.Identity.Name, out string error3);
            }
            ViewBag.error = error;
            return PartialView(posts);
        }

    }
}