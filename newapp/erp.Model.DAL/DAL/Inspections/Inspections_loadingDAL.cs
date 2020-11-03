
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

namespace erp.Model.DAL
{
    public class Inspections_loadingDAL
	{
	
		public static List<Inspections_loading> GetAll()
		{
			var result = new List<Inspections_loading>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspections_loading", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspections_loading> GetForInspection(int insp_id, IDbConnection conn = null)
        {
            return GetForInspection(new[] { insp_id }, conn);
        }

        public static List<Inspections_loading> GetForInspection(IList<int> insp_ids, IDbConnection conn = null)
        {
            var result = new List<Inspections_loading>();
            var dispose = conn == null;
            if (conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }
            try
            {

                var cmd = new MySqlCommand("", (MySqlConnection)conn);
                var criteria = Utils.CreateParametersFromIdList(cmd, insp_ids);
                cmd.CommandText =
                    $@"SELECT inspections_loading.*,inspection_lines_tested.*, containers.*  FROM inspections_loading INNER JOIN inspection_lines_tested 
                                             ON inspections_loading.insp_line_unique = inspection_lines_tested.insp_line_unique 
                                            INNER JOIN containers ON inspections_loading.container = containers.container_id 
                                WHERE containers.insp_id IN ({criteria}) AND inspection_lines_tested.insp_id IN ({criteria})";


                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var loading = GetFromDataReader(dr);
                    loading.Container = ContainersDAL.GetFromDataReader(dr);
                    result.Add(loading);
                }
                dr.Close();
            }
            finally
            {
                if (dispose)
                    conn.Dispose();
            }
            return result;
        }


        public static Inspections_loading GetById(int id)
		{
			Inspections_loading result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspections_loading WHERE loading_line_unique = @id", conn);
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
		
	
		private static Inspections_loading GetFromDataReader(MySqlDataReader dr)
		{
			Inspections_loading o = new Inspections_loading();
		
			o.loading_line_unique =  (int) dr["loading_line_unique"];
			o.insp_line_unique = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_line_unique"));
			o.container = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"container"));
			o.qty_per_pallet = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"qty_per_pallet"));
			o.full_pallets = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"full_pallets"));
			o.loose_load_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"loose_load_qty"));
			o.mixed_pallet_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mixed_pallet_qty"));
			o.mixed_pallet_qty2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mixed_pallet_qty2"));
			o.mixed_pallet_qty3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"mixed_pallet_qty3"));

		    if (Utilities.ColumnExists(dr, "insp_client_ref"))
		        o.insp_client_ref = string.Empty + dr["insp_client_ref"];
            if (Utilities.ColumnExists(dr, "insp_custpo"))
                o.insp_custpo = string.Empty + dr["insp_custpo"];
			
			return o;

		}
		
		
		public static void Create(Inspections_loading o)
        {
            string insertsql = @"INSERT INTO inspections_loading (insp_line_unique,container,qty_per_pallet,full_pallets,loose_load_qty,mixed_pallet_qty,mixed_pallet_qty2,mixed_pallet_qty3) VALUES(@insp_line_unique,@container,@qty_per_pallet,@full_pallets,@loose_load_qty,@mixed_pallet_qty,@mixed_pallet_qty2,@mixed_pallet_qty3)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT loading_line_unique FROM inspections_loading WHERE loading_line_unique = LAST_INSERT_ID()";
                o.loading_line_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspections_loading o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@loading_line_unique", o.loading_line_unique);
			cmd.Parameters.AddWithValue("@insp_line_unique", o.insp_line_unique);
			cmd.Parameters.AddWithValue("@container", o.container);
			cmd.Parameters.AddWithValue("@qty_per_pallet", o.qty_per_pallet);
			cmd.Parameters.AddWithValue("@full_pallets", o.full_pallets);
			cmd.Parameters.AddWithValue("@loose_load_qty", o.loose_load_qty);
			cmd.Parameters.AddWithValue("@mixed_pallet_qty", o.mixed_pallet_qty);
			cmd.Parameters.AddWithValue("@mixed_pallet_qty2", o.mixed_pallet_qty2);
			cmd.Parameters.AddWithValue("@mixed_pallet_qty3", o.mixed_pallet_qty3);
		}
		
		public static void Update(Inspections_loading o)
		{
			string updatesql = @"UPDATE inspections_loading SET insp_line_unique = @insp_line_unique,container = @container,qty_per_pallet = @qty_per_pallet,full_pallets = @full_pallets,loose_load_qty = @loose_load_qty,mixed_pallet_qty = @mixed_pallet_qty,mixed_pallet_qty2 = @mixed_pallet_qty2,mixed_pallet_qty3 = @mixed_pallet_qty3 WHERE loading_line_unique = @loading_line_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int loading_line_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM inspections_loading WHERE loading_line_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", loading_line_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			