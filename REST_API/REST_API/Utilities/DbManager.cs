using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Utilities
{
    public class DbManager
    {
        private const string connectionString = "server=mysqlstudenti.litv.sssvt.cz;uid=frnkamatej;pwd=123456;database=3b1_frnkamatej_db2";
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