using MySql.Data.MySqlClient;
using REST_API.Models.Database;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Repositories
{
    public class TokenRepository
    {
        private DbManager db;

        public TokenRepository(DbManager dbManager)
        {
            this.db = dbManager;
        }

        public Token FindById(uint id)
        {
            List<Token> result = this.ReadToList(this.db.ExecuteReader("SELECT * FROM Token WHERE Id = @id", new Dictionary<string, object>() { { "id", id } }));

            return result.FirstOrDefault();
        }

        public Token FindByValue(string value)
        {
            List<Token> result = this.ReadToList(this.db.ExecuteReader("SELECT * FROM Token WHERE Token = @val", new Dictionary<string, object>() { { "val", value } }));

            return result.FirstOrDefault();
        }

        public void Create(Token token)
        {
            string sql = "INSERT INTO Token(Id_User, Token, ExpireDate, Active) VALUES (@idu,@val,@date,@act)";

            this.db.ExecuteNonQuery(sql,new Dictionary<string, object>() { { "idu",token.Id_User}, { "val",token.Value}, { "date",token.ExpireDate}, { "act",token.Active} });
        }

        private List<Token> ReadToList(MySqlDataReader reader)
        {
            List<Token> result = new List<Token>();

            while (reader.Read())
            {
                Token token = new Token()
                {
                    Id = reader.GetUInt32("Id"),
                    Id_User = reader.GetUInt32("Id_User"),
                    Value = reader.GetString("Token"),
                    ExpireDate = reader.GetDateTime("ExpireDate"),
                    Active = reader.GetBoolean("Active")
                };

                result.Add(token);
            }

            reader.Close();
            return result;
        }
    }
}