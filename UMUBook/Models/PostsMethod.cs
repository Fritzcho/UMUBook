using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMUBook.Models;
using System.Data.SqlClient;
using System.Data;

namespace UMUBook.Models
{
    public class PostsMethod
    {
        public int CreatePost(PostsModel newPost, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string sqlstring = "INSERT INTO Posts (PosterUsername, PostText, ReplyID, PostDate, PostImagePath) VALUES (@PosterUsername, @PostText, @ReplyID, CURRENT_TIMESTAMP, @PostImagePath)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

            dbCommand.Parameters.Add("PosterUsername", SqlDbType.NVarChar).Value = newPost.PosterUsername;
            dbCommand.Parameters.Add("PostText", SqlDbType.NVarChar).Value = newPost.PostText;
            dbCommand.Parameters.Add("ReplyID", SqlDbType.NVarChar).Value = newPost.ReplyID;
            dbCommand.Parameters.Add("PostImagePath", SqlDbType.NVarChar).Value = "";

            try
            {
                db.Open();
                int i = 0;
                i = dbCommand.ExecuteNonQuery();
                if (i == 1) { errormsg = ""; }
                else { errormsg = "An error occcured"; };
                return (i);
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                db.Close();
            }
        }

        public List<PostsModel> GetPosts(string byUsername, int replyID, out string errormsg, int offset = 0, int rows = 10)
        {
            List<PostsModel> posts = new List<PostsModel>();

            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string sqlstring = "SELECT PostID, PostDate, PosterUsername, PostImagePath, ReplyID, PostText, SharerUsername, SharedPostID, ProfileUsername, Bio, DisplayName, ProfilePicturePath FROM Posts LEFT JOIN Share s ON s.SharedPostID = PostID";
            if (byUsername != "")
            {
                sqlstring += " AND s.SharerUsername = @Username";
            }
            sqlstring += " INNER JOIN Profile ON Posts.PosterUsername = Profile.ProfileUsername " +
            "WHERE ReplyID = @ReplyID";

            if (byUsername != "")
            {
                sqlstring += " AND Posts.PosterUsername = @Username OR SharerUsername = @Username";
            }

            sqlstring += " ORDER BY PostDate DESC";
            sqlstring += " OFFSET @Offset ROWS FETCH NEXT @Rows ROWS ONLY";


            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("ReplyID", SqlDbType.Int).Value = replyID;
            dbCommand.Parameters.Add("Username", SqlDbType.NVarChar).Value = byUsername;
            dbCommand.Parameters.Add("Offset", SqlDbType.Int).Value = offset;
            dbCommand.Parameters.Add("Rows", SqlDbType.Int).Value = rows;


            try
            {
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();

                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        PostsModel pm = new PostsModel();
                        pm.PostID = Convert.ToInt32(dbReader["PostID"]);
                        pm.PosterUsername = dbReader["PosterUsername"].ToString();
                        pm.PostDate = dbReader["PostDate"].ToString();
                        pm.PostText = dbReader["PostText"].ToString();
                        pm.ReplyID = replyID;
                        pm.SharerUsername = "";
                        ProfileModel prm = new ProfileModel();
                        prm.Displayname = dbReader["DisplayName"].ToString();
                        prm.ProfilePicturePath = dbReader["ProfilePicturePath"].ToString();

                        pm.Profile = prm;

                        posts.Add(pm);
                    }

                    errormsg = "";
                    return posts;
                }

                errormsg = "No posts found";
                return posts;

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                db.Close();
            }
        }

        public List<PostsModel> GetPosts(string byUsername, int replyID, string currentUser, out string errormsg, int? offset = 0, int rows = 3)
        {
            List<PostsModel> posts = new List<PostsModel>();

            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string sqlstring = "SELECT PostID, PostDate, PosterUsername, PostImagePath, ReplyID, PostText, ProfileUsername, Bio, DisplayName, ProfilePicturePath FROM Posts LEFT JOIN Profile ON ProfileUsername = PosterUsername LEFT JOIN Share ON Share.SharedPostID = PostID LEFT JOIN Following ON FollowedUser = PosterUsername OR FollowedUser = SharerUsername WHERE ReplyID = @ReplyID AND FollowerUser = @currentUser GROUP BY PostID, PostDate, PosterUsername, PostImagePath, ReplyID, PostText, ProfileUsername, Bio, DisplayName, ProfilePicturePath";

            sqlstring += " ORDER BY PostDate DESC";
            sqlstring += " OFFSET @Offset ROWS FETCH NEXT @Rows ROWS ONLY";
               

            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("ReplyID", SqlDbType.Int).Value = replyID;
            dbCommand.Parameters.Add("currentUser", SqlDbType.NVarChar, 40).Value = currentUser;
            dbCommand.Parameters.Add("Offset", SqlDbType.Int).Value = offset;
            dbCommand.Parameters.Add("Rows", SqlDbType.Int).Value = rows;

            try
            {
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();

                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        PostsModel pm = new PostsModel();
                        pm.PostID = Convert.ToInt32(dbReader["PostID"]);
                        pm.PosterUsername = dbReader["PosterUsername"].ToString();
                        pm.PostDate = dbReader["PostDate"].ToString();
                        pm.PostText = dbReader["PostText"].ToString();
                        pm.ReplyID = replyID;
                        pm.SharerUsername = "";
                        ProfileModel prm = new ProfileModel();
                        prm.Displayname = dbReader["DisplayName"].ToString();
                        prm.ProfilePicturePath = dbReader["ProfilePicturePath"].ToString();

                        pm.Profile = prm;

                        posts.Add(pm);
                    }

                    errormsg = "";
                    return posts;
                }
                else
                {
                    errormsg = "No posts found";
                    return posts;
                }

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                db.Close();
            }
        }

        public int DeletePost(int postID, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            errormsg = "";

            string sqlstring = "DELETE FROM Posts WHERE PostID = @PostID";

            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("PostID", SqlDbType.Int).Value = postID;

            try
            {
                db.Open();
                int i = 0;

                i = dbCommand.ExecuteNonQuery();

                if(i == 0)
                {
                    errormsg = "An error occured while deleting post";
                }
                else
                {
                    errormsg = "";
                }

                return i;
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return 0;
            }
            finally
            {
                db.Close();
            }
        }

        public PostsModel GetPostByID(int postID, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            errormsg = "";

            string sqlstring = "SELECT * FROM Posts INNER JOIN Profile ON Posts.PosterUsername = Profile.ProfileUsername WHERE PostID = @PostID";

            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("PostID", SqlDbType.Int).Value = postID;

            try
            {
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();

                if (dbReader.HasRows)
                {
                    PostsModel pm = new PostsModel();
                    while (dbReader.Read())
                    {
                        pm.PostID = postID;
                        pm.PosterUsername = dbReader["PosterUsername"].ToString();
                        pm.PostDate = dbReader["PostDate"].ToString();
                        pm.PostText = dbReader["PostText"].ToString();
                        pm.ReplyID = Convert.ToInt32(dbReader["ReplyID"]);

                        ProfileModel prm = new ProfileModel();
                        prm.Displayname = dbReader["DisplayName"].ToString();
                        prm.ProfilePicturePath = dbReader["ProfilePicturePath"].ToString();

                        pm.Profile = prm;
                    }

                    return pm;
                }
                else
                {
                    errormsg = "No post found.";
                    return null;
                }

            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                db.Close();
            }
        }




    }
}
