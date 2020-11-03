
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Stock_order_headerDAL
	{
	
		public static List<Stock_order_header> GetAll()
		{
			var result = new List<Stock_order_header>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT stock_order_header.*, factory.* FROM stock_order_header LEFT OUTER JOIN users factory ON stock_order_header.userid = factory.User_id", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Stock_order_header> GetByCriteria(DateTime? from, DateTime? to, int? factory_id)
        {
            var result = new List<Stock_order_header>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT stock_order_header.*, factory.* FROM stock_order_header 
                                            LEFT OUTER JOIN users factory ON stock_order_header.userid = factory.User_id
                                            WHERE (factory.user_id = @factory_id OR @factory_id IS NULL) AND
                                            (stock_order_header.orderdate >= @from OR @from IS NULL) AND
                                            (stock_order_header.orderdate <= @to OR @to IS NULL)", conn);
                cmd.Parameters.AddWithValue("@factory_id", factory_id);
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var so = GetFromDataReader(dr);
                    so.Factory = CompanyDAL.GetFromDataReader(dr);
                    result.Add(so);
                }
                dr.Close();
            }
            return result;
        }

        public static int GetHeaderSequence(int factory_id, DateTime date)
        {
            int result = 1;
            var factory = CompanyDAL.GetById(factory_id);
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(
                        "SELECT MAX(SUBSTRING_INDEX(poname,'-',-1)) FROM stock_order_header WHERE userid = @factory_id AND DATE(orderdate) = @date",
                        conn);
                cmd.Parameters.AddWithValue("@factory_id", factory_id);
                cmd.Parameters.AddWithValue("@date", date);
                var res = Utilities.FromDbValue<int>(cmd.ExecuteScalar());
                if (res != null)
                    result = res.Value+1;
            }
            return result;
        }


        public static Stock_order_header GetById(int id)
		{
			Stock_order_header result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM stock_order_header WHERE porderid = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Lines = Stock_order_linesDAL.GetForOrder(id);
                    if(result.from_id != null)
                        result.From = CompanyDAL.GetById(result.from_id.Value);
                    result.Factory = CompanyDAL.GetById(result.userid.Value);
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Stock_order_header GetFromDataReader(MySqlDataReader dr)
		{
			Stock_order_header o = new Stock_order_header();
		
			o.porderid =  (int) dr["porderid"];
			o.orderdate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"orderdate"));
			o.userid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"userid"));
			o.status = string.Empty + Utilities.GetReaderField(dr,"status");
			o.poname = string.Empty + Utilities.GetReaderField(dr,"poname");
			o.poadd1 = string.Empty + Utilities.GetReaderField(dr,"poadd1");
			o.poadd2 = string.Empty + Utilities.GetReaderField(dr,"poadd2");
			o.poadd3 = string.Empty + Utilities.GetReaderField(dr,"poadd3");
			o.poadd4 = string.Empty + Utilities.GetReaderField(dr,"poadd4");
			o.poadd5 = string.Empty + Utilities.GetReaderField(dr,"poadd5");
			o.poadd6 = string.Empty + Utilities.GetReaderField(dr,"poadd6");
			o.po_ready_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"po_ready_date"));
			o.po_reference = string.Empty + Utilities.GetReaderField(dr,"po_reference");
			o.instructions = string.Empty + Utilities.GetReaderField(dr,"instructions");
			o.FPI = string.Empty + Utilities.GetReaderField(dr,"FPI");
			o.process_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"process_id"));
			o.batch_inspection_line = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"batch_inspection_line"));
		    o.from_id = Utilities.FromDbValue<int>(dr["from_id"]);
			
			return o;

		}
		
		
		public static void Create(Stock_order_header o)
        {
            string insertsql = @"INSERT INTO stock_order_header (porderid,orderdate,userid,status,poname,poadd1,poadd2,poadd3,poadd4,poadd5,poadd6,po_ready_date,po_reference,instructions,FPI,process_id,batch_inspection_line,from_id) 
                                VALUES(@porderid,@orderdate,@userid,@status,@poname,@poadd1,@poadd2,@poadd3,@poadd4,@poadd5,@poadd6,@po_ready_date,@po_reference,@instructions,@FPI,@process_id,@batch_inspection_line,@from_id)";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            var tr = conn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand(insertsql,conn,tr);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT porderid FROM stock_order_header WHERE porderid = LAST_INSERT_ID()";
                o.porderid = (int)cmd.ExecuteScalar();

                foreach (var line in o.Lines)
                {
                    line.porderid = o.porderid;
                    Stock_order_linesDAL.Create(line, tr);
                }
                tr.Commit();
            }
            catch
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
                conn = null;
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Stock_order_header o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@porderid", o.porderid);
			cmd.Parameters.AddWithValue("@orderdate", o.orderdate);
			cmd.Parameters.AddWithValue("@userid", o.userid);
			cmd.Parameters.AddWithValue("@status", o.status);
			cmd.Parameters.AddWithValue("@poname", o.poname);
			cmd.Parameters.AddWithValue("@poadd1", o.poadd1);
			cmd.Parameters.AddWithValue("@poadd2", o.poadd2);
			cmd.Parameters.AddWithValue("@poadd3", o.poadd3);
			cmd.Parameters.AddWithValue("@poadd4", o.poadd4);
			cmd.Parameters.AddWithValue("@poadd5", o.poadd5);
			cmd.Parameters.AddWithValue("@poadd6", o.poadd6);
			cmd.Parameters.AddWithValue("@po_ready_date", o.po_ready_date);
			cmd.Parameters.AddWithValue("@po_reference", o.po_reference);
			cmd.Parameters.AddWithValue("@instructions", o.instructions);
			cmd.Parameters.AddWithValue("@FPI", o.FPI);
			cmd.Parameters.AddWithValue("@process_id", o.process_id);
			cmd.Parameters.AddWithValue("@batch_inspection_line", o.batch_inspection_line);
		    cmd.Parameters.AddWithValue("@from_id", o.from_id);
        }
		
		public static void Update(Stock_order_header o, List<Stock_order_lines> deletedLines )
		{
			string updatesql = @"UPDATE stock_order_header SET orderdate = @orderdate,userid = @userid,status = @status,poname = @poname,poadd1 = @poadd1,poadd2 = @poadd2,
                            poadd3 = @poadd3,poadd4 = @poadd4,poadd5 = @poadd5,poadd6 = @poadd6,po_ready_date = @po_ready_date,po_reference = @po_reference,
                        instructions = @instructions,FPI = @FPI,process_id = @process_id,batch_inspection_line = @batch_inspection_line,from_id = @from_id WHERE porderid = @porderid";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            var tr = conn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();

                if (o.Lines != null)
                {
                    foreach (var line in o.Lines)
                    {
                        if (line.linenum <= 0)
                        {
                            line.porderid = o.porderid;
                            Stock_order_linesDAL.Create(line, tr);
                        }
                        else
                        {
                            Stock_order_linesDAL.Update(line, tr);
                        }
                    }
                }
                foreach (var d in deletedLines)
                {
                    Stock_order_linesDAL.Delete(d.linenum, tr);
                }

                //Update lines
                tr.Commit();
            }
            catch
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
		}
		
		public static void Delete(int porderid)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM stock_order_header WHERE porderid = @id" , conn);
                cmd.Parameters.AddWithValue("@id", porderid);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			