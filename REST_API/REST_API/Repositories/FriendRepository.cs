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
        public List<UserPublic> FindByState(uint userId, FriendRequestState state)
        {
            List<UserPublic> friends = ReadToUserListNotFriends(this.db.ExecuteReader("SELECT f.Id_UserSender as Id, u.Name, u.Id_Attachment FROM FriendRequest f INNER JOIN User u ON u.Id = f.Id_UserReceiver WHERE f.Id_UserReceiver = @userId AND f.State = @state", new Dictionary<string, object>() { { "userId", userId }, {"state", state.ToString() } }));
            return friends;
        }
        //private List<FriendRequest> ReadToFriendList(MySqlDataReader reader)
        //{
        //    List<FriendRequest> result = new List<FriendRequest>();
        //    while (reader.Read())
        //    {
        //        FriendRequest friendRequest = new FriendRequest()
        //        {
        //            Id_UserSender = reader.GetUInt32("Id_UserSender"),
        //            Id_UserReceiver = reader.GetUInt32("Id_UserReceiver"),
        //            State = (FriendRequestState)Enum.Parse(typeof(FriendRequestState), reader.GetString("State")),
        //        };
        //        result.Add(friendRequest);
        //    }

        //    reader.Close();
        //    return result;
        //}
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
        //private List<uint> ReadToIdList(MySqlDataReader reader)
        //{
        //    List<uint> result = new List<uint>();
        //    while (reader.Read())
        //    {
        //        result.Add(reader.GetUInt32("FriendId"));
        //    }

        //    reader.Close();
        //    return result;
        //}
        public bool CreateRequest(uint IdFrom,uint IdTo)
        {
            string sql = "INSERT INTO FriendRequest(Id_UserSender, Id_UserReceiver) SELECT @IdFrom,@IdTo FROM User WHERE Id = @IdTo AND Visibility = 'PUBLIC'";

            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_From", IdFrom }, { "Id_To", IdTo } })==1)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public bool RespondToRequest(uint IdFrom, uint IdTo,FriendRequestState action)
        {
            string sql = "UPDATE FriendRequest SET State = @state WHERE Id_UserSender = @IdFrom AND Id_UserReceiver = @IdTo";

            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_UserSender", IdFrom }, { "Id_UserReceiver", IdTo }, { "state", action.ToString() } }) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public bool DeleteFriend(uint IdFrom, uint IdTo)
        {
            string sql = "DELETE FROM FriendRequest WHERE Id_UserSender = @IdFrom AND Id_UserReceiver = @IdTo";

            if (this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "IdFrom", IdFrom }, { "IdTo", IdTo } }) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        //public bool IsVisibleTo(uint Id)
        //{
        //    if (true)
        //    {
        //        return true;
        //    }
        //    else
        // {
        //        return false;
        //    }
        //}
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