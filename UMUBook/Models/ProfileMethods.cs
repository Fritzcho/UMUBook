using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace UMUBook.Models
{
    public class ProfileMethods
    {
        //Denna kanske kan flyttas till en parent-class för alla method-classes för att slippa ha den på flera ställen.
        private static readonly string connectionstring = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public ProfileMethods() { }
        
        //LITE TESTSKIT FÖR ATT FÖRSÖKA FÅ CONNECTION STRING FRÅN appsettings.json (har inte lyckats än)
        //private readonly IConfiguration _config;
        //private static string connectionstring;
        //public ProfileMethods(IConfiguration config) 
        //{
        //    _config = config;
        //    connectionstring = _config.GetConnectionString("SocialMedia");
        //}

        /**
         * För att generera en generell profil när en användare skapas
         */
        public int CreateStandardProfile(UserModel user, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = connectionstring;
            string sqlstring = "INSERT INTO Profile (ProfileUsername, Displayname) VALUES (@Username, @Displayname)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

            dbCommand.Parameters.Add("Username", SqlDbType.NVarChar,50).Value = user.Username;
            dbCommand.Parameters.Add("Displayname", SqlDbType.NVarChar,50).Value = user.Username;
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

        public int UpdateProfile(ProfileModel profile, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string sqlstring = "UPDATE Profile SET DisplayName = @displayname, Bio = @bio, ProfilePicturePath = @path WHERE ProfileUsername = @username ";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar, 50).Value = profile.Username;
            dbCommand.Parameters.Add("path", SqlDbType.NVarChar, 100).Value = profile.ProfilePicturePath;
            dbCommand.Parameters.Add("bio", SqlDbType.NVarChar, 500).Value = profile.Bio;
            dbCommand.Parameters.Add("displayname", SqlDbType.NVarChar, 50).Value = profile.Displayname;
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

        public ProfileModel SelectProfile(string username, out string ermsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            String sqlstring = "SELECT * FROM Profile WHERE Profile.ProfileUsername=@username";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);

            dbCommand.Parameters.Add("username", SqlDbType.NVarChar, 50).Value = username;
            SqlDataReader reader = null;
            ermsg = null;
            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();
                ProfileModel profile = new ProfileModel();
                while (reader.Read())
                {
                    profile.Username = reader["ProfileUsername"].ToString();
                    profile.Displayname = reader["DisplayName"].ToString();
                    if (reader["ProfilePicturePath"] != null)
                    {
                        profile.ProfilePicturePath = reader["ProfilePicturePath"].ToString();
                    }
                    if (reader["Bio"] != null)
                    {
                        profile.Bio = reader["Bio"].ToString();
                    }
                }

                reader.Close();
                return (profile);
            }
            catch (Exception e)
            {
                ermsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public List<ProfileModel> ListProfiles(out string ermsg)
        {
            SqlConnection dbConnection = new SqlConnection();
            dbConnection.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            String sqlstring = "SELECT * FROM Profile";
            SqlCommand dbCommand = new SqlCommand(sqlstring, dbConnection);
            SqlDataReader reader = null;
            ermsg = null;
            List<ProfileModel> profileList = new List<ProfileModel>();
            try
            {
                dbConnection.Open();
                reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    ProfileModel profile = new ProfileModel();
                    profile.Username = reader["ProfileUsername"].ToString();
                    profile.Displayname = reader["DisplayName"].ToString();
                    if (reader["ProfilePicturePath"] != null)
                    {
                        profile.ProfilePicturePath = reader["ProfilePicturePath"].ToString();
                    }
                    if (reader["Bio"] != null)
                    {
                        profile.Bio = reader["Bio"].ToString();
                    }

                    profileList.Add(profile);
                }

                reader.Close();
                return (profileList);
            }
            catch (Exception e)
            {
                ermsg = e.Message;
                return null;
            }
            finally
            {
                dbConnection.Close();
            }
        }
    }
}
