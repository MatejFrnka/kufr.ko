using MySql.Data.MySqlClient;
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
        private List<Attachment> ReadToList(MySqlDataReader reader)
        {
            List<Attachment> result = new List<Attachment>();

            while (reader.Read())
            {
                result.Add(new Attachment()
                {
                    Id = reader.GetUInt32("Id"),
                    Mime = reader.GetString("Mime"),
                    Path = reader.GetString("Path")
                });
            }
            reader.Close();

            return result;
        }
    }
}