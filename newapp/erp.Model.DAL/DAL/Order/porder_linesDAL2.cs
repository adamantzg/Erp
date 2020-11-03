using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Porder_linesDAL
    {
        public static List<Porder_lines> GetForOrderLine(int orderLineId, IDbConnection conn = null)
        {
            var result = new List<Porder_lines>();
            bool dispose = false;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }

            var cmd = Utils.GetCommand(@"SELECT porder_lines.*,mast_products.* FROM porder_lines INNER JOIN mast_products ON porder_lines.mast_id =mast_products.mast_id
                                        WHERE soline = @line_id", (MySqlConnection)conn);
            cmd.Parameters.AddWithValue("@line_id", orderLineId);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var line = GetFromDataReader(dr);
                line.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                result.Add(line);
            }
            dr.Close();
            if(dispose)
                conn.Dispose();
            return result;
        }

        public static List<Porder_lines> GetByProduct(int id)
        {
            var result = new List<Porder_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM porder_lines WHERE cprod_id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
    }
}
