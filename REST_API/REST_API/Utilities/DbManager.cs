using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Utilities
{
    public class DbManager
    {
        //Database=chat;Data Source=kufrko-rest-api-mysqldbserver.mysql.database.azure.com;User Id=kufrko@kufrko-rest-api-mysqldbserver;Password=mH$D62d)^*uNO*B*0gW5uIeL7
        private const string connectionString = "server=kufrko-rest-api-mysqldbserver.mysql.database.azure.com;uid=kufrko@kufrko-rest-api-mysqldbserver;pwd=mH$D62d)^*uNO*B*0gW5uIeL7;database=chat";
        private MySqlConnection conn;

        public DbManager()
        {
            this.conn = new MySqlConnection(connectionString);
            this.conn.Open();
        }

        public MySqlDataReader ExecuteReader(string sql, Dictionary<string, object> parameters)
        {
            return this.CreateCommand(sql, parameters).ExecuteReader();
        }

        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            return this.CreateCommand(sql, parameters).ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            return this.CreateCommand(sql, parameters).ExecuteScalar();
        }

        private MySqlCommand CreateCommand(string sql, Dictionary<string, object> parameters)
        {
            MySqlCommand cmd = new MySqlCommand(sql, this.conn);

            foreach (KeyValuePair<string, object> item in parameters)
            {
                cmd.Parameters.AddWithValue(item.Key, item.Value);
            }

            return cmd;
        }

        ~DbManager()
        {
            this.conn.Close();
        }
    }
}