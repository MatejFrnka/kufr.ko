using MySql.Data.MySqlClient;
using REST_API.Models.Api;
using REST_API.Models.Api.Message;
using REST_API.Models.Database;
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
        private UserRepository userRepository;
        private AttachmentRepository attachmentRepository;
        public MessageRepository(DbManager dbManager)
        {
            db = dbManager;
            userRepository = new UserRepository(db);
            attachmentRepository = new AttachmentRepository(db);
        }
        public Message FindById(ulong Id_Message)
        {
            string sql = "SELECT * FROM `Message` WHERE `Id` = @Id";

            List<Message> result = ReadToList(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id", Id_Message } }));
            return result.Count == 0 ? null : result[0];
        }

        public ulong SendMessage(uint Id_Sender, SendMessage message)
        {
            string sql = "INSERT INTO `Message`(`Id_User`, `Id_Group`, `Sent`, `TextBody`) VALUES (@Id_Sender, @Id_Group, @Sent, @Text);";

            db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_Sender", Id_Sender }, { "Id_Group", message.Id_Group }, { "Sent", DateTime.Now }, { "Text", message.Text } });
            ulong Id_Message = LastInsertedId();
            AddAttachments(message.Id_Attachment, Id_Message);

            return Id_Message;
        }
        public void AddAttachments(List<uint> Attachments, ulong Id_Message)
        {
            if (Attachments != null)
            {
                string sql = "INSERT INTO `Message_Attachment`(`Id_Message`, `Id_Attachment`) VALUES (@Id_Message, @Id_Attachment);";

                foreach (var Attachment in Attachments)
                {
                    db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_Message", Id_Message }, { "Id_Attachment", Attachment } });
                }
            }
        }
        public void SetMessageState(uint Id_Sender, ulong Id_Message, bool Seen)
        {
            string sql;
            uint group = GetMessageGroup(Id_Message);
            sql = "INSERT INTO `messagestate` (select m.Id as Id_Message, @IdUser as Id_User,0 as Seen from message as m left join messagestate as s on m.Id = s.Id_Message and s.Id_User = @IdUser " +
                "where m.Id_Group = @IdGroup and s.Id_Message is null and m.Id <= @IdMessage)";
            db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "IdGroup", group }, { "IdMessage", Id_Message }, { "IdUser", Id_Sender } });

            if (Seen)
            {
                sql = "UPDATE `messagestate` inner join message on message.Id = messagestate.Id_Message SET `Seen`=1 WHERE messagestate.Seen = 0 and messagestate.Id_User = @IdUser and message.Id_Group = @IdGroup and message.Id <= @IdMessage";
                db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "IdMessage", Id_Message }, { "@IdUser", Id_Sender }, {"@IdGroup", group } });
            }
            
        }
        public void EditMessage(EditMessage editMessage, uint User_Id)
        {
            if (editMessage.Text != null)
            {
                bool File = attachmentRepository.FindByMessageIdSecure(editMessage.Id_Message,User_Id).Count > 0;

                string sql = "INSERT INTO `MessageHistory` SELECT null as `Id`,  `Id` as `Id_Message`, `TextBody`, @File as `File`, now() as `ChangedTime` FROM `Message` WHERE `Id` = @Id_Message;";
                db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_Message", editMessage.Id_Message }, { "File", File } });

                sql = "UPDATE `Message` SET `TextBody`=@Text_Body, `Edited`=1 WHERE `Id` = @Id";
                db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id", editMessage.Id_Message }, { "Text_Body", editMessage.Text } });
            }
            if (editMessage.Id_Attachment != null)
            {
                string sql = "DELETE FROM `Message_Attachment` WHERE `Id_Message` = @Id_Message;";
                db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "Id_Message", editMessage.Id_Message } });
                this.AddAttachments(editMessage.Id_Attachment, editMessage.Id_Message);
            }
        }
        public List<MessageHistory> GetMessageHistory(ulong Id_Message)
        {
            string sql = "SELECT * FROM `MessageHistory` WHERE `Id_Message` = @Id_Message ORDER BY `ChangedTime` ASC";
            return ReadToMessageHistory(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Message", Id_Message } }));
        }
        public List<SingleMessage> GetMessages(ulong StartMessageId, uint Length, uint Id_Group, uint Id_Sender)
        {
            string sql;
            if (StartMessageId == 0)
            {
                sql = "SELECT `Id`, `Id_User`, `Id_Group`, `Sent`, `TextBody`,`Edited`, `Id_Attachment`  FROM `Message` " +
                   "LEFT JOIN `Message_Attachment` ON Message.Id = Message_Attachment.Id_Message " +
                   "WHERE `Message`.`Id_Group` = @Id_Group " +
                   "ORDER BY `Message`.`Id` DESC LIMIT @Length;";
            }
            else
            {
                sql = "SELECT `Id`, `Id_User`, `Id_Group`, `Sent`, `TextBody`,`Edited`, `Id_Attachment`  FROM `Message` " +
                    "LEFT JOIN `Message_Attachment` ON Message.Id = Message_Attachment.Id_Message " +
                    "WHERE `Message`.`Id_Group` = @Id_Group AND `Message`.`Id` < @StartId " +
                    "ORDER BY `Message`.`Id` DESC LIMIT @Length;";
            }
            List<SingleMessage> result = ReadToSingleMessage(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Group", Id_Group }, { "StartId", StartMessageId }, { "Length", Length } }), Id_Sender);
            return result;
        }
        public List<SingleMessage> GetNewMessages(DateTime OldestMessage, List<uint> FromGroups, uint Id_Sender)
        {
            string groups = "0";
            foreach (var item in FromGroups)
            {
                groups += " or `Id_Group` = " + item;
            }
            string sql = "SELECT `Id`, `Id_User`, `Id_Group`, `Sent`, `TextBody`,`Edited`, `Id_Attachment` FROM `Message` " +
                "LEFT JOIN `Message_Attachment` ON Message.Id = Message_Attachment.Id_Message " +
                "WHERE `Sent` >= @OldestDate and (" + groups + ") ";
            return ReadToSingleMessage(db.ExecuteReader(sql, new Dictionary<string, object>() { { "OldestDate", OldestMessage } }), Id_Sender);
        }
        public List<SingleMessage> GetNewMessages(ulong Id_Last, List<uint> FromGroups, uint Id_Sender)
        {
            string groups = "0";
            foreach (var item in FromGroups)
            {
                groups += " or `Id_Group` = " + item;
            }
            string sql = "SELECT `Id`, `Id_User`, `Id_Group`, `Sent`, `TextBody`,`Edited`, `Id_Attachment` FROM `Message` " +
                "LEFT JOIN `Message_Attachment` ON Message.Id = Message_Attachment.Id_Message " +
                "WHERE `Id` > @LastId and (" + groups + ") ";
            return ReadToSingleMessage(db.ExecuteReader(sql, new Dictionary<string, object>() { { "LastId", Id_Last} }), Id_Sender);
        }
        private uint GetMessageGroup(ulong Id_Message)
        {
            string sql = "SELECT `Id_Group` FROM `message` WHERE `Id` = @Id_Message";

            MySqlDataReader reader = db.ExecuteReader(sql, new Dictionary<string, object> { { "Id_Message", Id_Message } });
            uint? res = null;
            while (reader.Read())
            {
                res = reader.GetUInt32("Id_Group");
            }
            reader.Close();
            if(res == null)
                throw new Exception("No message with given id");
            return (uint)res;
        }
        //IMPORTANT: MySqlDataReader has to be ordered by Message.Id!
        private List<SingleMessage> ReadToSingleMessage(MySqlDataReader reader, uint Id_Sender)
        {
            List<Tuple<uint, SingleMessage>> messages = new List<Tuple<uint, SingleMessage>>();
            ulong prevId = 0;
            while (reader.Read())
            {
                ulong currId = reader.GetUInt64("Id");
                if (currId == prevId)
                {
                    messages.Last().Item2.Id_Attachment.Add(reader.GetUInt32("Id_Attachment"));
                }
                else
                {
                    SingleMessage item = new SingleMessage()
                    {
                        Id = currId,
                        Sent = reader.GetDateTime("Sent"),
                        Id_Group = reader.GetUInt32("Id_Group"),
                        Text = reader.IsDBNull(reader.GetOrdinal("TextBody"))?"" : reader.GetString("TextBody"),
                        Id_Attachment = new List<uint>(),
                        Edited = reader.GetBoolean("Edited"),
                        UserIsSender = reader.GetUInt32("Id_User") == Id_Sender
                    };
                    var attachment = reader["Id_Attachment"];
                    if (attachment != DBNull.Value)
                    {
                        item.Id_Attachment.Add(Convert.ToUInt32(attachment));
                    }
                    messages.Add(new Tuple<uint, SingleMessage>(reader.GetUInt32("Id_User"), item));
                }
                prevId = currId;
            }
            reader.Close();

            foreach (var item in messages)
            {
                item.Item2.UserInfo = userRepository.GetUserInfo(item.Item1, item.Item2.Id_Group);
            }
            List<SingleMessage> singleMessages = new List<SingleMessage>();
            messages.ForEach((item) => singleMessages.Add(item.Item2));
            return singleMessages;
        }
        private List<MessageHistory> ReadToMessageHistory(MySqlDataReader reader)
        {
            List<MessageHistory> result = new List<MessageHistory>();
            while (reader.Read())
            {
                result.Add(new MessageHistory
                {
                    Id = reader.GetUInt64("Id"),
                    Id_Message = reader.GetUInt64("Id_Message"),
                    TextBody = reader.GetString("TextBody"),
                    File = reader.GetBoolean("File"),
                    ChangedTime = reader.GetDateTime("ChangedTime")
                });
            }
            reader.Close();
            return result;
        }
        private List<Message> ReadToList(MySqlDataReader reader)
        {
            List<Message> result = new List<Message>();
            while (reader.Read())
            {
                result.Add(new Message()
                {
                    Id = reader.GetUInt64("Id"),
                    Id_User = reader.GetUInt32("Id_User"),
                    Id_Group = reader.GetUInt32("Id_Group"),
                    Edited = reader.GetBoolean("Edited"),
                    Sent = reader.GetDateTime("Sent"),
                    TextBody = reader.IsDBNull(reader.GetOrdinal("TextBody")) ? null : reader.GetString("TextBody"),
                });
            }

            reader.Close();
            return result;
        }
        private ulong LastInsertedId()
        {
            string sql = "SELECT LAST_INSERT_ID();";
            MySqlDataReader reader = db.ExecuteReader(sql, new Dictionary<string, object>());
            ulong Id_Message;
            if (reader.Read())
                Id_Message = reader.GetUInt64("LAST_INSERT_ID()");
            else
                throw new Exception("Could not read last Id from sql query");
            reader.Close();
            return Id_Message;
        }
        private bool? GetMessageState(uint Id_Sender, ulong Id_Message)
        {
            string sql = "SELECT * FROM `MessageState` WHERE `Id_Message` = @Id_Message and `Id_User` = @Id_User";
            MySqlDataReader reader = db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Message", Id_Message }, { "Id_User", Id_Sender } });
            bool? seen = null;
            if (reader.Read())
            {
                seen = reader.GetBoolean("Seen");
            }
            reader.Close();
            return seen;
        }

    }
}