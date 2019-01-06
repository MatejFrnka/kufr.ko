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
        public List<Attachment> FindByMessageId(ulong Id_Message)
        {
            string sql = "SELECT Id, Path, Mime FROM `Message_Attachment` INNER JOIN `Attachment` ON Message_Attachment.Id_Attachment = Attachment.Id WHERE Id_Message = @Id_Message";

            return this.ReadToList(db.ExecuteReader(sql, new Dictionary<string, object>() { {"Id_Message", Id_Message } }));
        }
        public Attachment FindByIdSecure(uint Id_Attachment,uint Id_User)
        {
            string sql = "SELECT a.Path, a.Mime FROM Attachment a INNER JOIN Message_Attachment ma on ma.Id_Attachment = a.Id INNER JOIN Message m on ma.Id_Message = m.Id INNER JOIN Group_User gu on gu.Id_Group = m.Id_Group WHERE a.Id = @Id_Attachment AND gu.Id_User = @Id_User";

            return this.ReadToObject(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_Attachment", Id_Attachment } }));
        }
        public uint FindIdByHash(string Hash)
        {
            string sql = "SELECT Id FROM Attachment WHERE Hash = @Hash";

            return (uint)db.ExecuteScalar(sql, new Dictionary<string, object>() { { "Hash", Hash } });
        }
        public uint CreateAttachment(Attachment attachment)
        {
            string sql = "INSERT INTO Attachment(Filename,Mime,Hash) @attachment.Filename, @attachment.Mime, @attachment.Hash; SELECT SCOPE_IDENTITY()";

            return (uint)this.db.ExecuteScalar(sql, new Dictionary<string, object>());

        }
        

        private List<Attachment> ReadToList(MySqlDataReader reader)
        {
            List<Attachment> result = new List<Attachment>();

            while (reader.Read())
            {
                result.Add(new Attachment()
                {
                    Id = reader.GetUInt32("Id"),
                    Mime = reader.GetString("Mime"),
                    Filename = reader.GetString("Filename"),
                    Hash = reader.GetString("Hash")
                });
            }
            reader.Close();

            return result;
        }
        
        private Attachment ReadToObject(MySqlDataReader reader)
        {
            Attachment result = new Attachment();
            result.Mime = reader.GetString("Mime");
            result.Filename = reader.GetString("Filename");
            result.Id = Convert.ToUInt32(reader.GetString("Id"));
            result.Hash = reader.GetString("Hash");
            reader.Close();
            return result;
        }

    }
}