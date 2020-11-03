
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.DAL
{
    public class InspectionsDAL
	{
	
		public static List<Inspections> GetAll()
		{
			var result = new List<Inspections>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspections", conn);
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
                var cmd = Utils.GetCommand("SELECT * FROM inspections WHERE insp_unique = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Controllers = Inspection_controllerDAL.GetByInspection(id);
                    if (result.Controllers.Count == 0)
                    {
                        if (result.insp_qc1 > 0)
                            result.Controllers.Add(new Inspection_controller{controller_id = result.insp_qc1.Value, inspection_id = id,Controller = UserDAL.GetById(result.insp_qc1.Value),startdate = result.insp_start ?? DateTime.Today,duration = result.insp_days ?? 1 });
                        if (result.insp_qc2 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc2.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc2.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc3 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc3.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc3.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc4 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc4.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc4.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc5 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc5.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc5.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        
                    }
                }
                dr.Close();
            }
			return result;
		}

        public static Inspections GetByNewInspId(int newid)
        {
            Inspections result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspections WHERE new_insp_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", newid);
                var dr = cmd.ExecuteReader();
                if (dr.Read()) {
                    result = GetFromDataReader(dr);
                    var id = (int)dr["insp_unique"];
                    result.Controllers = Inspection_controllerDAL.GetByInspection(id);
                    if (result.Controllers.Count == 0) {
                        if (result.insp_qc1 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc1.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc1.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc2 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc2.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc2.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc3 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc3.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc3.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc4 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc4.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc4.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });
                        if (result.insp_qc5 > 0)
                            result.Controllers.Add(new Inspection_controller { controller_id = result.insp_qc5.Value, inspection_id = id, Controller = UserDAL.GetById(result.insp_qc5.Value), startdate = result.insp_start ?? DateTime.Today, duration = result.insp_days ?? 1 });

                    }
                }
                dr.Close();
            }
            return result;
        }

        public static Inspections GetByLoadingId(int loadingId)
        {
            Inspections result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspections WHERE lo_id = @id", conn);
                cmd.Parameters.AddWithValue("@id", loadingId);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                }
            }
            return result;
        }
		
	
		public static Inspections GetFromDataReader(MySqlDataReader dr)
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
            o.new_insp_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "new_insp_id"));

		    o.CustPos = o.custpo.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
			
			return o;

		}
		
		
		public static void Create(Inspections o)
        {
            string insertsql = @"INSERT INTO inspections (insp_unique,insp_id,insp_type,insp_start,insp_end,insp_version,insp_days,insp_porderid,insp_qc1,insp_qc2,insp_qc3,insp_qc6,insp_fc,customer_code,custpo,batch_no,factory_code,insp_comments,insp_comments_admin,insp_status,qc_required,upload,upload_flag,lcl,gp20,gp40,hc40,adjustment,LO_id,acceptance_qc1,acceptance_qc2,acceptance_qc3,acceptance_qc4,acceptance_fc,acceptance_cc,insp_qc5,insp_qc4,etd,eta,insp_batch_inspection,insp_executor) VALUES(@insp_unique,@insp_id,@insp_type,@insp_start,@insp_end,@insp_version,@insp_days,@insp_porderid,@insp_qc1,@insp_qc2,@insp_qc3,@insp_qc6,@insp_fc,@customer_code,@custpo,@batch_no,@factory_code,@insp_comments,@insp_comments_admin,@insp_status,@qc_required,@upload,@upload_flag,@lcl,@gp20,@gp40,@hc40,@adjustment,@LO_id,@acceptance_qc1,@acceptance_qc2,@acceptance_qc3,@acceptance_qc4,@acceptance_fc,@acceptance_cc,@insp_qc5,@insp_qc4,@etd,@eta,@insp_batch_inspection,@insp_executor)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var tr = conn.BeginTransaction();

                try
                {
                    MySqlCommand cmd = Utils.GetCommand("SELECT MAX(insp_unique)+1 FROM inspections", conn);
                    o.insp_unique = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.CommandText = insertsql;

                    BuildSqlParameters(cmd, o);
                    cmd.ExecuteNonQuery();

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@id", o.insp_unique+1);
                    cmd.CommandText = "UPDATE nextinsp SET nextorderid = @id";

                    cmd.ExecuteNonQuery();

                    tr.Commit();
                }
                catch (Exception)
                {
                    tr.Rollback();
                    throw;
                }
				
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

        public static List<Inspections> GetFinalInspectionsForNS(DateTime? from)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                var result = new List<Inspections>();

                conn.Query<Inspections, Inspection_lines_tested, Inspections>(
                                @"SELECT i.*, lt.* FROM inspections i INNER JOIN inspection_lines_tested lt ON i.insp_unique = lt.insp_id
                                  LEFT OUTER JOIN order_lines ol ON lt.order_linenum = ol.linenum 
                                  LEFT OUTER JOIN cust_products p ON ol.cprod_id = p.cprod_id
                                  LEFT OUTER JOIN mast_products mp ON p.cprod_mast = mp.mast_id
                                  WHERE (lt.order_linenum = 0 OR mp.category1 = 13) AND i.insp_start >= @from
                                  AND NOT EXISTS (SELECT insp_id FROM nr_header WHERE insp_id = i.insp_unique)",
                    (i, lt) =>
                    {

                        var insp = result.FirstOrDefault(r=>r.insp_unique == i.insp_unique);
                        if (insp == null)
                        {
                            insp = i;
                            result.Add(insp);
                        }

                        if (insp.LinesTested == null)
                            insp.LinesTested = new List<Inspection_lines_tested>();
                        insp.LinesTested.Add(lt);
                        return insp;
                    }, new { from = from }, splitOn: "insp_line_unique"
                    ).ToList();
                return result;
            }
        }

        public static void Update(Inspections o, List<Inspection_controller> deletedControllers)
		{
			string updatesql = @"UPDATE inspections SET insp_id = @insp_id,insp_type = @insp_type,insp_start = @insp_start,insp_end = @insp_end,insp_version = @insp_version,insp_days = @insp_days,insp_porderid = @insp_porderid,insp_qc1 = @insp_qc1,insp_qc2 = @insp_qc2,insp_qc3 = @insp_qc3,insp_qc6 = @insp_qc6,insp_fc = @insp_fc,customer_code = @customer_code,custpo = @custpo,batch_no = @batch_no,factory_code = @factory_code,insp_comments = @insp_comments,insp_comments_admin = @insp_comments_admin,insp_status = @insp_status,qc_required = @qc_required,upload = @upload,upload_flag = @upload_flag,lcl = @lcl,gp20 = @gp20,gp40 = @gp40,hc40 = @hc40,adjustment = @adjustment,LO_id = @LO_id,acceptance_qc1 = @acceptance_qc1,acceptance_qc2 = @acceptance_qc2,acceptance_qc3 = @acceptance_qc3,acceptance_qc4 = @acceptance_qc4,acceptance_fc = @acceptance_fc,acceptance_cc = @acceptance_cc,insp_qc5 = @insp_qc5,insp_qc4 = @insp_qc4,etd = @etd,eta = @eta,insp_batch_inspection = @insp_batch_inspection,insp_executor = @insp_executor WHERE insp_unique = @insp_unique";

		    var conn = new MySqlConnection(Properties.Settings.Default.ConnString);
            conn.Open();
		    var tr = conn.BeginTransaction();
		    try
		    {
		        MySqlCommand cmd = Utils.GetCommand(updatesql, conn,tr);
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
				var cmd = Utils.GetCommand("DELETE FROM inspections WHERE insp_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_unique);
                cmd.ExecuteNonQuery();
            }
		}


        public static List<Inspections> GetByCriteria(DateTime dateFrom, DateTime dateTo, int? locationId)
        {
            var result = new List<Inspections>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT inspections.*, factory.* FROM inspections LEFT OUTER JOIN users AS factory ON inspections.factory_code = factory.factory_code 
                                        WHERE (insp_start BETWEEN @from AND @to OR DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) BETWEEN @from AND @to OR (insp_start <= @from AND DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) >= @to)) 
                                        AND (factory.consolidated_port2 = @location_id OR @location_id IS NULL OR factory.consolidated_port_mix = 1 OR (factory.factory_code IS NULL AND insp_type = 'X')) 
                                        AND COALESCE(insp_batch_inspection,0) = 0", conn);
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
                var cmd = Utils.GetCommand(string.Empty, conn);
                var sql = string.Format(
                    @"SELECT inspections.*, factory.*, customer.customer_code AS cust_code, customer.user_id AS cust_id, customer.user_name AS cust_name FROM inspections LEFT OUTER JOIN users AS factory ON inspections.factory_code = factory.factory_code 
                                    LEFT OUTER JOIN users customer ON inspections.customer_code = customer.customer_code {0}
                                        WHERE (insp_start BETWEEN @from AND @to OR DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) BETWEEN @from AND @to) AND inspections.factory_code IN ({1}) 
                                        ORDER BY inspections.custpo, inspections.factory_code ASC ,insp_start DESC",
                    user_id != null
                        ? " INNER JOIN admin_permissions ON factory.user_id = admin_permissions.cusid AND admin_permissions.userid = @user_id"
                        : "",Utils.CreateParametersFromIdList(cmd,new List<string>(factorycodes)));
                
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
                    var cmd = Utils.GetCommand(string.Empty, conn);
                    var sql =
                        string.Format(
                            @"SELECT * FROM inspections WHERE factory_code IN ({0}) AND customer_code IN ({1}) AND custpo IN ({2}) AND insp_type = 'LO'",
                            Utils.CreateParametersFromIdList(cmd, new List<string>(factory_codes), "fcode"),
                            Utils.CreateParametersFromIdList(cmd, new List<string>(customer_codes), "ccode"),
                            Utils.CreateParametersFromIdList(cmd, new List<string>(custpos), "custpo"));
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


            if (custpo != null && custpo.Length > 0)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = Utils.GetCommand(string.Empty, conn);
                    var sql =
                        string.Format(
                            "SELECT COUNT(DISTINCT mastid) FROM asaq.2011_change_notice_view WHERE factory_code = @factory_code AND product_po IN ({0})",
                            Utils.CreateParametersFromIdList(cmd, new List<string>(custpo)));
                    cmd.CommandText = sql;
                    cmd.Parameters.AddWithValue("@factory_code", factory_code);
                    result = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return result;
        }

        public static Inspections GetInspection(string custpo, string factory_code, string customer_code)
        {
            Inspections result = null;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT * FROM inspections WHERE custpo LIKE @custpo AND factory_code = @factory_code AND customer_code = @customer_code", conn);
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
            if (custpo != "9999")
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd =
                        Utils.GetCommand(@"SELECT * FROM inspections WHERE custpo LIKE @custpo",
                                         conn);
                    cmd.Parameters.AddWithValue("@custpo", "%" + custpo + "%");
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(GetFromDataReader(dr));
                    }
                    dr.Close();
                }
            }
            return result;
        }

        public static List<Inspections> GetForFeedback(Returns r, bool pendingOnly = true)
        {
            var result = new List<Inspections>();
            var customer = CompanyDAL.GetById(r.client_id ?? 0);
            var product = Cust_productsDAL.GetById(r.cprod_id ?? 0);
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT * FROM inspections WHERE inspections.customer_code = @customer_code 
                                             AND EXISTS (SELECT insp_id FROM inspection_lines_tested WHERE inspection_lines_tested.insp_id = inspections.insp_unique AND inspection_lines_tested.insp_client_ref = @cprod_code)
                                            AND inspections.insp_id <> 'create report' {0}",pendingOnly ? "AND inspections.insp_start > NOW()" : ""), conn);
                cmd.Parameters.AddWithValue("@customer_code", customer != null ? customer.customer_code : "");
                cmd.Parameters.AddWithValue("@cprod_code", product != null ? product.cprod_code1 : "");
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }

        public static List<Inspections> GetForQcs(DateTime? from, DateTime? to, IList<int?> qc_ids)
        {
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                return conn.Query<Inspections>(@"SELECT i.*,
                                (SELECT o.orderid FROM inspection_lines_tested ILT INNER JOIN order_lines O ON ILT.order_linenum = O.linenum WHERE ILT.order_linenum > 0 AND ILT.insp_id = i.insp_unique LIMIT 1) orderid
                                FROM inspections i LEFT OUTER JOIN inspection_v2 i2 ON i.new_insp_id = i2.id
                                WHERE i.insp_start >= @from AND i.insp_start <= @to AND
                                    (i.insp_qc1 IN @ids OR i.insp_qc2 IN @ids OR i.insp_qc3 IN @ids OR i.insp_qc4 IN @ids 
                                    OR i.insp_qc5 IN @ids) 
                                    AND (i.insp_status = 0 AND i.acceptance_fc <> 2 AND (i2.insp_status <> 2 OR i2.insp_status IS NULL))
                                    AND COALESCE(i.custpo,'') <> ''
                                    AND i.insp_batch_inspection = 0
                                    AND i.insp_type = 'FI'
                                    ORDER BY i.insp_start
                                ", new { from = from, to = to, ids = qc_ids.ToArray() }).ToList();                    
                    
            }
        }

		public static int GetIdFromIdString(string sId, bool isV2 = false)
		{
			int id;
			var isInt = int.TryParse(sId, out id);
			if (isInt)
				return id;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
			{
				conn.Open();
				var query = !isV2
					? "SELECT insp_unique FROM inspections WHERE id = @id"
					: "SELECT new_insp_id FROM inspections WHERE id = @id ";
				return conn.ExecuteScalar<int>(query, new {id = sId});
			}
		}

	}
}
			
			