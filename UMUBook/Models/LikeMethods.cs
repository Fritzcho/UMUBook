using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace UMUBook.Models
{
    public class LikeMethods : Methods
    {
        private SqlConnection db;

        public LikeMethods() 
        {
            db = new SqlConnection();
            db.ConnectionString = connectionString;
        }

        public int AddLike(int postID, string username, out string errormsg)
        {
            string sqlstring = "INSERT INTO Likes (LikesPostID, LikesUserID) VALUES(@postID, @username)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("postID", SqlDbType.Int).Value = postID;
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            return DatabaseWrite(db, dbCommand, out errormsg);
        }

        public int RemoveLike(int postID, string username, out string errormsg)
        {
            string sqlstring = "DELETE FROM Likes WHERE LikesPostID = @postID AND LikesUserID = @username";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("postID", SqlDbType.Int).Value = postID;
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            return DatabaseWrite(db, dbCommand, out errormsg);
        }

        public int GetLikesForPost(int postID, out string errormsg)
        {
            string sqlstring = "SELECT COUNT (*) FROM Likes WHERE LikesPostID = @post";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("post", SqlDbType.Int).Value = postID;
            return DatabaseGetScalar(db, dbCommand, out errormsg);
        }

        public bool UserHasLiked(int postID, string username, out string errormsg)
        {
            string sqlstring = "SELECT COUNT(*) FROM Likes WHERE LikesPostID = @postID AND LikesUserID = @username";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("postID", SqlDbType.Int).Value = postID;
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            return DatabaseGetScalar(db, dbCommand, out errormsg) == 1;
        }

        public int AddShare(int postID, string username, out string errormsg)
        {
            string sqlstring = "INSERT INTO Share (SharedPostID, SharerUsername) VALUES(@postID, @username)";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("postID", SqlDbType.Int).Value = postID;
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            return DatabaseWrite(db, dbCommand, out errormsg);
        }

        public int RemoveShare(int postID, string username, out string errormsg)
        {
            string sqlstring = "DELETE FROM Share WHERE SharedPostID = @postID AND SharerUsername = @username";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("postID", SqlDbType.Int).Value = postID;
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            return DatabaseWrite(db, dbCommand, out errormsg);
        }

        public bool UserHasShared(int postID, string username, out string errormsg)
        {
            string sqlstring = "SELECT COUNT(*) FROM Share WHERE SharedPostID = @postID AND SharerUsername = @username";
            SqlCommand dbCommand = new SqlCommand(sqlstring, db);
            dbCommand.Parameters.Add("postID", SqlDbType.Int).Value = postID;
            dbCommand.Parameters.Add("username", SqlDbType.NVarChar).Value = username;
            return DatabaseGetScalar(db, dbCommand, out errormsg) == 1;
        }
    }
}
