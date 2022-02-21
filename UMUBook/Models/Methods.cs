using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public abstract class Methods
    {
        public static readonly string connectionString = @"Data Source=socicalmedia-grupp9.database.windows.net;Initial Catalog=SocialMedia-studentDB;User ID=mongo;Password=Invalid123;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        /**
         * Use for insert, delete or update queries
         * returns: number of affected rows
         * */
        public int DatabaseWrite(SqlConnection db, SqlCommand command, out string errormsg)
        {
            try
            {
                db.Open();
                int i = 0;
                i = command.ExecuteNonQuery();

                if (i == 1) { errormsg = ""; }
                else
                {
                    errormsg = "Something went wrong.";
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

        /**
         * Use only with "select count" queries
         * returns: the first column in the result (casted as int)
         * */
        public int DatabaseGetScalar(SqlConnection db, SqlCommand command, out string errormsg)
        {
            try
            {
                db.Open();
                int i = 0;
                i = (int)command.ExecuteScalar();

                if (i == 1) { errormsg = ""; }
                else {errormsg = "Something went wrong.";}
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

        //methods for reading from db and building models should be implemented in the specific sub-class
    }
}
