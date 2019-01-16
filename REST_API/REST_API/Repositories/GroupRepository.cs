using MySql.Data.MySqlClient;
using REST_API.Models.Api.Group;
using REST_API.Models.Database;
using REST_API.Models.Enums;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Repositories
{
    public class GroupRepository
    {
        private DbManager db;

        public GroupRepository(DbManager dbManager)
        {
            this.db = dbManager;
        }

        public Group FindById(uint id)
        {
            List<Group> result = this.ReadToList(this.db.ExecuteReader("SELECT * FROM `Group` WHERE Id = @id", new Dictionary<string, object>() { { "id", id } }));

            return result.FirstOrDefault();
        }

        public GroupDetailInfo FindByIdInfo(uint groupId,uint userId)
        {
            string sql = "SELECT g.Id, g.GroupName, g.HistoryVisibility, g.Id_Attachment,IF(gu.Id_User = @uid,gu.IgnoreExpire,NULL) AS IgnoreExpire,IF(f.State IS NULL,0,f.State = 'BLOCKED') AS Blocked,IF(g.GroupName IS NOT NULL,g.GroupName,(SELECT GROUP_CONCAT(IF(gu.Nickname IS NULL, uu.Name, gu.Nickname) SEPARATOR ', ') FROM Group_User gu INNER JOIN User uu ON uu.Id = gu.Id_User WHERE gu.Id_Group = g.Id AND gu.Id_User != @uid)) AS DisplayName,gu.Id_User,gu.Id_Group,gu.Nickname,u.Name,gu.Permission FROM `Group` g INNER JOIN Group_User gu ON gu.Id_Group = g.Id INNER JOIN User u ON u.Id = gu.Id_User LEFT JOIN friendrequest f ON g.Id = f.Id_Group WHERE (SELECT COUNT(guu.Id_User) FROM Group_User guu WHERE guu.Id_Group = g.Id AND guu.Id_User = @uid) > 0 AND g.Id = @gid ORDER BY g.Id";

            List<GroupDetailInfo> result = this.ReadToListGroupDetailInfo(this.db.ExecuteReader(sql, new Dictionary<string, object>() { { "uid", userId }, {"gid", groupId } }));

            return result.FirstOrDefault();
        }

        public List<GroupInfo> FindAllForUser(uint userId,string like)
        {
            string sql = "SELECT g.Id,IF(g.GroupName IS NOT NULL,g.GroupName,(SELECT GROUP_CONCAT(IF(gu.Nickname IS NULL, uu.Name, gu.Nickname) SEPARATOR ', ') FROM Group_User gu INNER JOIN User uu ON uu.Id = gu.Id_User WHERE gu.Id_Group = g.Id AND gu.Id_User != @uid)) AS Name, g.Id_Attachment,u.IgnoreExpire,IF(senderG.Nickname IS NULL, senderU.Name, senderG.Nickname) AS sender, m.TextBody,m.Sent,(SELECT COUNT(mn1.Id) FROM Message mn1 WHERE mn1.Id_Group = g.Id) - (SELECT COUNT(mn2.Id) FROM Message mn2 INNER JOIN MessageState ms ON mn2.Id = ms.Id_Message WHERE mn2.Id_Group = g.Id AND ms.Id_User = @uid AND Seen = 1) AS NewMessages " +
                         "FROM `Group` g INNER JOIN Group_User u ON g.Id = u.Id_Group LEFT JOIN (SELECT gd.Id, MAX(m.Id) as mId FROM `Group` gd LEFT JOIN Message m ON m.Id_Group = gd.Id GROUP BY gd.Id) a ON a.Id = g.Id LEFT JOIN Message m ON a.mId = m.Id LEFT JOIN Group_User senderG ON senderG.Id_User = m.Id_User AND senderG.Id_Group = g.ID LEFT JOIN User senderU ON senderU.Id = m.Id_User WHERE u.Id_User = @uid AND IF(IF(g.GroupName IS NOT NULL,g.GroupName,(SELECT GROUP_CONCAT(IF(gu.Nickname IS NULL, uu.Name, gu.Nickname) SEPARATOR ', ') FROM Group_User gu INNER JOIN User uu ON uu.Id = gu.Id_User WHERE gu.Id_Group = g.Id AND gu.Id_User != @uid)) IS NULL,'Pouze já',IF(g.GroupName IS NOT NULL,g.GroupName,(SELECT GROUP_CONCAT(IF(gu.Nickname IS NULL, uu.Name, gu.Nickname) SEPARATOR ', ') FROM Group_User gu INNER JOIN User uu ON uu.Id = gu.Id_User WHERE gu.Id_Group = g.Id AND gu.Id_User != @uid))) LIKE @like ORDER BY NewMessages > 0 DESC,a.mId DESC";

            return this.ReadToListGroupInfo(this.db.ExecuteReader(sql, new Dictionary<string, object>() { { "uid", userId }, {"like",$"%{like}%"} }));

            //return this.ReadToList(this.db.ExecuteReader("SELECT g.* FROM `Group` g INNER JOIN Group_User u ON g.Id = u.Id_Group LEFT JOIN Message m ON m.Id_Group = g.Id WHERE u.Id_User = @uid GROUP BY(g.Id) ORDER BY MAX(m.Id) DESC", new Dictionary<string, object>() { { "uid", userId } }));
        }

        public uint CreateForUserWithDefaults(uint userId)
        {
            string sql = "INSERT INTO `Group`(HistoryVisibility) VALUES (0); SELECT LAST_INSERT_ID();";

            uint gId = Convert.ToUInt32(this.db.ExecuteScalar(sql, new Dictionary<string, object>()));

            string sql2 = "INSERT INTO Group_User(Id_User, Id_Group, Permission) VALUES (@uid,@gid,'OWNER')";

            this.db.ExecuteNonQuery(sql2, new Dictionary<string, object>() { { "uid", userId },{ "gid", gId } });

            return gId;
        }

        private List<GroupDetailInfo> ReadToListGroupDetailInfo(MySqlDataReader reader)
        {
            List<GroupDetailInfo> result = new List<GroupDetailInfo>();

            GroupDetailInfo lastInfo = null;

            while (reader.Read())
            {
                uint gId = reader.GetUInt32("Id");

                if(lastInfo == null || lastInfo.Id != gId)
                {
                    if (lastInfo != null)
                        result.Add(lastInfo);

                    lastInfo = new GroupDetailInfo()
                    {
                        Id = gId,
                        GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName"),
                        HistoryVisibility = reader.GetBoolean("HistoryVisibility"),
                        Id_Attachment = reader.GetUInt32("Id_Attachment"),
                        ReadOnly = reader.GetBoolean("Blocked"),
                        DisplayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? "Pouze já" : reader.GetString("DisplayName"),
                        Users = new List<Group_UserInfo>()
                    };
                }

                Group_UserInfo guInfo = new Group_UserInfo()
                {
                    Id_User = reader.GetUInt32("Id_User"),
                    Id_Group = reader.GetUInt32("Id_Group"),
                    Nickname = reader.IsDBNull(reader.GetOrdinal("Nickname")) ? null : reader.GetString("NickName"),
                    Name = reader.GetString("Name"),
                    Permission = (Permission)Enum.Parse(typeof(Permission), reader.GetString("Permission"))
                };

                if(!reader.IsDBNull(reader.GetOrdinal("IgnoreExpire"))) {
                    lastInfo.IgnoreExpire = reader.GetDateTime("IgnoreExpire");
                }

                lastInfo.Users.Add(guInfo);
            }

            reader.Close();

            if (lastInfo != null)
                result.Add(lastInfo);

            return result;
        }

        private List<GroupInfo> ReadToListGroupInfo(MySqlDataReader reader)
        {
            List<GroupInfo> result = new List<GroupInfo>();

            while (reader.Read())
            {
                GroupInfo group = new GroupInfo()
                {
                    Id = reader.GetUInt32("Id"),
                    Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? "Pouze já" : reader.GetString("Name"),
                    Id_Attachment = reader.GetUInt32("Id_Attachment"),
                    IgnoreExpire = reader.IsDBNull(reader.GetOrdinal("IgnoreExpire")) ? (DateTime?)null : reader.GetDateTime("IgnoreExpire"),
                    LastMessageSender = reader.IsDBNull(reader.GetOrdinal("sender")) ? null : reader.GetString("sender"),
                    LastMessageText = reader.IsDBNull(reader.GetOrdinal("TextBody")) ? null : reader.GetString("TextBody"),
                    LastMessageDate = reader.IsDBNull(reader.GetOrdinal("Sent")) ? (DateTime?)null : reader.GetDateTime("Sent"),
                    NewMessages = reader.GetInt32("NewMessages")
                };

                result.Add(group);
            }

            reader.Close();
            return result;
        }

        private List<Group> ReadToList(MySqlDataReader reader)
        {
            List<Group> result = new List<Group>();

            while (reader.Read())
            {
                Group group = new Group()
                {
                    Id = reader.GetUInt32("Id"),
                    GroupName = reader.IsDBNull(reader.GetOrdinal("GroupName")) ? null : reader.GetString("GroupName"),
                    HistoryVisibility = reader.GetBoolean("HistoryVisibility"),
                    Id_Attachment = reader.GetUInt32("Id_Attachment")
                };

                result.Add(group);
            }

            reader.Close();
            return result;
        }
    }
}