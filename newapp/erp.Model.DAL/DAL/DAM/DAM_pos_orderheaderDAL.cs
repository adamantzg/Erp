using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using asaq2.Model.DAL.Properties;

namespace asaq2.Model.DAL
{
    public class DAM_pos_orderheaderDAL
    {
        

        public static Dam_pos_orderheader GetById(int id,bool includeLines=false)
        {
            Dam_pos_orderheader result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM pos_order_header WHERE orderid = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
                if(includeLines && result != null)
                    result.Lines = DAM_pos_orderlinesDAL.GetForOrder(id);
            }
            return result;

        }
        public static void Create(Dam_pos_orderheader o)
        {
            string insertSql = @"INSERT INTO pos_order_header(orderid,dam_user,orderdate,invoice_add1,invoice_add2,invoice_add3,invoice_add4,invoice_postcode,delivery_add1,delivery_add2, delivery_add3,delivery_add4, delivery_postcode,invoice_status)
                            VALUES(@orderid,@dam_user,@orderdate,@invoice_add1,@invoice_add2,@invoice_add3,@invoice_add4,@invoice_postcode,@delivery_add1,@delivery_add2,@delivery_add3,@delivery_add4,@delivery_postcode,@invoice_status)";
            var conn = new MySqlConnection(Settings.Default.ConnString);
                conn.Open();
            var tr = conn.BeginTransaction();
            try
            {
                var cmd = Utils.GetCommand("SELECT MAX(orderid)+1 FROM pos_order_header", conn, tr);
                object v = cmd.ExecuteScalar();//check if table is empty
                o.orderid = cmd.ExecuteScalar() == DBNull.Value ? 1: Convert.ToInt32( cmd.ExecuteScalar());
                cmd.CommandText = insertSql;
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                if (o.Lines != null)
                {
                    foreach (var line in o.Lines)
                    {
                        line.orderid = o.orderid;
                        DAM_pos_orderlinesDAL.Create(line, tr);
                    }
                }
                tr.Commit();
            }
            catch (Exception)
            {
                tr.Rollback();
            }
            finally
            {
                tr = null;
                conn.Close();
            }
        }

        private static Dam_pos_orderheader GetFromDataReader(MySqlDataReader dr)
        {
            Dam_pos_orderheader o = new Dam_pos_orderheader();
            o.orderid = (int)dr["orderid"];
            o.dam_user = Utilities.FromDbValue<int>(dr["dam_user"]);
            o.orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]);
            o.invoice_add1 = String.Empty + dr["invoice_add1"];
            o.invoice_add2 = String.Empty + dr["invoice_add2"];
            o.invoice_add3 = String.Empty + dr["invoice_add3"];
            o.invoice_add4 = String.Empty + dr["invoice_add4"];
            o.invoice_postcode = String.Empty + dr["invoice_postcode"];
            o.delivery_add1= String.Empty + dr["delivery_add1"];
            o.delivery_add2= String.Empty + dr["delivery_add2"];
            o.delivery_add3= String.Empty + dr["delivery_add3"];
            o.delivery_add4= String.Empty + dr["delivery_add4"];
            o.delivery_postcode = String.Empty + dr["delivery_postcode"];
            o.invoice_status = String.Empty + dr["invoice_status"];
            return o;
        }

        private static void BuildSqlParameters(MySqlCommand cmd, Dam_pos_orderheader o, bool forInsert=true)
        {
            cmd.Parameters.AddWithValue("@orderid",o.orderid);
            cmd.Parameters.AddWithValue("@dam_user", o.dam_user);
            cmd.Parameters.AddWithValue("@orderdate",o.orderdate);
            cmd.Parameters.AddWithValue("@invoice_add1", o.invoice_add1);
            cmd.Parameters.AddWithValue("@invoice_add2", o.invoice_add2);
            cmd.Parameters.AddWithValue("@invoice_add3", o.invoice_add3);
            cmd.Parameters.AddWithValue("@invoice_add4", o.invoice_add4);
            cmd.Parameters.AddWithValue("@invoice_postcode", o.invoice_postcode);
            cmd.Parameters.AddWithValue("@delivery_add1", o.delivery_add1);
            cmd.Parameters.AddWithValue("@delivery_add2", o.delivery_add2);
            cmd.Parameters.AddWithValue("@delivery_add3", o.delivery_add3);
            cmd.Parameters.AddWithValue("@delivery_add4", o.delivery_add4);
            cmd.Parameters.AddWithValue("@delivery_postcode", o.delivery_postcode);
            cmd.Parameters.AddWithValue("@invoice_status", o.invoice_status);
        }
    }
}
