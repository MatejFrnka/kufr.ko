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
        public List<uint> FindAcceptedFriends(uint userId)
        {
            List<uint> Friends = ReadToIdList(this.db.ExecuteReader("SELECT (SELECT IF(Id_UserSender = @userId,Id_UserReceiver,Id_UserSender) as FriendId )FROM FriendRequest WHERE (State = 'ACCEPTED' AND ((Id_UserSender = @userId) OR (Id_UserReceiver = @userId)))", new Dictionary<string, object>() { { "userId", userId }}));
            return Friends;
        }

        private List<FriendRequest> ReadToFriendList(MySqlDataReader reader)
        {
            List<FriendRequest> result = new List<FriendRequest>();
            while (reader.Read())
            {
                FriendRequest friendRequest = new FriendRequest()
                {
                    Id_UserSender = reader.GetUInt32("Id_UserSender"),
                    Id_UserReceiver = reader.GetUInt32("Id_UserReceiver"),
                    State = (FriendRequestState)Enum.Parse(typeof(FriendRequestState), reader.GetString("State")),
                };
                result.Add(friendRequest);
            }

            reader.Close();
            return result;
        }
        private List<uint> ReadToIdList(MySqlDataReader reader)
        {
            List<uint> result = new List<uint>();
            while (reader.Read())
            {
                result.Add(reader.GetUInt32("FriendId"));
            }

            reader.Close();
            return result;
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