using asaq2.Model.DAL.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model.DAL
{
    class DAM_pos_orderlinesDAL
    {
        public static List<Dam_pos_orderline> GetForOrder(int id)
        {
            var result = new List<Dam_pos_orderline>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT pos_order_line.* FROM pos_order_line                                            
                                            WHERE orderid = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    result.Add(line);

                }
                dr.Close();
            }
            return result;
        }

        public static void Create(Dam_pos_orderline o, MySqlTransaction tr=null)
        {
            string insertSql= @"
                                INSERT INTO pos_order_line (linenum,orderid,web_unique,description,price,quantity)
                                VALUES(@linenum,@orderid,@web_unique,@description,@price,@quantity)    
                               ";
            var conn =  tr==null ?  new MySqlConnection(Settings.Default.ConnString):tr.Connection;
            if(tr==null)
                conn.Open();

            MySqlCommand cmd = Utils.GetCommand(insertSql, conn, tr);
            BuildSqlParameters(cmd, o);

            cmd.ExecuteNonQuery();
            cmd.CommandText="SELECT MAX(linenum)+1 FROM pos_order_line";
            o.linenum = Convert.ToInt32(cmd.ExecuteScalar());
            //cmd.CommandText = insertSql;

          
            //cmd.ExecuteNonQuery();
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Dam_pos_orderline o)
        {
            cmd.Parameters.AddWithValue("@linenum", o.linenum);
            cmd.Parameters.AddWithValue("@orderid", o.orderid);
            cmd.Parameters.AddWithValue("@web_unique", o.web_unique);
            cmd.Parameters.AddWithValue("@description", o.description);
            cmd.Parameters.AddWithValue("@price", o.price);
            cmd.Parameters.AddWithValue("@quantity", o.quantity);
        }

        private static Dam_pos_orderline GetFromDataReader(MySqlDataReader dr)
        {
            Dam_pos_orderline o = new Dam_pos_orderline();
            o.linenum= (int)dr["linenum"];
            o.orderid= Utilities.FromDbValue<int>(dr["orderid"]);
            o.web_unique =Utilities.FromDbValue<int>(dr["web_unique"]);
            o.description= string.Empty + Utilities.GetReaderField(dr, "description");
            o.price= Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "price"));
            o.quantity= Utilities.FromDbValue<int>(dr["quantity"]);
            return o;
           // throw new NotImplementedException();
        }

       

    }
}
