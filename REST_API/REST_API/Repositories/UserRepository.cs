﻿using MySql.Data.MySqlClient;
using REST_API.Models.Database;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REST_API.Models.Enums;
using REST_API.Models.Api;

namespace REST_API.Repositories
{
    public class UserRepository
    {
        private DbManager db;

        public UserRepository(DbManager dbManager)
        {
            this.db = dbManager;
        }

        public User FindById(uint id)
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

            this.db.ExecuteNonQuery(sql, new Dictionary<string, object>() { { "name", user.Name }, { "email", user.Email }, { "pass", user.Password } });
        }
        public List<Group_User> GetGroups(uint Id_User)
        {
            string sql = "SELECT * FROM `Group_User` WHERE `Id_User` = @Id_User";

            return this.ReadToGroup_User(db.ExecuteReader(sql, new Dictionary<string, object>() { { "Id_User", Id_User } }));
        }

        public void UpdateLastOnline(uint id)
        {
            this.db.ExecuteNonQuery("UPDATE User SET LastOnline = NOW() WHERE Id = @id", new Dictionary<string, object>() { { "id", id } });
        }
        private List<User> ReadToList(MySqlDataReader reader)
        {
            List<User> result = new List<User>();

            while (reader.Read())
            {
                User user = new User()
                {
                    Id = reader.GetUInt32("Id"),
                    Name = reader.GetString("Name"),
                    Email = reader.GetString("Email"),
                    Password = reader.GetString("Password"),
                    Created = reader.GetDateTime("Created"),
                    LastOnline = reader.IsDBNull(reader.GetOrdinal("LastOnline")) ? (DateTime?)null : reader.GetDateTime("LastOnline"),
                    Visibility = (Visibility)Enum.Parse(typeof(Visibility), reader.GetString("Visibility")),
                    Id_Attachment = reader.GetUInt32("Id_Attachment")
                };

                result.Add(user);
            }

            reader.Close();
            return result;
        }
        private List<Group_User> ReadToGroup_User(MySqlDataReader reader)
        {
            List<Group_User> result = new List<Group_User>();

            while (reader.Read())
            {
                Group_User group_User = new Group_User()
                {
                    Id_User = reader.GetUInt32("Id_User"),
                    Id_Group = reader.GetUInt32("Id_Group"),
                    NickName = reader.IsDBNull(reader.GetOrdinal("Nickname")) ? null : reader.GetString("Nickname"),
                    Permission = (Permission)Enum.Parse(typeof(Permission), reader.GetString("Permission")),
                    IgnoreExpire = reader.IsDBNull(reader.GetOrdinal("IgnoreExpire")) ? (DateTime?)null : reader.GetDateTime("IgnoreExpire")
                };

                result.Add(group_User);
            }

            reader.Close();
            return result;
        }
    }
}