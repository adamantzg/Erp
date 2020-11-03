
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.DAL
{
    public class Inspection_lines_testedDAL
	{
	
		public static List<Inspection_lines_tested> GetAll()
		{
			var result = new List<Inspection_lines_tested>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_lines_tested", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
		}

        public static List<Inspection_lines_tested> GetByInspection(int insp_id, bool includeProducts = false, bool includeLoadings = false)
        {
            var result = new List<Inspection_lines_tested>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand($@"SELECT 
							inspection_lines_tested.insp_line_unique,
							inspection_lines_tested.insp_id,
							inspection_lines_tested.insp_factory_ref,
							inspection_lines_tested.insp_client_ref,
							inspection_lines_tested.insp_client_desc,
							inspection_lines_tested.insp_qty,
							inspection_lines_tested.insp_override_qty,
							inspection_lines_tested.insp_custpo,
							inspection_lines_tested.order_linenum,
							inspection_lines_tested.photo_confirm,
							inspection_lines_tested.photo_confirma,
							inspection_lines_tested.photo_confirmm,
							inspection_lines_tested.photo_confirmd,
							inspection_lines_tested.photo_confirmf,
							inspection_lines_tested.photo_confirmp,
							inspection_lines_tested.packaging_rej,
							inspection_lines_tested.label_rej,
							inspection_lines_tested.instructions_rej,
							 order_lines.* {(includeProducts ? ",cust_products.*, mast_products.*" : "")} 
                                            FROM Inspection_lines_tested LEFT OUTER JOIN order_lines ON Inspection_lines_tested.order_linenum = order_lines.linenum 
                                            {(includeProducts ? "LEFT OUTER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id LEFT OUTER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id" : "")}
                                            WHERE insp_id = @insp_id", conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    if (line.order_linenum > 0)
                        line.OrderLine = Order_linesDAL.GetFromDataReader(dr);
                    if (includeProducts && line.OrderLine != null && line.OrderLine.cprod_id > 0)
                    {
                        line.OrderLine.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                        line.OrderLine.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    }
                    result.Add(line);
                }
                dr.Close();

                if (includeLoadings)
                {
                    FillLoadings(new int[] { insp_id }, conn, result);
                }
            }
            return result;
        }

        public static List<Inspection_lines_tested> GetLinesForOrders(IList<int> orderIds, int? excludedInspId = null, bool includeLoadings = false)
        {
            var result = new List<Inspection_lines_tested>();

            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    $@"SELECT inspection_lines_tested.*,order_lines.*,cust_products.*, mast_products.* ,inspections.*
                        FROM inspection_lines_tested INNER JOIN inspections ON inspection_lines_tested.insp_id = inspections.insp_unique 
                        LEFT OUTER JOIN order_lines ON inspection_lines_tested.order_linenum = order_lines.linenum 
                        LEFT OUTER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                        LEFT OUTER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        WHERE {(excludedInspId != null ? " inspection_lines_tested.insp_id <> @insp_id AND " : "")} order_lines.orderid IN ({Utils
                        .CreateParametersFromIdList(cmd, orderIds)})";
                if (excludedInspId != null)
                    cmd.Parameters.AddWithValue("@insp_id", excludedInspId);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var lt = GetFromDataReader(dr);
                    lt.Inspection = InspectionsDAL.GetFromDataReader(dr);
                    lt.OrderLine = Order_linesDAL.GetFromDataReader(dr);
                    lt.OrderLine.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                    lt.OrderLine.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(lt);
                }
                dr.Close();
                if (includeLoadings)
                {
                    var insp_ids = result.Select(i => i.insp_id ?? 0).Distinct().ToList();
                    FillLoadings(insp_ids, conn, result);
                }
            }

            return result;
        }

        private static void FillLoadings(IList<int> insp_ids, MySqlConnection conn, List<Inspection_lines_tested> lines)
        {
            var loadings = Inspections_loadingDAL.GetForInspection(insp_ids, conn);
            foreach (var l in loadings)
            {
                var line = lines.FirstOrDefault(li => li.insp_line_unique == l.insp_line_unique);
                if (line != null)
                {
                    if (line.Loadings == null)
                        line.Loadings = new List<Inspections_loading>();
                    line.Loadings.Add(l);
                }
            }
        }


        public static Inspection_lines_tested GetById(int id)
		{
			Inspection_lines_tested result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_lines_tested WHERE insp_line_unique = @id", conn);
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
        public static List<Inspection_lines_tested> GetInspLines(int insp_id)
        {
            var result = new List<Inspection_lines_tested>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT * FROM inspection_lines_tested WHERE insp_id = @insp_id", conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(GetFromDataReader(dr));
                }
                dr.Close();
            }
            return result;
        }
        /// <summary>
        /// prvi parametar za filtriranje po inspection
        /// druga dva dohvati sve pa filtriraj po factories  po distributeru
        /// </summary>
        /// <param name="insp_id"></param>
        /// <param name="factory_id"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static List<Inspections_documents> GetByInspId(int insp_id=0, string factory_code="", string customer_code="")
        {
            var result = new List<Inspections_documents>();
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(@"SELECT
                    inspection_lines_tested.insp_factory_ref,
                    inspection_lines_tested.insp_client_ref,
                    inspection_lines_tested.insp_client_desc,
                    cust_products.cprod_instructions,
                    cust_products.cprod_label,
                    cust_products.cprod_packaging,
                    cust_products.cprod_dwg,
                    mast_products.asaq_ref,
                    mast_products.prod_length,
                    mast_products.prod_height,
                    mast_products.prod_width,
                    mast_products.prod_nw,
                    mast_products.pack_width,
                    mast_products.pack_depth,
                    mast_products.pack_height,
                    mast_products.pack_GW,
                    mast_products.prod_image3,
                    mast_products.prod_instructions,
                    mast_products.prod_image4,
                    mast_products.prod_image5,
                    mast_products.prod_image2,
                    mast_products.prod_image1,                    
                    inspection_lines_tested.insp_line_unique,
                    inspection_lines_tested.insp_id,
                    inspection_lines_tested.insp_qty,
                    inspection_lines_tested.insp_override_qty,
                    inspection_lines_tested.order_linenum,
                    mast_products.mast_id,
                    cust_products.cprod_id,
                    inspections.insp_unique as inspection_unique,
                    inspections.insp_id as inspection_id,
                    inspections.factory_code,
                    inspections.customer_code
                    FROM
                    inspection_lines_tested
                    INNER JOIN order_lines ON inspection_lines_tested.order_linenum = order_lines.linenum
                    INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                    INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                    INNER JOIN inspections ON inspections.insp_unique = inspection_lines_tested.insp_id
                    WHERE {0}", insp_id > 0?"inspection_lines_tested.insp_id = @insp_id":"inspections.factory_code = @factory_code and inspections.customer_code = @customer_code"), conn);
                cmd.Parameters.AddWithValue("@insp_id", insp_id);
                cmd.Parameters.AddWithValue("@factory_code",factory_code);
                cmd.Parameters.AddWithValue("@customer_code", customer_code);
                var dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    var line = GetDocFromDataReader(dr);
                    result.Add(line);
                }
                dr.Close();
            }


            return result;
        }
        public static List<Inspections_documents> GetByFactoryRef(int insp_id)
        {
            var  result= new List<Inspections_documents>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(@"SELECT
                    inspection_lines_tested.insp_factory_ref,
                    inspection_lines_tested.insp_client_ref,
                    inspection_lines_tested.insp_client_desc,
                    inspection_lines_tested.order_linenum,
                    cust_products.cprod_instructions,
                    cust_products.cprod_label,
                    cust_products.cprod_packaging,
                    cust_products.cprod_dwg,
                    mast_products.asaq_ref,
                    inspection_lines_tested.insp_id,
                    inspection_lines_tested.insp_qty,
                    mast_products.mast_id,
                    cust_products.cprod_id,
                    mast_products.prod_image2,
                    mast_products.prod_image3,
                    mast_products.prod_image4,
                    mast_products.prod_image5,
                    inspection_lines_tested.insp_id as inspection_id
                    FROM
                    inspection_lines_tested
                    INNER JOIN mast_products ON inspection_lines_tested.insp_factory_ref = mast_products.factory_ref
                    INNER JOIN cust_products ON cust_products.cprod_mast = mast_products.mast_id
                    WHERE
                    inspection_lines_tested.insp_id = @insp_id AND inspection_lines_tested.order_linenum = 0 ", conn);
                cmd.Parameters.AddWithValue("@insp_id",insp_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetDocFromDataReader(dr);
                    result.Add(line);
                }
                dr.Close();
            }
                return result;
        }

        public static List<Inspections_documents> GetByProducts(int factory_id, int customer_id)
        {
            var result = new List<Inspections_documents>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand(string.Format(
                    @"SELECT
                                cust_product_details.cprod_id,
                                cust_product_details.factory_code,
                                cust_product_details.factory_id,
                                cust_product_details.cprod_user,
                                cust_product_details.cprod_code1,
                                cust_product_details.cprod_name,
                                cust_product_details.factory_ref,
                                cust_product_details.cprod_label,
                                cust_product_details.cprod_dwg,
                                users.customer_code,
                                mast_products.prod_image2,
                                mast_products.prod_image3,
                                mast_products.prod_image4,
                                mast_products.prod_image5,
                                mast_products.prod_instructions,
                                mast_products.mast_id
                                FROM
                                cust_product_details
                                INNER JOIN users ON cust_product_details.cprod_user = users.user_id
                                INNER JOIN mast_products ON mast_products.mast_id = cust_product_details.mast_id
                                 where cust_product_details.factory_id = @factory_code 
	                                and cust_product_details.cprod_user = @customer_code
                              "), conn);
                
                cmd.Parameters.AddWithValue("@factory_code", factory_id);

                cmd.Parameters.AddWithValue("@customer_code", customer_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    //var line = GetDocFromDataReader(dr);

                    result.Add(new Inspections_documents

                    {
                        insp_factory_ref=string.Empty + dr["factory_ref"],
                        insp_client_ref=string.Empty + dr["cprod_code1"],
                        insp_client_desc=string.Empty +dr["cprod_name"],
                        cprod_label=string.Empty +dr["cprod_label"],
                        cprod_dwg=string.Empty + dr["cprod_dwg"],
                        prod_image2 = string.Empty + dr["prod_image2"],
                        prod_image3 = string.Empty + dr["prod_image3"],
                        prod_image4 = string.Empty + dr["prod_image4"],
                        prod_image5 = string.Empty + dr["prod_image5"],
                        prod_instructions = string.Empty + dr["prod_instructions"],
                        mast_id=(int)dr["mast_id"],
                        cprod_id=(int)dr["cprod_id"]




                    });
                }
                dr.Close();
            }


            return result;
        }

        private static Inspections_documents GetDocFromDataReader(MySqlDataReader dr)
        {
            Inspections_documents o = new Inspections_documents();
            o.insp_factory_ref = string.Empty + Utilities.GetReaderField(dr, "insp_factory_ref");
            o.insp_client_ref = string.Empty + Utilities.GetReaderField(dr, "insp_client_ref");
            o.insp_client_desc = string.Empty + Utilities.GetReaderField(dr, "insp_client_desc");
            o.cprod_instructions = string.Empty + Utilities.GetReaderField(dr, "cprod_instructions");
            o.cprod_label= string.Empty + Utilities.GetReaderField(dr, "cprod_label");
            o.cprod_packaging = string.Empty + Utilities.GetReaderField(dr, "cprod_packaging");
            o.cprod_dwg = string.Empty + Utilities.GetReaderField(dr, "cprod_dwg");
            o.asaq_ref = string.Empty + Utilities.GetReaderField(dr, "asaq_ref");
            //o.prod_length = (int)dr["prod_length"];
            //o.prod_height = (int)dr["prod_height"];
            //o.prod_width = (int)dr["prod_width "];
            o.insp_id = (int)dr["insp_id"];
            o.insp_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "insp_qty")); ;
            o.mast_id = (int)dr["mast_id"];
            o.cprod_id = (int)dr["cprod_id"];

            o.prod_instructions = string.Empty + Utilities.GetReaderField(dr, "prod_instructions");
            o.prod_image1 = string.Empty + Utilities.GetReaderField(dr, "prod_image1");
            o.prod_image2 = string.Empty + Utilities.GetReaderField(dr, "prod_image2");
            o.prod_image3 = string.Empty + Utilities.GetReaderField(dr, "prod_image3");
            o.prod_image4 = string.Empty + Utilities.GetReaderField(dr, "prod_image4");
            o.prod_image5 = string.Empty + Utilities.GetReaderField(dr, "prod_image5");
            
            o.inspection_id = string.Empty + Utilities.GetReaderField(dr, "inspection_id");

            return o;
        }
		
	
		private static Inspection_lines_tested GetFromDataReader(MySqlDataReader dr)
		{
			Inspection_lines_tested o = new Inspection_lines_tested();
		
			o.insp_line_unique =  (int) dr["insp_line_unique"];
			o.insp_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"insp_id"));
			o.insp_factory_ref = string.Empty + Utilities.GetReaderField(dr,"insp_factory_ref");
			o.insp_client_ref = string.Empty + Utilities.GetReaderField(dr,"insp_client_ref");
			o.insp_client_desc = string.Empty + Utilities.GetReaderField(dr,"insp_client_desc");
			o.insp_qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"insp_qty"));
			o.insp_override_qty = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr,"insp_override_qty"));
			o.insp_custpo = string.Empty + Utilities.GetReaderField(dr,"insp_custpo");
			o.order_linenum = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"order_linenum"));
			o.photo_confirm = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirm"));
			o.photo_confirma = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirma"));
			o.photo_confirmm = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmm"));
			o.photo_confirmd = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmd"));
			o.photo_confirmf = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmf"));
			o.photo_confirmp = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"photo_confirmp"));
			o.packaging_rej = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"packaging_rej"));
			o.label_rej = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"label_rej"));
			o.instructions_rej = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"instructions_rej"));

            

            return o;

		}
		
		
		public static void Create(Inspection_lines_tested o)
        {
            string insertsql = @"INSERT INTO inspection_lines_tested (insp_id,insp_factory_ref,insp_client_ref,insp_client_desc,insp_qty,insp_override_qty,insp_custpo,order_linenum,photo_confirm,photo_confirma,photo_confirmm,photo_confirmd,photo_confirmf,photo_confirmp,packaging_rej,label_rej,instructions_rej) VALUES(@insp_id,@insp_factory_ref,@insp_client_ref,@insp_client_desc,@insp_qty,@insp_override_qty,@insp_custpo,@order_linenum,@photo_confirm,@photo_confirma,@photo_confirmm,@photo_confirmd,@photo_confirmf,@photo_confirmp,@packaging_rej,@label_rej,@instructions_rej)";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				
				var cmd = Utils.GetCommand(insertsql, conn);
                BuildSqlParameters(cmd,o);
                cmd.ExecuteNonQuery();
				cmd.CommandText = "SELECT insp_line_unique FROM inspection_lines_tested WHERE insp_line_unique = LAST_INSERT_ID()";
                o.insp_line_unique = (int) cmd.ExecuteScalar();
				
            }
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Inspection_lines_tested o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@insp_line_unique", o.insp_line_unique);
			cmd.Parameters.AddWithValue("@insp_id", o.insp_id);
			cmd.Parameters.AddWithValue("@insp_factory_ref", o.insp_factory_ref);
			cmd.Parameters.AddWithValue("@insp_client_ref", o.insp_client_ref);
			cmd.Parameters.AddWithValue("@insp_client_desc", o.insp_client_desc);
			cmd.Parameters.AddWithValue("@insp_qty", o.insp_qty);
			cmd.Parameters.AddWithValue("@insp_override_qty", o.insp_override_qty);
			cmd.Parameters.AddWithValue("@insp_custpo", o.insp_custpo);
			cmd.Parameters.AddWithValue("@order_linenum", o.order_linenum);
			cmd.Parameters.AddWithValue("@photo_confirm", o.photo_confirm);
			cmd.Parameters.AddWithValue("@photo_confirma", o.photo_confirma);
			cmd.Parameters.AddWithValue("@photo_confirmm", o.photo_confirmm);
			cmd.Parameters.AddWithValue("@photo_confirmd", o.photo_confirmd);
			cmd.Parameters.AddWithValue("@photo_confirmf", o.photo_confirmf);
			cmd.Parameters.AddWithValue("@photo_confirmp", o.photo_confirmp);
			cmd.Parameters.AddWithValue("@packaging_rej", o.packaging_rej);
			cmd.Parameters.AddWithValue("@label_rej", o.label_rej);
			cmd.Parameters.AddWithValue("@instructions_rej", o.instructions_rej);
		}
		
		public static void Update(Inspection_lines_tested o)
		{
			string updatesql = @"UPDATE inspection_lines_tested SET insp_id = @insp_id,insp_factory_ref = @insp_factory_ref,insp_client_ref = @insp_client_ref,insp_client_desc = @insp_client_desc,insp_qty = @insp_qty,insp_override_qty = @insp_override_qty,insp_custpo = @insp_custpo,order_linenum = @order_linenum,photo_confirm = @photo_confirm,photo_confirma = @photo_confirma,photo_confirmm = @photo_confirmm,photo_confirmd = @photo_confirmd,photo_confirmf = @photo_confirmf,photo_confirmp = @photo_confirmp,packaging_rej = @packaging_rej,label_rej = @label_rej,instructions_rej = @instructions_rej WHERE insp_line_unique = @insp_line_unique";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int insp_line_unique)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				var cmd = Utils.GetCommand("DELETE FROM inspection_lines_tested WHERE insp_line_unique = @id" , conn);
                cmd.Parameters.AddWithValue("@id", insp_line_unique);
                cmd.ExecuteNonQuery();
            }
		}
		
		
	}
}
			
			