using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class User_manualDAL
    {
        public static List<User_manual> GetAll()
        {
            var result = new List<User_manual>();
            using(var conn= new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM user_manuals",conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    User_manual um = GetFromDataReader(dr);
                    um.UserManualType = User_manual_typesDAL.GetById(um.manual_type);
                    //result.Add(GetFromDataReader(dr));
                    result.Add(um);
                }
                dr.Close();
            }

            return result;
        }

        private static User_manual GetFromDataReader(MySqlDataReader dr)
        {
            User_manual o = new User_manual();

            o.id = (int) dr["id"];
            o.manual_type = (int) dr["manual_type"];
            o.version=(int) dr["version"];
            o.date_uploaded=Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"date_uploaded"));
            o.file_name=string.Empty + Utilities.GetReaderField(dr,"file_name");
            return o;
        }

        public static void Create(User_manual o)
        {
            string insertsql = @"INSERT INTO user_manuals(id,manual_type,version,date_uploaded,file_name) VALUES(@id,@manual_type,@version,@date_uploaded,@file_name)";
            using(var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(insertsql,conn);
                BuildSqlParam(cmd, o);
                cmd.ExecuteNonQuery();
            }
        }

        private static void BuildSqlParam(MySqlCommand cmd, User_manual o)
        {
            cmd.Parameters.AddWithValue("@id",o.id);
            cmd.Parameters.AddWithValue("@manual_type", o.manual_type);
            cmd.Parameters.AddWithValue("@version", o.version);
            cmd.Parameters.AddWithValue("@date_uploaded", o.date_uploaded);
            cmd.Parameters.AddWithValue("@file_name", o.file_name);
        }

       

     }
}
