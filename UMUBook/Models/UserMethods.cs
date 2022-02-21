using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace UMUBook.Models
{
    public class UserMethods
    {
        public int RegisterUser(UserModel user, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string sqlstring = "INSERT INTO Users (Username, Password) VALUES (@Name, @Password)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

            dbCommand.Parameters.Add("Name", SqlDbType.NVarChar).Value = user.Username;
            dbCommand.Parameters.Add("Password", SqlDbType.NVarChar).Value = user.Password;
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

        public UserModel LoginUser(UserModel user, out string errormsg)
        {
            SqlConnection db = new SqlConnection();
            db.ConnectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string sqlstring = "SELECT * FROM Users WHERE Username = @Name AND Password = @Password";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);

            dbCommand.Parameters.Add("Name", SqlDbType.NVarChar).Value = user.Username;
            dbCommand.Parameters.Add("Password", SqlDbType.NVarChar).Value = user.Password;
            try
            {
                db.Open();
                SqlDataReader dbReader = dbCommand.ExecuteReader();
                errormsg = "";

                if (dbReader.HasRows)
                {
                    return user;
                }
                else
                {
                    errormsg = "No user with that name or password was found.";
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
