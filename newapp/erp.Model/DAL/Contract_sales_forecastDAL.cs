
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
	public class Contract_sales_forecastDAL
	{
	
		public static List<Contract_sales_forecast> GetAll()
		{
			List<Contract_sales_forecast> result = new List<Contract_sales_forecast>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(GetSelectSql(), conn);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}

        

		public static List<Contract_sales_forecast> GetForCompany(int company_id)
		{
			List<Contract_sales_forecast> result = new List<Contract_sales_forecast>();
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(GetSelectSql() + " WHERE client_id = @client_id", conn);
				cmd.Parameters.AddWithValue("@client_id", company_id);
				MySqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					result.Add(GetFromDataReader(dr));
				}
				dr.Close();
			}
			return result;
		}

		private static string GetSelectSql()
		{
			return @"SELECT contract_sales_forecast.*,Creator.userwelcome AS creator,Editor.userwelcome AS editor FROM contract_sales_forecast 
					 INNER JOIN userusers AS Creator ON Creator.useruserid = contract_sales_forecast.created_userid
					LEFT JOIN userusers AS Editor ON Editor.useruserid = contract_sales_forecast.modified_userid";
		}

		public static Contract_sales_forecast GetById(int id)
		{
			Contract_sales_forecast result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("SELECT * FROM contract_sales_forecast WHERE forecast_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				MySqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					result = GetFromDataReader(dr);
					result.Lines = Contract_sales_forecast_linesDAL.GetByForecastId(id);
				}
				dr.Close();
			}
			return result;
		}
	
		public static Contract_sales_forecast GetFromDataReader(MySqlDataReader dr)
		{
			Contract_sales_forecast o = new Contract_sales_forecast();
		
			o.forecast_id =  (int) dr["forecast_id"];
			o.client_id = Utilities.FromDbValue<int>(dr["client_id"]);
			o.reference = string.Empty + dr["reference"];
			o.startmonth = Utilities.FromDbValue<DateTime>(dr["startmonth"]);
		    if (o.startmonth != null)
		    {
                //reset to 1st in month
		        o.startmonth = o.startmonth.Value.AddDays(-1*(o.startmonth.Value.Day - 1));
		    }
		    o.monthduration = Utilities.FromDbValue<int>(dr["monthduration"]);
			o.datecreated = Utilities.FromDbValue<DateTime>(dr["datecreated"]);
			o.created_userid = Utilities.FromDbValue<int>(dr["created_userid"]);
			o.datemodified = Utilities.FromDbValue<DateTime>(dr["datemodified"]);
			o.modified_userid = Utilities.FromDbValue<int>(dr["modified_userid"]);
			if (Utilities.ColumnExists(dr, "creator"))
				o.creator = string.Empty + dr["creator"];
			if (Utilities.ColumnExists(dr, "editor"))
				o.editor = string.Empty + dr["editor"];
			
			return o;

		}

		public static void Create(Contract_sales_forecast o)
		{
			var insertsql = @"INSERT INTO contract_sales_forecast (client_id,reference,startmonth,monthduration,datecreated,created_userid,datemodified,modified_userid) VALUES(@client_id,@reference,@startmonth,@monthduration,@datecreated,@created_userid,@datemodified,@modified_userid)";

			var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{
				var cmd = new MySqlCommand(insertsql, conn, tr);
				BuildSqlParameters(cmd, o);
				cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT forecast_id FROM contract_sales_forecast WHERE forecast_id = LAST_INSERT_ID()";
				o.forecast_id = (int) cmd.ExecuteScalar();
				if (o.Lines != null)
				{ 
					foreach (var line in o.Lines)
					{
						line.forecast_id = o.forecast_id;
						Contract_sales_forecast_linesDAL.Create(line, tr);
					}
				}
				tr.Commit();
			}
			catch
			{
				if (tr != null)
					tr.Rollback();
			}
			finally
			{
				conn = null;
			}
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Contract_sales_forecast o, bool forInsert = true)
		{
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@forecast_id", o.forecast_id);
			cmd.Parameters.AddWithValue("@client_id", o.client_id);
			cmd.Parameters.AddWithValue("@reference", o.reference);
			cmd.Parameters.AddWithValue("@startmonth", o.startmonth);
			cmd.Parameters.AddWithValue("@monthduration", o.monthduration);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@created_userid", o.created_userid);
			cmd.Parameters.AddWithValue("@datemodified", o.datemodified);
			cmd.Parameters.AddWithValue("@modified_userid", o.modified_userid);
		}
		
		public static void Update(Contract_sales_forecast o, List<Contract_sales_forecast_lines> deletedLines)
		{
			string updatesql = @"UPDATE contract_sales_forecast SET client_id = @client_id,reference = @reference,startmonth = @startmonth,monthduration = @monthduration,datecreated = @datecreated,created_userid = @created_userid,datemodified = @datemodified,modified_userid = @modified_userid WHERE forecast_id = @forecast_id";
			var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
				BuildSqlParameters(cmd, o, false);
				cmd.ExecuteNonQuery();

				if (o.Lines != null)
				{ 
					foreach (var line in o.Lines)
					{
						if(line.lines_id <=0)
							Contract_sales_forecast_linesDAL.Create(line, tr);
						else
						{
							Contract_sales_forecast_linesDAL.Update(line,tr);
						}
					}
				}
				foreach (var d in deletedLines)
				{
					Contract_sales_forecast_linesDAL.Delete(d.lines_id,tr);
				}

				//Update lines
				tr.Commit();
			}
			catch
			{
				if (tr != null)
					tr.Rollback();
			}
			finally
			{
				conn = null;
			}
		}
		
		public static void Delete(int forecast_id)
		{
			var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
			conn.Open();
			MySqlTransaction tr = conn.BeginTransaction();
			try
			{
				Contract_sales_forecast_linesDAL.DeleteByForecast(forecast_id, tr);
				var cmd = new MySqlCommand("DELETE FROM contract_sales_forecast WHERE forecast_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", forecast_id);
				cmd.ExecuteNonQuery();
				tr.Commit();
			}
			catch
			{
				if (tr != null)
					tr.Rollback();
			}
			finally
			{
				conn = null;
			}
		}
	}
}
			
			