using MySql.Data.MySqlClient;
using REST_API.Models.Api.Message;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Repositories
{
    public class MessageRepository
    {
        private DbManager db;
        public MessageRepository(DbManager dbManager)
        {
            db = dbManager;
        }
        public void SendMessage(uint Id_Sender, SendMessage message)
        {
            string sql = "INSERT INTO `Message`(`Id_User`, `Id_Group`, `Sent`, `TextBody`) VALUES (@Id_Sender, @Id_Group, @Sent, @Text);" +
                            "SELECT LAST_INSERT_ID();";

            MySqlDataReader reader = db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Sender", Id_Sender }, { "Id_Group", message.Id_Group }, { "Sent", DateTime.Now }, { "Text", message.Text } });
            uint Id_Message;
            if (reader.Read())
            {
                Id_Message = reader.GetUInt32("LAST_INSERT_ID()");
            }
            else
            {
                throw new Exception("Could not read last Id from sql query");
            }
            reader.Close();
            sql = "INSERT INTO `Message_Attachment`(`Id_Message`, `Id_Attachment`) VALUES (@Id_Message, @Id_Attachment);";

            foreach (var Attachment in message.Id_Attachment)
            {
                db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_Message", Id_Message }, { "Id_Attachment", Attachment } });
            }
        }
        public void SetMessageState(int Id_Sender, SetMessageState messageState)
        {
            string sql;
            sql = "SELECT * FROM `MessageState` WHERE `Id_Message` = @Id_Message and `Id_User` = @Id_User";
            MySqlDataReader reader = db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Message", messageState.Id_Message }, { "Id_User", Id_Sender } });
            bool seen = false;
            bool exists = false;
            if (reader.Read())
            {
                seen = reader.GetBoolean("Seen");
                exists = true;
            }
            reader.Close();
            if ((seen && !messageState.Seen) || (seen == messageState.Seen && exists))
            {
                return;
            }
            if (exists)
            {
                sql = "UPDATE `MessageState` SET `Seen`= 1 WHERE `Id_Message`= @Id_Message and `Id_User`= @Id_User";
            }
            else
            {
                sql = "INSERT INTO `MessageState`(`Id_Message`, `Id_User`, `Seen`) VALUES (@Id_Message, @Id_User, @Seen)";
            }
            db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_Message", messageState.Id_Message }, { "Id_User", Id_Sender }, { "Seen", messageState.Seen } });
        }
        public List<SingleMessage> GetMessages(ulong StartMessageId, uint Length, uint Id_Group)
        {
            string sql = "SELECT `Id`, `Id_User`, `Id_Group`, `Sent`, `TextBody`,`Id_Attachment`  FROM `Message` " +
                "LEFT JOIN `Message_Attachment` ON Message.Id = Message_Attachment.Id_Message " +
                "WHERE `Message`.`Id_Group` = 1 AND `Message`.`Id` >= @StartId " +
                "ORDER BY `Message`.`Id` DESC LIMIT 25;";
            List<SingleMessage> result = ReadToSingleMessage(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Group", Id_Group }, { "StartId", StartMessageId }, { "Length", Length } }));
            return result;
        }
        private List<SingleMessage> ReadToSingleMessage(MySqlDataReader reader)
        {
            List<SingleMessage> result = new List<SingleMessage>();
            ulong prevId = 0;
            while (reader.Read())
            {
                ulong currId = reader.GetUInt64("Id");
                if (currId == prevId)
                {
                    result.Last().Id_Attachment.Add(reader.GetUInt32("Id_Attachment"));
                }
                else
                {
                    SingleMessage item = new SingleMessage()
                    {
                        Id_UserSender = reader.GetUInt32("Id_User"),
                        Sent = reader.GetDateTime("Sent"),
                        Id_Group = reader.GetUInt32("Id_Group"),
                        Text = reader.GetString("TextBody"),
                        Id_Attachment = new List<uint>()
                    };
                    var attachment = reader["Id_Attachment"];
                    if (attachment != DBNull.Value)
                    {
                        item.Id_Attachment.Add(Convert.ToUInt32(attachment));
                    }
                    result.Add(item);
                }
                prevId = currId;
            }
            reader.Close();
            return result;
        }
    }
}