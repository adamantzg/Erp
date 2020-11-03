
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Instructions.Model
{
    public class DetailDAL
	{
	
		public static List<Detail> GetAll()
		{
			var result = new List<Detail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT * FROM detail", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Detail> GetForSection(int section_id)
        {
            var result = new List<Detail>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT detail.*, flag.* FROM detail LEFT OUTER JOIN flag ON detail.flag_id = flag.flag_id WHERE section_id = @section_id", conn);
                cmd.Parameters.AddWithValue("@section_id", section_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var detail = GetFromDataReader(dr);
                    if(detail.flag_id != null)
                        detail.Flag = FlagDAL.GetById(detail.flag_id.Value);
                    result.Add(detail);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Detail GetById(int id)
		{
			Detail result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utilities.GetCommand("SELECT detail.*, section.heading AS section_heading FROM detail INNER JOIN section ON detail.section_id = section.section_id WHERE detail_id = @id", conn);
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
		
	
		private static Detail GetFromDataReader(MySqlDataReader dr)
		{
			Detail o = new Detail();
		
			o.detail_id =  (int) dr["detail_id"];
			o.section_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"section_id"));
			o.detail = string.Empty + Utilities.GetReaderField(dr,"detail");
			o.sequence = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"sequence"));
			o.flag_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"flag_id"));
		    if (Utilities.ColumnExists(dr, "section_heading"))
		        o.section_heading = string.Empty + dr["section_heading"];
			
			return o;

		}
		
		
		public static void Create(Detail o)
        {
            string insertsql = @"INSERT INTO detail (detail_id,section_id,detail,sequence,flag_id) VALUES(@detail_id,@section_id,@detail,@sequence,@flag_id)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                MySqlCommand cmd = Utilities.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd, o);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "SELECT detail_id FROM detail WHERE detail_id = LAST_INSERT_ID()";
                o.detail_id = (int)cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Detail o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@detail_id", o.detail_id);
			cmd.Parameters.AddWithValue("@section_id", o.section_id);
			cmd.Parameters.AddWithValue("@detail", o.detail);
			cmd.Parameters.AddWithValue("@sequence", o.sequence);
			cmd.Parameters.AddWithValue("@flag_id", o.flag_id);
		}
		
		public static void Update(Detail o)
		{
			string updatesql = @"UPDATE detail SET section_id = @section_id,detail = @detail,sequence = @sequence,flag_id = @flag_id WHERE detail_id = @detail_id";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = Utilities.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int detail_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utilities.GetCommand("DELETE FROM detail WHERE detail_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", detail_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			