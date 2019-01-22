using MySql.Data.MySqlClient;
using REST_API.Models.Api.Attachments;
using REST_API.Models.Database;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Repositories
{
    public class AttachmentRepository
    {
        private DbManager db;
        public AttachmentRepository(DbManager dbManager)
        {
            this.db = dbManager;
        }
        public List<AttachmentMessage> FindByMessageIdSecure(ulong Id_Message, uint Id_User)
        {
            string sql = "SELECT ma.Id_Attachment, ma.Filename, ma.Mime FROM Message_Attachment ma INNER JOIN Message m on ma.Id_Message = m.Id INNER JOIN Group_User gu on gu.Id_Group = m.Id_Group WHERE Id_Message = @Id_Message AND gu.Id_User = @Id_User";

            return this.ReadToListAM(db.ExecuteReader(sql, new Dictionary<string, object>() { {"Id_Message", Id_Message }, { "Id_User", Id_User } }));
        }
        public AttachmentMessage FindByPrimaryKeysSecure(ulong Id_Message,uint Id_Attachment, uint Id_User)
        {
            string sql = "SELECT ma.Filename, ma.Mime FROM Message_Attachment ma INNER JOIN Message m on ma.Id_Message = m.Id INNER JOIN Group_User gu on gu.Id_Group = m.Id_Group WHERE Id_Message = @Id_Message AND Id_Attachment = @Id_Attachment AND gu.Id_User = @Id_User";

            return this.ReadToObjectAM(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Message", Id_Message }, { "Id_User", Id_User }, { "Id_Attachment", Id_Attachment } }),Id_Attachment);
        }
        public Attachment FindByIdSecure(uint Id_Attachment,uint Id_User)
        {
            string sql = "SELECT a.Hash FROM Attachment a INNER JOIN Message_Attachment ma on ma.Id_Attachment = a.Id INNER JOIN Message m on ma.Id_Message = m.Id INNER JOIN Group_User gu on gu.Id_Group = m.Id_Group WHERE a.Id = @Id_Attachment AND gu.Id_User = @Id_User";

            return this.ReadToObject(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Attachment", Id_Attachment }, { "Id_User", Id_User } }), Id_Attachment); 
        }
        public Attachment FindById(uint Id_Attachment, uint Id_User)
        {
            string sql = "SELECT a.Hash FROM Attachment a WHERE a.Id = @Id_Attachment";

            return this.ReadToObject(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Attachment", Id_Attachment } }), Id_Attachment);
        }
        public uint? FindIdByHash(string Hash)
        {
            string sql = "SELECT Id FROM Attachment WHERE Hash = @Hash";

            return (uint?)db.ExecuteScalar(sql, new Dictionary<string, object>() { { "Hash", Hash } });
        }
        public uint CreateAttachment(string hash)
        {
            string sql = "INSERT INTO Attachment(Hash) VALUES (@hash); SELECT LAST_INSERT_ID();";

            return Convert.ToUInt32(this.db.ExecuteScalar(sql, new Dictionary<string, object>() { { "hash", hash } }));

        }
        

        private List<AttachmentMessage> ReadToListAM(MySqlDataReader reader)
        {
            List<AttachmentMessage> result = new List<AttachmentMessage>();

            while (reader.Read())
            {
                result.Add(new AttachmentMessage()
                {
                    Id_Attachment = reader.GetUInt32("Id"),
                    Mime = reader.GetString("Mime"),
                    Filename = reader.GetString("Filename")
                });
            }
            reader.Close();

            return result;
        }
        
        private Attachment ReadToObject(MySqlDataReader reader, uint Id)
        {
            Attachment result = new Attachment();
            result.Id = Id;
            result.Hash = reader.GetString("Hash");
            reader.Close();
            return result;
        }
        private AttachmentMessage ReadToObjectAM(MySqlDataReader reader, uint Id)
        {
            AttachmentMessage result = new AttachmentMessage();
            result.Id_Attachment = Id;
            result.Filename = reader.GetString("Filename");
            result.Mime = reader.GetString("Mime");
            reader.Close();
            return result;
        }

    }
}