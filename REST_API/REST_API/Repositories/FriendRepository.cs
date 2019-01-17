using MySql.Data.MySqlClient;
using REST_API.Models.Database;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REST_API.Models.Enums;

namespace REST_API.Repositories
{
    public class FriendRepository
    {
        private DbManager db;

        public FriendRepository(DbManager dbManager)
        {
            this.db = dbManager;
        }

        public FriendRequest FindById(uint idSender,uint idReceiver)
        {
            return ReadFriendRequest(this.db.ExecuteReader("SELECT * FROM FriendRequest WHERE (Id_UserSender = @idSender) AND (Id_UserReceiver = @idReceiver)", new Dictionary<string, object>() { { "idSender", idSender }, { "idReceiver", idReceiver } }));
        }
        public List<UserPublic> FindAcceptedFriends(uint userId)
        {
            List<UserPublic> friends = ReadToUserList(this.db.ExecuteReader("SELECT f.Id_UserSender as Id, u.Name, u.Id_Attachment, u.LastOnline FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserSender WHERE f.Id_UserReceiver = @userId AND f.State='ACCEPTED' UNION SELECT f.Id_UserReceiver as Id, u.Name, u.Id_Attachment, u.LastOnline FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserReceiver WHERE f.Id_UserSender = @userId AND f.State='ACCEPTED'", new Dictionary<string, object>() { { "userId", userId }}));
            return friends;
        }
        public List<UserPublic> SearchPossibleFriends(uint userId, string fulltext)
        {
            List<UserPublic> friends = ReadToUserListNotFriends(this.db.ExecuteReader("SELECT u.Id, u.Name, u.Id_Attachment FROM User u LEFT JOIN (SELECT f.Id_UserSender as Id FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserSender WHERE f.Id_UserReceiver = @userId UNION SELECT f.Id_UserReceiver as Id FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserReceiver WHERE f.Id_UserSender = @userId) friends ON friends.Id = u.Id WHERE u.Visibility = 'PUBLIC' AND (instr(u.Name,@fulltext) OR u.Email = @fulltext) AND friends.Id is null", new Dictionary<string, object>() { { "userId", userId }, { "fulltext", fulltext } }));
            return friends;
        }
        public List<UserPublic> FindByStateToUser(uint userId, FriendRequestState state)
        {
            List<UserPublic> friends = ReadToUserListNotFriends(this.db.ExecuteReader("SELECT f.Id_UserSender as Id, u.Name, u.Id_Attachment FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserSender WHERE f.Id_UserReceiver = @userId AND f.State = @state", new Dictionary<string, object>() { { "userId", userId }, {"state", state.ToString() } }));
            return friends;
        }
        public List<UserPublic> FindByStateFromUser(uint userId, FriendRequestState state)
        {
            List<UserPublic> friends = ReadToUserListNotFriends(this.db.ExecuteReader("SELECT f.Id_UserReceiver as Id, u.Name, u.Id_Attachment FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserReceiver WHERE f.Id_UserSender = @userId AND f.State = @state", new Dictionary<string, object>() { { "userId", userId }, { "state", state.ToString() } }));
            return friends;
        }
        private List<UserPublic> ReadToUserList(MySqlDataReader reader)
        {
            List<UserPublic> result = new List<UserPublic>();
            while (reader.Read())
            {
                UserPublic u = new UserPublic()
                {
                    Id = reader.GetUInt32("Id"),
                    Name = reader.GetString("Name"),
                    Id_Attachment = reader.GetUInt32("Id_Attachment"),
                    LastOnline = reader.IsDBNull(reader.GetOrdinal("LastOnline")) ? (DateTime?) null : reader.GetDateTime("LastOnline")
                    //,DefaultGroup = reader.IsDBNull(reader.GetInt32("GroupId")) ? (uint?)null : reader.GetUInt32("GroupId")
                };
                result.Add(u);
            }

            reader.Close();
            return result;
        }
        private List<UserPublic> ReadToUserListNotFriends(MySqlDataReader reader)
        {
            List<UserPublic> result = new List<UserPublic>();
            while (reader.Read())
            {
                UserPublic u = new UserPublic()
                {
                    Id = reader.GetUInt32("Id"),
                    Name = reader.GetString("Name"),
                    Id_Attachment = reader.GetUInt32("Id_Attachment")
                };
                result.Add(u);
            }

            reader.Close();
            return result;
        }
        public bool CreateRequest(uint IdFrom,uint IdTo)
        {
            string sql = "INSERT INTO FriendRequest(Id_UserSender, Id_UserReceiver) SELECT @IdFrom, @IdTo FROM User u WHERE u.Id = 51 AND u.Visibility = 'PUBLIC' AND NOT EXISTS (SELECT 1 FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserReceiver INNER JOIN User u2 ON u2.Id = f.Id_UserSender WHERE (f.Id_UserReceiver = @IdFrom AND f.Id_UserSender = @IdTo) OR (f.Id_UserReceiver = @IdTo AND f.Id_UserSender = @IdFrom))";

            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } })==1)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public bool AcceptFriend(uint IdFrom, uint IdTo)
        {
            string sql = "UPDATE FriendRequest SET State = 'ACCEPTED' WHERE Id_UserSender = @IdFrom AND Id_UserReceiver = @IdTo";

            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } }) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool BlockFriend(uint IdFrom, uint IdTo)
        {
            string sql = "UPDATE FriendRequest SET State = 'BLOCKED' WHERE Id_UserSender = @IdTo AND Id_UserReceiver = @IdFrom";
            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } }) == 1)
            {
                return true;
            }
            else
            {
                //Vim je to prasárna ale nechtělo se mi psát select kterej by to udělal v jednom takže #ŠibravůvTrychtýř
                string sql2 = "UPDATE FriendRequest SET Id_UserSender = @IdFrom, Id_UserReceiver = @IdTo, State = 'BLOCKED' WHERE Id_UserSender = @IdTo AND Id_UserReceiver = @IdFrom";
                if (this.db.ExecuteNonQuery(sql2, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } }) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        public bool DeleteFriend(uint IdFrom, uint IdTo)
        {
            string sql = "DELETE FROM FriendRequest WHERE Id_UserSender = @IdTo AND Id_UserReceiver = @IdFrom";

            
            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } }) == 1)
            {
                return true;
            }
            else
            {
                //Same as above
                string sql2 = "DELETE FROM FriendRequest WHERE Id_UserSender = @IdFrom AND Id_UserReceiver = @IdTo";
                if (this.db.ExecuteNonQuery(sql2, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } }) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        private FriendRequest ReadFriendRequest(MySqlDataReader reader)
        {
            FriendRequest result = new FriendRequest();
            result.Id_UserSender = reader.GetUInt32("Id_UserSender");
            result.Id_UserReceiver = reader.GetUInt32("Id_UserReceiver");
            result.State = (FriendRequestState)Enum.Parse(typeof(FriendRequestState), reader.GetString("State"));

            reader.Close();
            return result;
        }
    }
}