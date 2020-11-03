
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public partial class ShipmentsDAL
	{
	
		public static List<Shipments> GetAll()
		{
			var result = new List<Shipments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM shipments", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Shipments> GetForOrder(int orderid)
        {
            var result = new List<Shipments>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM shipments WHERE orderid = @orderid", conn);
                cmd.Parameters.AddWithValue("@orderid", orderid);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Shipments GetById(int id)
		{
			Shipments result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM shipments WHERE ship_id = @id", conn);
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


		
	
		public static Shipments GetFromDataReader(MySqlDataReader dr)
		{
			Shipments o = new Shipments();
		
			o.ship_id =  (int) dr["ship_id"];
			o.orderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"orderid"));
			o.poid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"poid"));
			o.packing_items = string.Empty + Utilities.GetReaderField(dr,"packing_items");
			o.shipped_from = string.Empty + Utilities.GetReaderField(dr,"shipped_from");
			o.shipper_per = string.Empty + Utilities.GetReaderField(dr,"shipper_per");
			o.sailing_date = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"sailing_date"));
			o.shipped_by = string.Empty + Utilities.GetReaderField(dr,"shipped_by");
			o.container_no = string.Empty + Utilities.GetReaderField(dr,"container_no");
			o.notes = string.Empty + Utilities.GetReaderField(dr,"notes");
			o.cbm = string.Empty + Utilities.GetReaderField(dr,"cbm");
			o.qty_type_container = string.Empty + Utilities.GetReaderField(dr,"qty_type_container");
			o.gross_weight = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"gross_weight"));
			o.forwarder = string.Empty + Utilities.GetReaderField(dr,"forwarder");
			
			return o;

		}
		
		
		public static void Create(Shipments o)
        {
            string insertsql = @"INSERT INTO shipments (orderid,poid,packing_items,shipped_from,shipper_per,sailing_date,shipped_by,container_no,notes,cbm,qty_type_container,gross_weight,forwarder) VALUES(@orderid,@poid,@packing_items,@shipped_from,@shipper_per,@sailing_date,@shipped_by,@container_no,@notes,@cbm,@qty_type_container,@gross_weight,@forwarder)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT ship_id FROM shipments WHERE ship_id = LAST_INSERT_ID()";
                o.ship_id = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Shipments o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@ship_id", o.ship_id);
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@poid", o.poid);
			cmd.Parameters.AddWithValue("@packing_items", o.packing_items);
			cmd.Parameters.AddWithValue("@shipped_from", o.shipped_from);
			cmd.Parameters.AddWithValue("@shipper_per", o.shipper_per);
			cmd.Parameters.AddWithValue("@sailing_date", o.sailing_date);
			cmd.Parameters.AddWithValue("@shipped_by", o.shipped_by);
			cmd.Parameters.AddWithValue("@container_no", o.container_no);
			cmd.Parameters.AddWithValue("@notes", o.notes);
			cmd.Parameters.AddWithValue("@cbm", o.cbm);
			cmd.Parameters.AddWithValue("@qty_type_container", o.qty_type_container);
			cmd.Parameters.AddWithValue("@gross_weight", o.gross_weight);
			cmd.Parameters.AddWithValue("@forwarder", o.forwarder);
		}
		
		public static void Update(Shipments o)
		{
			string updatesql = @"UPDATE shipments SET orderid = @orderid,poid = @poid,packing_items = @packing_items,shipped_from = @shipped_from,shipper_per = @shipper_per,sailing_date = @sailing_date,shipped_by = @shipped_by,container_no = @container_no,notes = @notes,cbm = @cbm,qty_type_container = @qty_type_container,gross_weight = @gross_weight,forwarder = @forwarder WHERE ship_id = @ship_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int ship_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM shipments WHERE ship_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", ship_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			