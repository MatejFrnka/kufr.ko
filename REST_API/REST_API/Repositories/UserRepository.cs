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
    public class UserRepository
    {
        private DbManager db;

        public UserRepository(DbManager dbManager)
        {
            this.db = dbManager;
        }

        public User FindById(int id)
        {
            List<User> result = this.ReadToList(this.db.ExecuteReader("SELECT * FROM User WHERE Id = @id", new Dictionary<string, object>() { { "id", id } }));
            
            return result.FirstOrDefault();
        }

        public User FindByEmail(string email)
        {
            List<User> result = this.ReadToList(this.db.ExecuteReader("SELECT * FROM User WHERE Email = @mail", new Dictionary<string, object>() { { "mail", email } }));

            return result.FirstOrDefault();
        }

        public void CreateWithDefaults(User user)
        {
            string sql = "INSERT INTO User(Name, Email, Password) VALUES (@name,@email,@pass)";

            this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { {"name",user.Name },{"email",user.Email },{ "pass", user.Password } });
        }

        private List<User> ReadToList(MySqlDataReader reader)
        {
            List<User> result = new List<User>();

            while (reader.Read())
            {
                User user = new User()
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Email = reader.GetString("Email"),
                    Password = reader.GetString("Password"),
                    Created = reader.GetDateTime("Created"),
                    LastOnline = reader.IsDBNull(reader.GetOrdinal("LastOnline")) ? (DateTime?)null : reader.GetDateTime("LastOnline"),
                    Visibility = (Visibility)Enum.Parse(typeof(Visibility), reader.GetString("Visibility")),
                    Id_Attachment = reader.GetInt32("Id_Attachment")
                };

                result.Add(user);
            }

            reader.Close();
            return result;
        }
    }
}