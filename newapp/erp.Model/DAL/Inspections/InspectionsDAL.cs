
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asaq2.Model
{
    public class InspectionsDAL
	{
	
		public static List<Inspections> GetAll()
		{
			var result = new List<Inspections>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspections", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}
		
		
		public static Inspections GetById(int id)
		{
			Inspections result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM inspections WHERE insp_unique = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Controllers = Inspection_controllerDAL.GetByInspection(id);
                    if (result.Controllers.Count == 0)
                    {
                        if (result.insp_qc1 > 0)
                            result.Controllers.Add(new Inspection_controller{controller_id = result.insp_qc1.Value, Controller = UserDAL.GetById(result.insp_qc1.Value)});
                        if (result.insp_qc2 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc2.Value, Controller = UserDAL.GetById(result.insp_qc2.Value) });
                        if (result.insp_qc3 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc3.Value, Controller = UserDAL.GetById(result.insp_qc3.Value) });
                        if (result.insp_qc4 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc4.Value, Controller = UserDAL.GetById(result.insp_qc4.Value) });
                        if (result.insp_qc5 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc5.Value, Controller = UserDAL.GetById(result.insp_qc5.Value) });
                        
                    }
                }
                dr.Close();
            }
			return result;
		}
		
	
		private static Inspections GetFromDataReader(MySqlDataReader dr)
		{
			Inspections o = new Inspections();
		
			o.insp_unique =  (int) dr["insp_unique"];
			o.insp_id = string.Empty + Utilities.GetReaderField(dr,"insp_id");
			o.insp_type = string.Empty + Utilities.GetReaderField(dr,"insp_type");
		    o.insp_start = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"insp_start"));
			o.insp_end = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"insp_end"));
			o.insp_version = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_version"));
			o.insp_days = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_days"));
			o.insp_porderid = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_porderid"));
			o.insp_qc1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qc1"));
			o.insp_qc2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qc2"));
			o.insp_qc3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qc3"));
			o.insp_qc6 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qc6"));
			o.insp_fc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_fc"));
			o.customer_code = string.Empty + Utilities.GetReaderField(dr,"customer_code");
			o.custpo = string.Empty + Utilities.GetReaderField(dr,"custpo");
			o.batch_no = string.Empty + Utilities.GetReaderField(dr,"batch_no");
			o.factory_code = string.Empty + Utilities.GetReaderField(dr,"factory_code");
			o.insp_comments = string.Empty + Utilities.GetReaderField(dr,"insp_comments");
			o.insp_comments_admin = string.Empty + Utilities.GetReaderField(dr,"insp_comments_admin");
			o.insp_status = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_status"));
			o.qc_required = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"qc_required"));
			o.upload = string.Empty + Utilities.GetReaderField(dr,"upload");
			o.upload_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"upload_flag"));
			o.lcl = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"lcl"));
			o.gp20 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"gp20"));
			o.gp40 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"gp40"));
			o.hc40 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"hc40"));
			o.adjustment = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"adjustment"));
			o.LO_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"LO_id"));
			o.acceptance_qc1 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"acceptance_qc1"));
			o.acceptance_qc2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"acceptance_qc2"));
			o.acceptance_qc3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"acceptance_qc3"));
			o.acceptance_qc4 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"acceptance_qc4"));
			o.acceptance_fc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"acceptance_fc"));
			o.acceptance_cc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"acceptance_cc"));
			o.insp_qc5 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qc5"));
			o.insp_qc4 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_qc4"));
			o.etd = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"etd"));
			o.eta = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr,"eta"));
			o.insp_batch_inspection = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_batch_inspection"));
			o.insp_executor = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_executor"));

		    o.CustPos = o.custpo.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
			
			return o;

		}
		
		
		public static void Create(Inspections o)
        {
            string insertsql = @"INSERT INTO inspections (insp_unique,insp_id,insp_type,insp_start,insp_end,insp_version,insp_days,insp_porderid,insp_qc1,insp_qc2,insp_qc3,insp_qc6,insp_fc,customer_code,custpo,batch_no,factory_code,insp_comments,insp_comments_admin,insp_status,qc_required,upload,upload_flag,lcl,gp20,gp40,hc40,adjustment,LO_id,acceptance_qc1,acceptance_qc2,acceptance_qc3,acceptance_qc4,acceptance_fc,acceptance_cc,insp_qc5,insp_qc4,etd,eta,insp_batch_inspection,insp_executor) VALUES(@insp_unique,@insp_id,@insp_type,@insp_start,@insp_end,@insp_version,@insp_days,@insp_porderid,@insp_qc1,@insp_qc2,@insp_qc3,@insp_qc6,@insp_fc,@customer_code,@custpo,@batch_no,@factory_code,@insp_comments,@insp_comments_admin,@insp_status,@qc_required,@upload,@upload_flag,@lcl,@gp20,@gp40,@hc40,@adjustment,@LO_id,@acceptance_qc1,@acceptance_qc2,@acceptance_qc3,@acceptance_qc4,@acceptance_fc,@acceptance_cc,@insp_qc5,@insp_qc4,@etd,@eta,@insp_batch_inspection,@insp_executor)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
                MySqlCommand cmd = new MySqlCommand("SELECT MAX(insp_unique)+1 FROM inspections", conn);
                o.insp_unique = Convert.ToInt32(cmd.ExecuteScalar());
				cmd.CommandText = insertsql;
				
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspections o, bool forInsert = true)
        {
			
			cmd.Parameters.AddWithValue("@insp_unique", o.insp_unique);
			cmd.Parameters.AddWithValue("@insp_id", o.insp_id);
			cmd.Parameters.AddWithValue("@insp_type", o.insp_type);
			cmd.Parameters.AddWithValue("@insp_start", o.insp_start);
			cmd.Parameters.AddWithValue("@insp_end", o.insp_end);
			cmd.Parameters.AddWithValue("@insp_version", o.insp_version);
			cmd.Parameters.AddWithValue("@insp_days", o.insp_days);
			cmd.Parameters.AddWithValue("@insp_porderid", o.insp_porderid);
			cmd.Parameters.AddWithValue("@insp_qc1", o.insp_qc1);
			cmd.Parameters.AddWithValue("@insp_qc2", o.insp_qc2);
			cmd.Parameters.AddWithValue("@insp_qc3", o.insp_qc3);
			cmd.Parameters.AddWithValue("@insp_qc6", o.insp_qc6);
			cmd.Parameters.AddWithValue("@insp_fc", o.insp_fc);
			cmd.Parameters.AddWithValue("@customer_code", o.customer_code);
			cmd.Parameters.AddWithValue("@custpo", o.custpo);
			cmd.Parameters.AddWithValue("@batch_no", o.batch_no);
			cmd.Parameters.AddWithValue("@factory_code", o.factory_code);
			cmd.Parameters.AddWithValue("@insp_comments", o.insp_comments);
			cmd.Parameters.AddWithValue("@insp_comments_admin", o.insp_comments_admin);
			cmd.Parameters.AddWithValue("@insp_status", o.insp_status);
			cmd.Parameters.AddWithValue("@qc_required", o.qc_required);
			cmd.Parameters.AddWithValue("@upload", o.upload);
			cmd.Parameters.AddWithValue("@upload_flag", o.upload_flag);
			cmd.Parameters.AddWithValue("@lcl", o.lcl);
			cmd.Parameters.AddWithValue("@gp20", o.gp20);
			cmd.Parameters.AddWithValue("@gp40", o.gp40);
			cmd.Parameters.AddWithValue("@hc40", o.hc40);
			cmd.Parameters.AddWithValue("@adjustment", o.adjustment);
			cmd.Parameters.AddWithValue("@LO_id", o.LO_id);
			cmd.Parameters.AddWithValue("@acceptance_qc1", o.acceptance_qc1);
			cmd.Parameters.AddWithValue("@acceptance_qc2", o.acceptance_qc2);
			cmd.Parameters.AddWithValue("@acceptance_qc3", o.acceptance_qc3);
			cmd.Parameters.AddWithValue("@acceptance_qc4", o.acceptance_qc4);
			cmd.Parameters.AddWithValue("@acceptance_fc", o.acceptance_fc);
			cmd.Parameters.AddWithValue("@acceptance_cc", o.acceptance_cc);
			cmd.Parameters.AddWithValue("@insp_qc5", o.insp_qc5);
			cmd.Parameters.AddWithValue("@insp_qc4", o.insp_qc4);
			cmd.Parameters.AddWithValue("@etd", o.etd);
			cmd.Parameters.AddWithValue("@eta", o.eta);
			cmd.Parameters.AddWithValue("@insp_batch_inspection", o.insp_batch_inspection);
			cmd.Parameters.AddWithValue("@insp_executor", o.insp_executor);
		}
		
		public static void Update(Inspections o, List<Inspection_controller> deletedControllers)
		{
			string updatesql = @"UPDATE inspections SET insp_id = @insp_id,insp_type = @insp_type,insp_start = @insp_start,insp_end = @insp_end,insp_version = @insp_version,insp_days = @insp_days,insp_porderid = @insp_porderid,insp_qc1 = @insp_qc1,insp_qc2 = @insp_qc2,insp_qc3 = @insp_qc3,insp_qc6 = @insp_qc6,insp_fc = @insp_fc,customer_code = @customer_code,custpo = @custpo,batch_no = @batch_no,factory_code = @factory_code,insp_comments = @insp_comments,insp_comments_admin = @insp_comments_admin,insp_status = @insp_status,qc_required = @qc_required,upload = @upload,upload_flag = @upload_flag,lcl = @lcl,gp20 = @gp20,gp40 = @gp40,hc40 = @hc40,adjustment = @adjustment,LO_id = @LO_id,acceptance_qc1 = @acceptance_qc1,acceptance_qc2 = @acceptance_qc2,acceptance_qc3 = @acceptance_qc3,acceptance_qc4 = @acceptance_qc4,acceptance_fc = @acceptance_fc,acceptance_cc = @acceptance_cc,insp_qc5 = @insp_qc5,insp_qc4 = @insp_qc4,etd = @etd,eta = @eta,insp_batch_inspection = @insp_batch_inspection,insp_executor = @insp_executor WHERE insp_unique = @insp_unique";

		    var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
		    var tr = conn.BeginTransaction();
		    try
		    {
		        MySqlCommand cmd = new MySqlCommand(updatesql, conn,tr);
		        BuildSqlParameters(cmd, o, false);
		        cmd.ExecuteNonQuery();

                if (o.Controllers != null)
                {
                    foreach (var c in o.Controllers)
                    {
                        if (c.id <= 0)
                            Inspection_controllerDAL.Create(c, tr);
                        else
                        {
                            Inspection_controllerDAL.Update(c, tr);
                        }
                    }
                }
                foreach (var d in deletedControllers)
                {
                    Inspection_controllerDAL.Delete(d.id, tr);
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
		
		public static void Delete(int insp_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = new MySqlCommand("DELETE FROM inspections WHERE insp_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_unique);
                cmd.ExecuteNonQuery();
            }
		}


        public static List<Inspections> GetByCriteria(DateTime dateFrom, DateTime dateTo, int locationId)
        {
            var result = new List<Inspections>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT inspections.*, factory.* FROM inspections LEFT OUTER JOIN users AS factory ON inspections.factory_code = factory.factory_code 
                                        WHERE (insp_start BETWEEN @from AND @to OR DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) BETWEEN @from AND @to OR (insp_start <= @from AND DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) >= @to)) 
                                        AND (factory.consolidated_port2 = @location_id OR factory.consolidated_port_mix = 1 OR (factory.factory_code IS NULL AND insp_type = 'X')) AND COALESCE(insp_batch_inspection,0) = 0", conn);
                cmd.Parameters.AddWithValue("@from", dateFrom);
                cmd.Parameters.AddWithValue("@to", dateTo);
                cmd.Parameters.AddWithValue("@location_id", locationId);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var insp = GetFromDataReader(dr);
                    var factory_id = Utilities.FromDbValue<int>(dr["user_id"]);
                    if (factory_id != null)
                    {
                        insp.Factory = new Company
                            {
                                user_id = (int) dr["user_id"],
                                user_name = string.Empty + dr["user_name"],
                                factory_code = string.Empty + dr["factory_code"],
                                consolidated_port_mix = Utilities.FromDbValue<int>(dr["consolidated_port_mix"]),
                                consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                                consolidated_port2 = Utilities.FromDbValue<int>(dr["consolidated_port2"])
                            };
                    }
                    if (result.Count(r => r.insp_unique == insp.insp_unique) == 0)
                    {
                        //Prevent duplicates
                        insp.Controllers = Inspection_controllerDAL.GetByInspection(insp.insp_unique);
                        result.Add(insp);
                    }
                }
                dr.Close();
                //SI
                cmd.CommandText = @"SELECT 2012_inspection_sample_table.*, factory.*, customer.customer_code AS cust_code FROM 2012_inspection_sample_table INNER JOIN users factory ON 2012_inspection_sample_table.si_factory = factory.user_id
                                    LEFT OUTER JOIN users customer ON 2012_inspection_sample_table.si_client = customer.user_id
                                    WHERE (si_insp_date BETWEEN @from AND @to) 
                                        AND (factory.consolidated_port2 = @location_id OR factory.consolidated_port_mix = 1)";
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var insp = new Inspections
                        {
                            insp_type = "SI",
                            insp_start =  Utilities.FromDbValue<DateTime>(dr["si_insp_date"]),
                            insp_days = 1,
                            Controllers = new List<Inspection_controller>(),
                            insp_qc1 = Utilities.FromDbValue<int>(dr["si_qc"]),
                            insp_qc2 = Utilities.FromDbValue<int>(dr["si_qc2"]),
                            customer_code = string.Empty + dr["cust_code"],
                            factory_code = string.Empty + dr["factory_code"],
                            insp_unique = (int) dr["si_id"]
                        };

                    insp.Factory = new Company
                    {
                        user_id = (int)dr["user_id"],
                        user_name = string.Empty + dr["user_name"],
                        factory_code = string.Empty + dr["factory_code"],
                        consolidated_port_mix = Utilities.FromDbValue<int>(dr["consolidated_port_mix"]),
                        consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                        consolidated_port2 = Utilities.FromDbValue<int>(dr["consolidated_port2"])
                    };
                    result.Add(insp);
                }
                dr.Close();
            }
            return result;
        }

        /// <summary>
        /// Possibly restricts inspections if user is not admin
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public static List<Inspections> GetForExport(DateTime dateFrom, DateTime dateTo,string[] factorycodes, int? user_id = null)
        {
            var result = new List<Inspections>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(string.Empty, conn);
                var sql = string.Format(
                    @"SELECT inspections.*, factory.*, customer.customer_code AS cust_code, customer.user_id AS cust_id, customer.user_name AS cust_name FROM inspections LEFT OUTER JOIN users AS factory ON inspections.factory_code = factory.factory_code 
                                    LEFT OUTER JOIN users customer ON inspections.customer_code = customer.customer_code {0}
                                        WHERE (insp_start BETWEEN @from AND @to OR DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) BETWEEN @from AND @to) AND inspections.factory_code IN ({1}) 
                                        ORDER BY inspections.custpo, inspections.factory_code ASC ,insp_start DESC",
                    user_id != null
                        ? " INNER JOIN admin_permissions ON factory.user_id = admin_permissions.cusid AND admin_permissions.userid = @user_id"
                        : "",Utilities.CreateParametersFromIdList(cmd,new List<string>(factorycodes)));
                
                cmd.Parameters.AddWithValue("@from", dateFrom);
                cmd.Parameters.AddWithValue("@to", dateTo);
                cmd.Parameters.AddWithValue("@user_id", user_id != null ? (object) user_id : DBNull.Value);
                cmd.CommandText = sql;
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var insp = GetFromDataReader(dr);
                    var factory_id = Utilities.FromDbValue<int>(dr["user_id"]);
                    if (factory_id != null)
                    {
                        insp.Factory = new Company
                            {
                                user_id = (int) dr["user_id"],
                                user_name = string.Empty + dr["user_name"],
                                factory_code = string.Empty + dr["factory_code"],
                                consolidated_port_mix = Utilities.FromDbValue<int>(dr["consolidated_port_mix"]),
                                consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                                consolidated_port2 = Utilities.FromDbValue<int>(dr["consolidated_port2"])
                            };
                    }
                    insp.customer_id = Utilities.FromDbValue<int>(dr["cust_id"]);
                    
                    if (result.Count(r => r.insp_unique == insp.insp_unique) == 0)
                    {
                        //Prevent duplicates
                        result.Add(insp);
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static List<Inspections> GetLoadingInspections(string[] factory_codes, string[] customer_codes,string[] custpos)
	    {
	        var result = new List<Inspections>();
            if (factory_codes.Length > 0 && customer_codes.Length > 0 && custpos.Length > 0)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(string.Empty, conn);
                    var sql =
                        string.Format(
                            @"SELECT * FROM inspections WHERE factory_code IN ({0}) AND customer_code IN ({1}) AND custpo IN ({2}) AND insp_type = 'LO'",
                            Utilities.CreateParametersFromIdList(cmd, new List<string>(factory_codes), "fcode"),
                            Utilities.CreateParametersFromIdList(cmd, new List<string>(customer_codes), "ccode"),
                            Utilities.CreateParametersFromIdList(cmd, new List<string>(custpos), "custpo"));
                    cmd.CommandText = sql;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                }
            }
            return result;
	    }

        public static int GetChangedProductCount(string[] custpo,string factory_code)
        {
            int result = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(string.Empty, conn);
                var sql =
                    string.Format(
                        "SELECT COUNT(DISTINCT mastid) FROM asaq.2011_change_notice_view WHERE factory_code = @factory_code AND product_po IN ({0})",
                        Utilities.CreateParametersFromIdList(cmd, new List<string>(custpo)));
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@factory_code", factory_code);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }

        public static Inspections GetInspection(string custpo, string factory_code, string customer_code)
        {
            Inspections result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM inspections WHERE custpo LIKE @custpo AND factory_code = @factory_code AND customer_code = @customer_code", conn);
                cmd.Parameters.AddWithValue("@custpo", "%" + custpo + "%");
                cmd.Parameters.AddWithValue("@factory_code", factory_code);
                cmd.Parameters.AddWithValue("@customer_code", customer_code);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
            }
            return result;
        }

        public static List<Inspections> GetForCustPo(string custpo)
        {
            var result = new List<Inspections>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM inspections WHERE custpo LIKE @custpo", conn);
                cmd.Parameters.AddWithValue("@custpo", "%" + custpo + "%");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

	}
}
			
			