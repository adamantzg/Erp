
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Survey_resultDAL
	{
	
		public static List<Survey_result> GetAll()
		{
			var result = new List<Survey_result>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Survey_result> GetForDef(int def_id)
        {
            var result = new List<Survey_result>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result WHERE surveydef_id = @def_id", conn);
                cmd.Parameters.AddWithValue("@def_id", def_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
		
		
		public static Survey_result GetById(int id)
		{
			Survey_result result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result WHERE result_id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    //result.Answers = Survey_result_answerDAL.GetForResult(result.result_id);
                }
                dr.Close();
            }
			return result;
		}

        public static Survey_result GetForDealer(int dealer_id, int surveydef_id)
        {
            Survey_result result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM survey_result WHERE dealer_id = @dealer_id AND surveydef_id = @def_id", conn);
                cmd.Parameters.AddWithValue("@dealer_id", dealer_id);
                cmd.Parameters.AddWithValue("@def_id", surveydef_id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    //result.Answers = Survey_result_answerDAL.GetForResult(result.result_id);
                }
                dr.Close();
            }
            return result;
        }

        public static Survey_result GetForDealer(string dealer_code, int surveydef_id)
        {
            Survey_result result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM survey_result INNER JOIN dealers ON survey_result.dealer_id = dealers.user_id 
                                            WHERE dealers.survey_id = @code AND surveydef_id = @def_id", conn);
                cmd.Parameters.AddWithValue("@code", dealer_code);
                cmd.Parameters.AddWithValue("@def_id", surveydef_id);
                var dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    result = GetFromDataReader(dr);
                    //result.Answers = Survey_result_answerDAL.GetForResult(result.result_id);
                }
                dr.Close();
            }
            return result;
        }


        public static Survey_result GetFromDataReader(MySqlDataReader dr)
		{
			Survey_result o = new Survey_result();
		
			o.result_id =  (int) dr["result_id"];
			o.surveydef_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"surveydef_id"));
			o.datecreated = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"datecreated"));
			o.dealer_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"dealer_id"));
			o.user_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"user_id"));
			o.ipaddress = string.Empty + Utilities.GetReaderField(dr,"ipaddress");
		    
			return o;

		}
		
		
		public static void Create(Survey_result o)
        {
            string insertsql = @"INSERT INTO survey_result (result_id,surveydef_id,datecreated,dealer_id,user_id,ipaddress) VALUES(@result_id,@surveydef_id,@datecreated,@dealer_id,@user_id,@ipaddress)";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
		    try
		    {

		        var cmd = Utils.GetCommand(insertsql,conn,tr);
		        BuildSqlParameters(cmd, o);
		        cmd.ExecuteNonQuery();
                
				cmd.CommandText = "SELECT result_id FROM survey_result WHERE result_id = LAST_INSERT_ID()";
                o.result_id = (int) cmd.ExecuteScalar();

		        if (o.Answers != null)
		        {
		            foreach (var answer in o.Answers)
		            {
		                answer.result_id = o.result_id;
                        Survey_result_answerDAL.Create(answer,tr);
		            }
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
		        conn = null;
		    }
		    
        }
		
		private static void BuildSqlParameters(MySqlCommand cmd, Survey_result o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@result_id", o.result_id);
			cmd.Parameters.AddWithValue("@surveydef_id", o.surveydef_id);
			cmd.Parameters.AddWithValue("@datecreated", o.datecreated);
			cmd.Parameters.AddWithValue("@dealer_id", o.dealer_id);
			cmd.Parameters.AddWithValue("@user_id", o.user_id);
			cmd.Parameters.AddWithValue("@ipaddress", o.ipaddress);
		}
		
		public static void Update(Survey_result o)
		{
			string updatesql = @"UPDATE survey_result SET surveydef_id = @surveydef_id,datecreated = @datecreated,dealer_id = @dealer_id,user_id = @user_id,ipaddress = @ipaddress WHERE result_id = @result_id";

            var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
            MySqlTransaction tr = conn.BeginTransaction();
            try {

                var cmd = Utils.GetCommand(updatesql, conn,tr);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "DELETE FROM survey_result_answer WHERE result_id = @result_id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@result_id", o.result_id);
                //cmd.ExecuteNonQuery();

                if (o.Answers != null) {
                    foreach (var answer in o.Answers) {
                        answer.result_id = o.result_id;
                        Survey_result_answerDAL.Create(answer, tr);
                    }
                }
                tr.Commit();


            }
            catch {
                tr.Rollback();
                throw;
            }
            finally {
                conn = null;
            }
        }
		
		public static void Delete(int result_id)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM survey_result WHERE result_id = @id" , conn);
                cmd.Parameters.AddWithValue("@id", result_id);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			