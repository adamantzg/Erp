
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class Porder_linesDAL
	{
	
		public static List<Porder_lines> GetAll()
		{
			var result = new List<Porder_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM porder_lines", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}		
		
		public static Porder_lines GetById(int id)
		{
			Porder_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM porder_lines WHERE linenum = @id", conn);
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
		
	
		public static Porder_lines GetFromDataReader(MySqlDataReader dr)
		{
			Porder_lines o = new Porder_lines();
		
			o.linenum =  (int) dr["linenum"];
			o.porderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"porderid"));
			o.linedate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"linedate"));
			o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"cprod_id"));
			o.desc1 = string.Empty + Utilities.GetReaderField(dr,"desc1");
			o.orderqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"orderqty"));
			o.pending_orderqty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"pending_orderqty"));
			o.unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"unitprice"));
			o.pending_unitprice = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"pending_unitprice"));
			o.unitcurrency = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"unitcurrency"));
			o.linestatus = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"linestatus"));
			o.mast_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mast_id"));
			o.mfg_code = string.Empty + Utilities.GetReaderField(dr,"mfg_code");
			o.asaq_ref = string.Empty + Utilities.GetReaderField(dr,"asaq_ref");
			o.soline = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"soline"));
			o.lme = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"lme"));
			
			return o;

		}
		
		
		public static void Create(Porder_lines o)
        {
            string insertsql = @"INSERT INTO porder_lines (porderid,linedate,cprod_id,desc1,orderqty,pending_orderqty,unitprice,pending_unitprice,unitcurrency,linestatus,mast_id,mfg_code,asaq_ref,soline,lme) VALUES(@porderid,@linedate,@cprod_id,@desc1,@orderqty,@pending_orderqty,@unitprice,@pending_unitprice,@unitcurrency,@linestatus,@mast_id,@mfg_code,@asaq_ref,@soline,@lme)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT linenum FROM porder_lines WHERE linenum = LAST_INSERT_ID()";
                o.linenum = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Porder_lines o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@linenum", o.linenum);
			cmd.Parameters.AddWithValue("@porderid", o.porderid);
			cmd.Parameters.AddWithValue("@linedate", o.linedate);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@desc1", o.desc1);
			cmd.Parameters.AddWithValue("@orderqty", o.orderqty);
			cmd.Parameters.AddWithValue("@pending_orderqty", o.pending_orderqty);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@pending_unitprice", o.pending_unitprice);
			cmd.Parameters.AddWithValue("@unitcurrency", o.unitcurrency);
			cmd.Parameters.AddWithValue("@linestatus", o.linestatus);
			cmd.Parameters.AddWithValue("@mast_id", o.mast_id);
			cmd.Parameters.AddWithValue("@mfg_code", o.mfg_code);
			cmd.Parameters.AddWithValue("@asaq_ref", o.asaq_ref);
			cmd.Parameters.AddWithValue("@soline", o.soline);
			cmd.Parameters.AddWithValue("@lme", o.lme);
		}
		
		public static void Update(Porder_lines o)
		{
			string updatesql = @"UPDATE porder_lines SET porderid = @porderid,linedate = @linedate,cprod_id = @cprod_id,desc1 = @desc1,orderqty = @orderqty,pending_orderqty = @pending_orderqty,unitprice = @unitprice,pending_unitprice = @pending_unitprice,unitcurrency = @unitcurrency,linestatus = @linestatus,mast_id = @mast_id,mfg_code = @mfg_code,asaq_ref = @asaq_ref,soline = @soline,lme = @lme WHERE linenum = @linenum";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int linenum)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM porder_lines WHERE linenum = @id" , conn);
                cmd.Parameters.AddWithValue("@id", linenum);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			