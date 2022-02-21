using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMUBook.Models;
using System.Data.SqlClient;
using System.Data;

namespace UMUBook.Models
{
    public class FollowingMethods
    {
        public List<ProfileModel> ViewFollowers(string user, out string errormsg)
        {
            List<ProfileModel> followers = new List<ProfileModel>();

            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string sqlstring = "SELECT FollowerUser FROM Following WHERE FollowedUser = @user";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

            dbCommand.Parameters.Add("user", SqlDbType.NVarChar).Value = user;


            try {
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        ProfileModel pm = new ProfileModel();
                        pm.Username = dbReader["FollowerUser"].ToString();
                        followers.Add(pm);
                    }

                    errormsg = "";
                    return followers;
                }
                else
                {
                    errormsg = "No Followers found";
                    return followers;
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

        public List<ProfileModel> ViewFollowing(string user, out string errormsg)
        {
            List<ProfileModel> following = new List<ProfileModel>();
            SqlConnection db = new SqlConnection();

            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string sqlstring = "SELECT FollowedUser FROM Following WHERE FollowerUser = @user";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

           

            dbCommand.Parameters.Add("user", SqlDbType.NVarChar).Value = user;

            try
            {
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();
                if (dbReader.HasRows)
                {
                    while (dbReader.Read())
                    {
                        ProfileModel pm = new ProfileModel();
                        pm.Username = dbReader["FollowedUser"].ToString();
                        following.Add(pm);
                    }

                    errormsg = "";
                    return following;
                }
                else
                {
                    errormsg = "No Followers found";
                    return following;
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

        public int FollowUser(string user , string followedUser , out string errormsg)
        {
            if(user != followedUser)
            {
                SqlConnection db = new SqlConnection();
                db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                string sqlstring = "INSERT INTO Following (FollowerUser, FollowedUser) VALUES (@user, @FollowedUser)";
                SqlCommand dbCommand = new SqlCommand(sqlstring, db);

                dbCommand.Parameters.Add("user", SqlDbType.NVarChar).Value = user;
                dbCommand.Parameters.Add("FollowedUser", SqlDbType.NVarChar).Value = followedUser;

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
            else
            {
                errormsg = "can't follow self";
                return 0;
            }
            
        }

        public int UnFollowUser(string user, string followedUser, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string sqlstring = "DELETE FROM Following WHERE FollowerUser = @user AND FollowedUser = @followedUser";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

            dbCommand.Parameters.Add("user", SqlDbType.NVarChar).Value = user;
            dbCommand.Parameters.Add("followedUser", SqlDbType.NVarChar).Value = followedUser;

            try
            {
                db.Open();
                int i = 0;

                i = dbCommand.ExecuteNonQuery();

                if (i == 0)
                {
                    errormsg = "Error: Can't Unfollow";
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

        public bool isFollowed(string user, string followedUser, out string error)
        {
            SqlConnection db = new SqlConnection();

            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string sqlstring = "SELECT * FROM Following WHERE FollowerUser = @user And FollowedUser = @followedUser";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);



            dbCommand.Parameters.Add("user", SqlDbType.NVarChar).Value = user;
            dbCommand.Parameters.Add("followedUser", SqlDbType.NVarChar).Value = followedUser;

            try
            {
                error = "";
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();
                if (dbReader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
            finally
            {
                db.Close();
            }
        }

        
    }
}
