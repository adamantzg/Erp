using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.DAL
{
    public class User_manual_typesDAL
    {
        public static List<User_manual_types> GetAll()
        {
            var result = new List<User_manual_types>();
            using( var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM user_manual_types",conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static User_manual_types GetById(int id)
        {
            User_manual_types result =null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM user_manual_types WHERE id=@id", conn);
                
                cmd.Parameters.AddWithValue("@id", id);

                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
            return result;
        }

        private static User_manual_types GetFromDataReader(MySqlDataReader dr)
        {
            User_manual_types o = new User_manual_types();
            o.id = (int)dr["id"];
            o.description = string.Empty + Utilities.GetReaderField(dr, "description");

            return o;
        }
    }
}
