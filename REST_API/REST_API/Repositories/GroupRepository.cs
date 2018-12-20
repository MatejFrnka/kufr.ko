using MySql.Data.MySqlClient;
using REST_API.Models.Api.Group;
using REST_API.Models.Database;
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

        public List<GroupInfo> FindAllForUser(uint userId)
        {
            string sql = "SELECT g.Id,IF(g.GroupName IS NOT NULL,g.GroupName,(SELECT GROUP_CONCAT(IF(gu.Nickname IS NULL, uu.Name, gu.Nickname) SEPARATOR ', ') FROM Group_User gu INNER JOIN User uu ON uu.Id = gu.Id_User WHERE gu.Id_Group = g.Id)) AS Name, g.Id_Attachment,IF(senderG.Nickname IS NULL, senderU.Name, senderG.Nickname) AS sender, m.TextBody,m.Sent " +
                         "FROM `Group` g INNER JOIN Group_User u ON g.Id = u.Id_Group LEFT JOIN (SELECT gd.Id, MAX(m.Id) as mId FROM `Group` gd LEFT JOIN Message m ON m.Id_Group = gd.Id GROUP BY gd.Id) a ON a.Id = g.Id LEFT JOIN Message m ON a.mId = m.Id LEFT JOIN Group_User senderG ON senderG.Id_User = m.Id_User AND senderG.Id_Group = g.ID LEFT JOIN User senderU ON senderU.Id = m.Id_User WHERE u.Id_User = @uid ORDER BY a.mId DESC";

            return this.ReadToListGroupInfo(this.db.ExecuteReader(sql, new Dictionary<string, object>() { { "uid", userId } }));

            //return this.ReadToList(this.db.ExecuteReader("SELECT g.* FROM `Group` g INNER JOIN Group_User u ON g.Id = u.Id_Group LEFT JOIN Message m ON m.Id_Group = g.Id WHERE u.Id_User = @uid GROUP BY(g.Id) ORDER BY MAX(m.Id) DESC", new Dictionary<string, object>() { { "uid", userId } }));
        }

        private List<GroupInfo> ReadToListGroupInfo(MySqlDataReader reader)
        {
            List<GroupInfo> result = new List<GroupInfo>();

            while (reader.Read())
            {
                GroupInfo group = new GroupInfo()
                {
                    Id = reader.GetUInt32("Id"),
                    Name = reader.GetString("Name"),
                    Id_Attachment = reader.GetUInt32("Id_Attachment"),
                    LastMessageSender = reader.IsDBNull(reader.GetOrdinal("sender")) ? null : reader.GetString("sender"),
                    LastMessageText = reader.IsDBNull(reader.GetOrdinal("TextBody")) ? null : reader.GetString("TextBody"),
                    LastMessageDate = reader.IsDBNull(reader.GetOrdinal("Sent")) ? (DateTime?) null : reader.GetDateTime("Sent")
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