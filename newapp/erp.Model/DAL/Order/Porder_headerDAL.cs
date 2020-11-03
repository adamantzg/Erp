
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class Porder_headerDAL
	{
	
		public static List<Porder_header> GetAll()
		{
			List<Porder_header> result = new List<Porder_header>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM porder_header", conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Porder_header GetById(int id)
		{
			Porder_header result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM porder_header WHERE porderid = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
                dr.Close();
            }
			return result;
		}
	
		private static Porder_header GetFromDataReader(MySqlDataReader dr)
		{
			Porder_header o = new Porder_header();
		
			o.porderid =  (int) dr["porderid"];
			o.orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]);
			o.userid = Utilities.FromDbValue<int>(dr["userid"]);
			o.locid = Utilities.FromDbValue<int>(dr["locid"]);
			o.status = string.Empty + dr["status"];
			o.po_status = string.Empty + dr["po_status"];
			o.delivery_address1 = string.Empty + dr["delivery_address1"];
			o.delivery_address2 = string.Empty + dr["delivery_address2"];
			o.delivery_address3 = string.Empty + dr["delivery_address3"];
			o.delivery_address4 = string.Empty + dr["delivery_address4"];
			o.delivery_address5 = string.Empty + dr["delivery_address5"];
			o.delivery_address6 = string.Empty + dr["delivery_address6"];
			o.delivery_address7 = string.Empty + dr["delivery_address7"];
			o.currency = Utilities.FromDbValue<int>(dr["currency"]);
			o.poname = string.Empty + dr["poname"];
			o.poadd1 = string.Empty + dr["poadd1"];
			o.poadd2 = string.Empty + dr["poadd2"];
			o.poadd3 = string.Empty + dr["poadd3"];
			o.poadd4 = string.Empty + dr["poadd4"];
			o.poadd5 = string.Empty + dr["poadd5"];
			o.poadd6 = string.Empty + dr["poadd6"];
			o.soorderid = Utilities.FromDbValue<int>(dr["soorderid"]);
			o.po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]);
			o.original_po_req_etd = Utilities.FromDbValue<DateTime>(dr["original_po_req_etd"]);
			o.pending_po_req_etd = Utilities.FromDbValue<DateTime>(dr["pending_po_req_etd"]);
			o.po_ready_date = Utilities.FromDbValue<DateTime>(dr["po_ready_date"]);
			o.po_ready = Utilities.FromDbValue<int>(dr["po_ready"]);
			o.po_reference = string.Empty + dr["po_reference"];
			o.instructions = string.Empty + dr["instructions"];
			o.po_cfm_etd = Utilities.FromDbValue<DateTime>(dr["po_cfm_etd"]);
			o.FPI = string.Empty + dr["FPI"];
			o.process_id = Utilities.FromDbValue<int>(dr["process_id"]);
			o.system_cbm = Utilities.FromDbValue<double>(dr["system_cbm"]);
			o.system_sqm = Utilities.FromDbValue<double>(dr["system_sqm"]);
			o.factory_cbm = Utilities.FromDbValue<double>(dr["factory_cbm"]);
			o.factory_sqm = Utilities.FromDbValue<double>(dr["factory_sqm"]);
			o.comments = string.Empty + dr["comments"];
			o.comments_factory = string.Empty + dr["comments_factory"];
			o.special_comments = string.Empty + dr["special_comments"];
			o.fi_reviewed = Utilities.FromDbValue<int>(dr["fi_reviewed"]);
			o.li_reviewed = Utilities.FromDbValue<int>(dr["li_reviewed"]);
			o.batch_inspection = Utilities.FromDbValue<int>(dr["batch_inspection"]);
			o.invoices_paid = Utilities.FromDbValue<int>(dr["invoices_paid"]);
			o.invoices_bl = Utilities.FromDbValue<int>(dr["invoices_bl"]);
			o.original_process_id = Utilities.FromDbValue<int>(dr["original_process_id"]);
			
			return o;

		}
		
		public static void Create(Porder_header o)
        {
            string insertsql = @"INSERT INTO porder_header (porderid,orderdate,userid,locid,status,po_status,delivery_address1,delivery_address2,delivery_address3,delivery_address4,delivery_address5,delivery_address6,delivery_address7,currency,poname,poadd1,poadd2,poadd3,poadd4,poadd5,poadd6,soorderid,po_req_etd,original_po_req_etd,pending_po_req_etd,po_ready_date,po_ready,po_reference,instructions,po_cfm_etd,FPI,process_id,system_cbm,system_sqm,factory_cbm,factory_sqm,comments,comments_factory,special_comments,fi_reviewed,li_reviewed,batch_inspection,invoices_paid,invoices_bl,original_process_id) VALUES(@porderid,@orderdate,@userid,@locid,@status,@po_status,@delivery_address1,@delivery_address2,@delivery_address3,@delivery_address4,@delivery_address5,@delivery_address6,@delivery_address7,@currency,@poname,@poadd1,@poadd2,@poadd3,@poadd4,@poadd5,@poadd6,@soorderid,@po_req_etd,@original_po_req_etd,@pending_po_req_etd,@po_ready_date,@po_ready,@po_reference,@instructions,@po_cfm_etd,@FPI,@process_id,@system_cbm,@system_sqm,@factory_cbm,@factory_sqm,@comments,@comments_factory,@special_comments,@fi_reviewed,@li_reviewed,@batch_inspection,@invoices_paid,@invoices_bl,@original_process_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(porderid)+1 FROM porder_header", conn);
                o.porderid = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Porder_header o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@porderid", o.porderid);
			cmd.Parameters.AddWithValue("@orderdate", o.orderdate);
			cmd.Parameters.AddWithValue("@userid", o.userid);
			cmd.Parameters.AddWithValue("@locid", o.locid);
			cmd.Parameters.AddWithValue("@status", o.status);
			cmd.Parameters.AddWithValue("@po_status", o.po_status);
			cmd.Parameters.AddWithValue("@delivery_address1", o.delivery_address1);
			cmd.Parameters.AddWithValue("@delivery_address2", o.delivery_address2);
			cmd.Parameters.AddWithValue("@delivery_address3", o.delivery_address3);
			cmd.Parameters.AddWithValue("@delivery_address4", o.delivery_address4);
			cmd.Parameters.AddWithValue("@delivery_address5", o.delivery_address5);
			cmd.Parameters.AddWithValue("@delivery_address6", o.delivery_address6);
			cmd.Parameters.AddWithValue("@delivery_address7", o.delivery_address7);
			cmd.Parameters.AddWithValue("@currency", o.currency);
			cmd.Parameters.AddWithValue("@poname", o.poname);
			cmd.Parameters.AddWithValue("@poadd1", o.poadd1);
			cmd.Parameters.AddWithValue("@poadd2", o.poadd2);
			cmd.Parameters.AddWithValue("@poadd3", o.poadd3);
			cmd.Parameters.AddWithValue("@poadd4", o.poadd4);
			cmd.Parameters.AddWithValue("@poadd5", o.poadd5);
			cmd.Parameters.AddWithValue("@poadd6", o.poadd6);
			cmd.Parameters.AddWithValue("@soorderid", o.soorderid);
			cmd.Parameters.AddWithValue("@po_req_etd", o.po_req_etd);
			cmd.Parameters.AddWithValue("@original_po_req_etd", o.original_po_req_etd);
			cmd.Parameters.AddWithValue("@pending_po_req_etd", o.pending_po_req_etd);
			cmd.Parameters.AddWithValue("@po_ready_date", o.po_ready_date);
			cmd.Parameters.AddWithValue("@po_ready", o.po_ready);
			cmd.Parameters.AddWithValue("@po_reference", o.po_reference);
			cmd.Parameters.AddWithValue("@instructions", o.instructions);
			cmd.Parameters.AddWithValue("@po_cfm_etd", o.po_cfm_etd);
			cmd.Parameters.AddWithValue("@FPI", o.FPI);
			cmd.Parameters.AddWithValue("@process_id", o.process_id);
			cmd.Parameters.AddWithValue("@system_cbm", o.system_cbm);
			cmd.Parameters.AddWithValue("@system_sqm", o.system_sqm);
			cmd.Parameters.AddWithValue("@factory_cbm", o.factory_cbm);
			cmd.Parameters.AddWithValue("@factory_sqm", o.factory_sqm);
			cmd.Parameters.AddWithValue("@comments", o.comments);
			cmd.Parameters.AddWithValue("@comments_factory", o.comments_factory);
			cmd.Parameters.AddWithValue("@special_comments", o.special_comments);
			cmd.Parameters.AddWithValue("@fi_reviewed", o.fi_reviewed);
			cmd.Parameters.AddWithValue("@li_reviewed", o.li_reviewed);
			cmd.Parameters.AddWithValue("@batch_inspection", o.batch_inspection);
			cmd.Parameters.AddWithValue("@invoices_paid", o.invoices_paid);
			cmd.Parameters.AddWithValue("@invoices_bl", o.invoices_bl);
			cmd.Parameters.AddWithValue("@original_process_id", o.original_process_id);
		}
		
		public static void Update(Porder_header o)
		{
			string updatesql = @"UPDATE porder_header SET orderdate = @orderdate,userid = @userid,locid = @locid,status = @status,po_status = @po_status,delivery_address1 = @delivery_address1,delivery_address2 = @delivery_address2,delivery_address3 = @delivery_address3,delivery_address4 = @delivery_address4,delivery_address5 = @delivery_address5,delivery_address6 = @delivery_address6,delivery_address7 = @delivery_address7,currency = @currency,poname = @poname,poadd1 = @poadd1,poadd2 = @poadd2,poadd3 = @poadd3,poadd4 = @poadd4,poadd5 = @poadd5,poadd6 = @poadd6,soorderid = @soorderid,po_req_etd = @po_req_etd,original_po_req_etd = @original_po_req_etd,pending_po_req_etd = @pending_po_req_etd,po_ready_date = @po_ready_date,po_ready = @po_ready,po_reference = @po_reference,instructions = @instructions,po_cfm_etd = @po_cfm_etd,FPI = @FPI,process_id = @process_id,system_cbm = @system_cbm,system_sqm = @system_sqm,factory_cbm = @factory_cbm,factory_sqm = @factory_sqm,comments = @comments,comments_factory = @comments_factory,special_comments = @special_comments,fi_reviewed = @fi_reviewed,li_reviewed = @li_reviewed,batch_inspection = @batch_inspection,invoices_paid = @invoices_paid,invoices_bl = @invoices_bl,original_process_id = @original_process_id WHERE porderid = @porderid";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int porderid)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM porder_header WHERE porderid = @id" , conn);
                cmd.Parameters.AddWithValue("@id", porderid);
                cmd.ExecuteNonQuery();
            }
		}
	}
}
			
			