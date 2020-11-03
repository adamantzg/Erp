
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Inspection_controllerDAL
	{
	
		public static List<Inspection_controller> GetAll()
		{
			var result = new List<Inspection_controller>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_controller", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_controller> GetByInspection(int inspection_id)
        {
            var result = new List<Inspection_controller>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT inspection_controller.*,userusers.* FROM inspection_controller INNER JOIN userusers ON inspection_controller.controller_id = userusers.useruserid" +
                                           " WHERE inspection_id = @inspection_id", conn);
                cmd.Parameters.AddWithValue("@inspection_id", inspection_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var c = GetFromDataReader(dr);
                    c.Controller = UserDAL.GetFromDataReader(dr);
                    result.Add(c);
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Inspection_controller GetById(int id)
		{
			Inspection_controller result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_controller WHERE id = @id", conn);
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
		
	
		private static Inspection_controller GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_controller o = new Inspection_controller();
		
			o.id =  (int) dr["id"];
			o.inspection_id = (int)Utilities.GetReaderField(dr,"inspection_id");
			o.controller_id = (int)Utilities.GetReaderField(dr,"controller_id");
			o.startdate = (DateTime)Utilities.GetReaderField(dr,"startdate");
			o.duration = (int)Utilities.GetReaderField(dr,"duration");
			
			return o;

		}
		
		
		public static void Create(Inspection_controller o, MySqlTransaction tr)
        {
            string insertsql = @"INSERT INTO inspection_controller (inspection_id,controller_id,startdate,duration) VALUES(@inspection_id,@controller_id,@startdate,@duration)";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
            	
			var cmd = Utils.GetCommand(insertsql, conn, tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT id FROM inspection_controller WHERE id = LAST_INSERT_ID()";
            o.id = (int) cmd.ExecuteScalar();

            if(tr == null)
		        conn = null;


        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_controller o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@id", o.id);
			cmd.Parameters.AddWithValue("@inspection_id", o.inspection_id);
			cmd.Parameters.AddWithValue("@controller_id", o.controller_id);
			cmd.Parameters.AddWithValue("@startdate", o.startdate);
			cmd.Parameters.AddWithValue("@duration", o.duration);
		}
		
		public static void Update(Inspection_controller o, MySqlTransaction tr)
		{
			string updatesql = @"UPDATE inspection_controller SET inspection_id = @inspection_id,controller_id = @controller_id,startdate = @startdate,duration = @duration WHERE id = @id";

            var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
            if (tr == null)
                conn.Open();
            
			var cmd = Utils.GetCommand(updatesql, conn);
            BuildSqlParameters(cmd,o, false);
            cmd.ExecuteNonQuery();
		    if (tr == null)
		        conn = null;
		}
		
		public static void Delete(int id, MySqlTransaction tr)
		{
			var conn = tr != null ? tr.Connection : new MySqlConnection(Properties.Settings.Default.ConnString);
			if(tr == null)
                conn.Open();
            
			var cmd = Utils.GetCommand("DELETE FROM inspection_controller WHERE id = @id" , conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            if (tr == null)
                conn = null;
		}
		
		
	}
}
			
			