
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using asaq2.Model.Properties;

namespace asaq2.Model
{
    public class Order_linesDAL
	{
	
		public static List<Order_lines> GetByOrderId(int order_id)
		{
			var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT order_lines.*, cust_products.*,order_header.*, mast_products.*, factory.*,
                                (CASE order_header.stock_order 
                                    WHEN 1 THEN 
                                       DATE_ADD(order_header.orderdate,INTERVAL (CASE WHEN mast_products.factory_id IN (16,18) THEN 4 ELSE 6 END) MONTH)
                                    ELSE COALESCE(porder_header.po_req_etd, DATE_ADD(order_header.req_eta, INTERVAL -35 DAY)) END) AS po_req_etd
                                    FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
                                RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN order_header ON 
                                    order_lines.orderid = order_header.orderid INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                                    WHERE order_lines.orderid = @order_id", conn);
                cmd.Parameters.AddWithValue("@order_id", order_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    result.Add(line);
                    line.Header = Order_headerDAL.GetFromDataReader(dr);
                    line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                    if (line.Cust_Product.cprod_mast != null)
                    {
                        line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);// Mast_productsDAL.GetById(line.Cust_Product.cprod_mast.Value);
                        if (line.Cust_Product.MastProduct.factory_id != null)
                            line.Cust_Product.MastProduct.Factory = CompanyDAL.GetFromDataReader(dr);
                                //CompanyDAL.GetById(line.Cust_Product.MastProduct.factory_id.Value);
                    }
                }
                dr.Close();
            }
            return result;
		}

        /***/
        public static List<Order_lines> GetByOrder()
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT " +
                                               "Count(order_line_detail2_v6.orderid) AS orderFreq , " +
                                                "order_line_detail2_v6.cprod_id " +
                                                "FROM order_line_detail2_v6 " +
                                           "INNER JOIN cust_products ON cust_products.cprod_id = order_line_detail2_v6.cprod_id  " +
                                           "WHERE order_line_detail2_v6.orderqty > 0 " +
                                               "AND order_line_detail2_v6.req_eta >= DATE_SUB(NOW(),INTERVAL 12 MONTH) " +
                                               "AND order_line_detail2_v6.stock_order IN (8) " +
                                               "AND cust_products.cprod_user IN (45,55,149) " +
                                           "GROUP BY order_line_detail2_v6.cprod_id", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Order_lines
                        {
                            orderid = Convert.ToInt32(dr["orderFreq"]),
                            cprod_id = Convert.ToInt32(dr["cprod_id"])

                        });
                }
                dr.Close();
            }



            return result;
        } 


        /***/


        public static List<Order_lines> GetByCustPo(string custPo, int? company_id = null)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT order_lines.*, cust_products.*,order_header.*,
                            (CASE order_header.stock_order 
                                    WHEN 1 THEN 
                                       DATE_ADD(order_header.orderdate,INTERVAL (CASE WHEN mast_products.factory_id IN (16,18) THEN 4 ELSE 6 END) MONTH)
                                    ELSE COALESCE(porder_header.po_req_etd, DATE_ADD(order_header.req_eta, INTERVAL -35 DAY)) END) AS po_req_etd
                                                        FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
                                RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN order_header ON 
                                                        order_lines.orderid = order_header.orderid INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                                        WHERE order_header.custpo = @custpo AND (order_header.userid1 = @company_id OR @company_id IS NULL)", conn);
                cmd.Parameters.AddWithValue("@custpo", custPo);
                cmd.Parameters.AddWithValue("@company_id", company_id != null ? (object) company_id : DBNull.Value);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    result.Add(line);
                    line.Header = Order_headerDAL.GetFromDataReader(dr);
                    line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                    if (line.Cust_Product.cprod_mast != null)
                    {
                        line.Cust_Product.MastProduct = Mast_productsDAL.GetById(line.Cust_Product.cprod_mast.Value);
                        if (line.Cust_Product.MastProduct.factory_id != null)
                            line.Cust_Product.MastProduct.Factory =
                                CompanyDAL.GetById(line.Cust_Product.MastProduct.factory_id.Value);
                    }
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetByCustPos(string[] custPos)
        {
            var result = new List<Order_lines>();
            if (custPos.Length > 0)
            {
                using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(string.Empty, conn);
                    var sql =
                        string.Format(@"SELECT order_lines.*, cust_products.*, order_header.*,(SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd,
                                                        porder_header.special_comments, mast_products.factory_ref,mast_products.special_comments AS product_special_comments
                                                        FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                                                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN order_header ON 
                                                        order_lines.orderid = order_header.orderid INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum INNER JOIN 
                                                        porder_header ON porder_lines.porderid = porder_header.porderid
                                                        WHERE order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) 
                                                        AND order_header.custpo IN ({0})",
                                      Utilities.CreateParametersFromIdList(cmd, new List<string>(custPos)));
                    cmd.CommandText = sql;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var line = GetFromDataReader(dr);
                        line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                        line.Header = Order_headerDAL.GetFromDataReader(dr);
                        line.POHeader = new Porder_header {special_comments = string.Empty + dr["special_comments"]};
                        line.Cust_Product.MastProduct = new Mast_products
                            {
                                special_comments = string.Empty + dr["product_special_comments"],
                                factory_ref = string.Empty + dr["factory_ref"]
                            };
                        result.Add(line);
                    }
                    dr.Close();
                }
            }
            return result;
        }

        public static List<Order_lines> GetByCriteria(int? month, int? year, string criteria, int company_id)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Settings.Default.ConnString))
            {
                conn.Open();
                DateTime? date = null;
                if(month != null)
                    date = new DateTime(year.Value, month.Value,1);
                var cmd = new MySqlCommand(@"SELECT users.consolidated_port, order_lines.cprod_id, cust_products.cprod_name, cust_products.cprod_code1,
    (SELECT line.unitprice FROM order_lines line INNER JOIN order_header oh ON line.orderid = oh.orderid 
        WHERE cprod_id = order_lines.cprod_id AND (oh.req_eta < @date OR @date IS NULL) ORDER BY oh.req_eta DESC LIMIT 1)  AS unitprice
                                                    FROM
                                                      order_lines INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                                                    INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                                    INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                    INNER JOIN users ON mast_products.factory_id = users.user_id
                                                    WHERE (order_header.req_eta < @date OR @date IS NULL) AND (cprod_name LIKE @criteria OR cprod_code1 LIKE @criteria) AND order_header.userid1 = @company_id
                                                    AND order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) AND (cust_products.cprod_returnable = 0) 
                                                    AND order_header.req_eta < now() AND order_header.req_eta > (now() - interval 24 month)
                                                    GROUP BY order_lines.cprod_id,cust_products.cprod_name,cust_products.cprod_code1,users.consolidated_port", conn);
                cmd.Parameters.AddWithValue("@date", date != null ? (object) date : DBNull.Value);
                cmd.Parameters.AddWithValue("@criteria", "%" + criteria + "%");
                cmd.Parameters.AddWithValue("@company_id", company_id);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Order_lines
                        {
                            cprod_id = (int)dr["cprod_id"],
                            unitprice = (double)dr["unitprice"],
                            Cust_Product = new Cust_products
                            {
                                cprod_code1 = string.Empty + dr["cprod_code1"],
                                cprod_name = string.Empty + dr["cprod_name"],
                                consolidated_port = (int)dr["consolidated_port"]
                            }
                        });
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetForProductAndCriteria(int? month, int? year, int cprod_id, int company_id)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                DateTime? date = null;
                if (month != null)
                    date = new DateTime(year.Value, month.Value, 1);
                var cmd = new MySqlCommand(@"SELECT order_lines.cprod_id, cust_products.cprod_name, cust_products.cprod_code1,order_header.custpo,order_header.orderid,
                                                order_lines.unitprice, order_lines.orderqty
                                                    FROM
                                                      order_lines INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
                                                    INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                                    WHERE (order_header.req_eta < @date OR @date IS NULL) AND cust_products.cprod_id =@cprod_id AND order_header.userid1 = @company_id
                                                    AND order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) AND (cust_products.cprod_returnable = 0) 
                                                    AND order_header.req_eta < now() AND order_header.req_eta > (now() - interval 24 month)
                                                    ", conn);
                cmd.Parameters.AddWithValue("@date", date != null ? (object)date : DBNull.Value);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                cmd.Parameters.AddWithValue("@company_id", company_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(new Order_lines
                    {
                        cprod_id = (int)dr["cprod_id"],
                        unitprice = (double)dr["unitprice"],
                        orderqty = Utilities.FromDbValue<double>(dr["orderqty"]),
                        Cust_Product = new Cust_products
                        {
                            cprod_code1 = string.Empty + dr["cprod_code1"],
                            cprod_name = string.Empty + dr["cprod_name"]
                        },
                        Header = new Order_header { orderid = (int) dr["orderid"], custpo = string.Empty + dr["custpo"]}
                    });
                }
                dr.Close();
            }
            return result;
        }

        public static int GetNumOfPreviousShipments(DateTime po_req_etd, int cprod_id)
        {
            int result = 0;
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd =
                    new MySqlCommand(@"SELECT COUNT(*) AS numOfLines FROM order_lines INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum 
                                             INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid WHERE order_lines.cprod_id = @cprod_id 
                                             AND porder_header.po_req_etd < @po_req_etd", conn);
                cmd.Parameters.AddWithValue("@po_req_etd", po_req_etd);
                cmd.Parameters.AddWithValue("@cprod_id", cprod_id);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return result;
        }


        public static
            Order_lines GetById(int id)
		{
			Order_lines result = null;
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM order_lines WHERE linenum = @id", conn);
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
	
		public static Order_lines GetFromDataReader(MySqlDataReader dr)
		{
			Order_lines o = new Order_lines();
		
			o.linenum =  (int) dr["linenum"];
			o.orderid = Utilities.FromDbValue<int>(dr["orderid"]);
			o.original_orderid = Utilities.FromDbValue<int>(dr["original_orderid"]);
			o.linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]);
			o.cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]);
			o.description = string.Empty + dr["description"];
			o.spec_code = string.Empty + dr["spec_code"];
			o.orderqty = Utilities.FromDbValue<double>(dr["orderqty"]);
			o.orig_orderqty = Utilities.FromDbValue<double>(dr["orig_orderqty"]);
			o.unitprice = Utilities.FromDbValue<double>(dr["unitprice"]);
			o.unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]);
			o.linestatus = Utilities.FromDbValue<int>(dr["linestatus"]);
			o.lineunit = Utilities.FromDbValue<int>(dr["lineunit"]);
			o.factory_group = Utilities.FromDbValue<int>(dr["factory_group"]);
			o.mc_qty = Utilities.FromDbValue<int>(dr["mc_qty"]);
			o.pallet_qty = Utilities.FromDbValue<int>(dr["pallet_qty"]);
			o.unit_qty = Utilities.FromDbValue<int>(dr["unit_qty"]);
			o.lme = Utilities.FromDbValue<double>(dr["lme"]);
			o.allow_change = Utilities.FromDbValue<int>(dr["allow_change"]);
			o.allow_change_down = Utilities.FromDbValue<int>(dr["allow_change_down"]);
			o.fi_line = Utilities.FromDbValue<int>(dr["fi_line"]);
			o.pack_qty = Utilities.FromDbValue<double>(dr["pack_qty"]);
			o.li_line = Utilities.FromDbValue<int>(dr["li_line"]);
            //if (Utilities.ColumnExists(dr, "cprod_mast"))
            //    o.cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]);
            //if (Utilities.ColumnExists(dr, "cprod_code1"))
            //    o.cprod_code1 = string.Empty + dr["cprod_code1"];
            //if (Utilities.ColumnExists(dr, "cprod_name"))
            //    o.cprod_name = string.Empty + dr["cprod_name"];
			return o;

		}
		
		public static void Create(Order_lines o, MySqlTransaction  tr = null)
        {
            string insertsql = @"INSERT INTO order_lines (orderid,original_orderid,linedate,cprod_id,description,spec_code,orderqty,orig_orderqty,unitprice,unitcurrency,linestatus,lineunit,factory_group,mc_qty,pallet_qty,unit_qty,lme,allow_change,allow_change_down,fi_line,pack_qty,li_line) VALUES(@orderid,@original_orderid,@linedate,@cprod_id,@description,@spec_code,@orderqty,@orig_orderqty,@unitprice,@unitcurrency,@linestatus,@lineunit,@factory_group,@mc_qty,@pallet_qty,@unit_qty,@lme,@allow_change,@allow_change_down,@fi_line,@pack_qty,@li_line)";

		    var conn = tr == null ? new MySqlConnection(Properties.Settings.Default.ConnString) : tr.Connection;
			if(tr == null)
                conn.Open();
            	
			MySqlCommand cmd = new MySqlCommand(insertsql, conn,tr);
            BuildSqlParameters(cmd,o);
            cmd.ExecuteNonQuery();
			cmd.CommandText = "SELECT linenum FROM order_lines WHERE linenum = LAST_INSERT_ID()";
            o.linenum = (int) cmd.ExecuteScalar();
				
            
		}
		
		private static void BuildSqlParameters(MySqlCommand cmd, Order_lines o, bool forInsert = true)
        {
			
			if(!forInsert)
				cmd.Parameters.AddWithValue("@linenum", o.linenum);
			cmd.Parameters.AddWithValue("@orderid", o.orderid);
			cmd.Parameters.AddWithValue("@original_orderid", o.original_orderid);
			cmd.Parameters.AddWithValue("@linedate", o.linedate);
			cmd.Parameters.AddWithValue("@cprod_id", o.cprod_id);
			cmd.Parameters.AddWithValue("@description", o.description);
			cmd.Parameters.AddWithValue("@spec_code", o.spec_code);
			cmd.Parameters.AddWithValue("@orderqty", o.orderqty);
			cmd.Parameters.AddWithValue("@orig_orderqty", o.orig_orderqty);
			cmd.Parameters.AddWithValue("@unitprice", o.unitprice);
			cmd.Parameters.AddWithValue("@unitcurrency", o.unitcurrency);
			cmd.Parameters.AddWithValue("@linestatus", o.linestatus);
			cmd.Parameters.AddWithValue("@lineunit", o.lineunit);
			cmd.Parameters.AddWithValue("@factory_group", o.factory_group);
			cmd.Parameters.AddWithValue("@mc_qty", o.mc_qty);
			cmd.Parameters.AddWithValue("@pallet_qty", o.pallet_qty);
			cmd.Parameters.AddWithValue("@unit_qty", o.unit_qty);
			cmd.Parameters.AddWithValue("@lme", o.lme);
			cmd.Parameters.AddWithValue("@allow_change", o.allow_change);
			cmd.Parameters.AddWithValue("@allow_change_down", o.allow_change_down);
			cmd.Parameters.AddWithValue("@fi_line", o.fi_line);
			cmd.Parameters.AddWithValue("@pack_qty", o.pack_qty);
			cmd.Parameters.AddWithValue("@li_line", o.li_line);
		}
		
		public static void Update(Order_lines o)
		{
			string updatesql = @"UPDATE order_lines SET orderid = @orderid,original_orderid = @original_orderid,linedate = @linedate,cprod_id = @cprod_id,description = @description,spec_code = @spec_code,orderqty = @orderqty,orig_orderqty = @orig_orderqty,unitprice = @unitprice,unitcurrency = @unitcurrency,linestatus = @linestatus,lineunit = @lineunit,factory_group = @factory_group,mc_qty = @mc_qty,pallet_qty = @pallet_qty,unit_qty = @unit_qty,lme = @lme,allow_change = @allow_change,allow_change_down = @allow_change_down,fi_line = @fi_line,pack_qty = @pack_qty,li_line = @li_line WHERE linenum = @linenum";

			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand(updatesql, conn);
                BuildSqlParameters(cmd,o, false);
                cmd.ExecuteNonQuery();
            }
		}
		
		public static void Delete(int linenum)
		{
			using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM order_lines WHERE linenum = @id" , conn);
                cmd.Parameters.AddWithValue("@id", linenum);
                cmd.ExecuteNonQuery();
            }
		}

        public static List<Order_lines> GetByOrderIds(List<int> order_ids)
        {
            List<Order_lines> result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("",conn);
                cmd.CommandText = string.Format(@"SELECT order_lines.*, cust_products.*,order_header.*,mast_products.*
                                                        FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                                                        INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                                        WHERE order_lines.orderid IN ({0})",
                                                Utilities.CreateParametersFromIdList(cmd, order_ids));
                //cmd.Parameters.AddWithValue("@order_id", order_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                    line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    line.Header = Order_headerDAL.GetFromDataReader(dr);
                    result.Add(line);
                    
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetByExportCriteria(IList<int> cprod_ids=null, List<int> mast_ids=null, int? client_id=null, DateTime? etd_from = null,DateTime? etd_to=null, int? factory_id=null, List<string> custpoList = null)
        {
            List<Order_lines> result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                string sql = @"SELECT order_lines.*, cust_products.*, mast_products.*, order_header.*,users.*,
                              (SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid AND (userid = @factory_id OR @factory_id IS NULL)) AS po_req_etd,
                              (SELECT MAX(original_po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid AND (userid = @factory_id OR @factory_id IS NULL)) AS original_po_req_etd,
                              (SELECT GROUP_CONCAT(special_comments, ', ') FROM porder_header WHERE soorderid = order_header.orderid AND (userid = @factory_id OR @factory_id IS NULL)) AS po_comments
                              FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                              INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                              INNER JOIN users ON order_header.userid1 = users.user_id
                              INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast";
                if (cprod_ids != null)
                    sql += string.Format(" WHERE order_lines.cprod_id IN ({0})",
                                         Utilities.CreateParametersFromIdList(cmd, cprod_ids));
                else if(mast_ids != null)
                {
                    sql += string.Format(" WHERE cust_products.cprod_mast IN ({0})",
                                         Utilities.CreateParametersFromIdList(cmd, mast_ids));
                }
                if (client_id != null)
                {
                    if(client_id != -1)
                        sql += " AND (order_header.userid1 = @client_id) ";
                    else
                    {
                        sql += " AND (order_header.userid1 IN (SELECT user_id FROM users WHERE distributor > 0))";
                    }
                }
                if (custpoList != null && custpoList.Count > 0)
                {
                    var parts = new List<string>();
                    for (int i = 0; i < custpoList.Count; i++)
                    {
                        cmd.Parameters.AddWithValue(string.Format("@custpo{0}", i + 1),
                                                                "%" + custpoList[i] + "%");
                        parts.Add(string.Format(" order_header.custpo LIKE @custpo{0} ",i+1));
                    }
                    sql += " AND (" + string.Join(" OR ", parts) + ")";
                }
                sql += " AND order_header.status NOT IN ('X','Y') ";
                if (etd_from != null)
                    sql += " AND COALESCE((SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid AND (userid = @factory_id OR @factory_id IS NULL)),'1900-1-1') >= @etd_from";
                if (etd_to != null)
                    sql += " AND COALESCE((SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid AND (userid = @factory_id OR @factory_id IS NULL)),'2099-1-1') <= @etd_to";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@etd_from", etd_from != null ? (object) etd_from : DBNull.Value );
                cmd.Parameters.AddWithValue("@etd_to", etd_to != null ? (object) etd_to : DBNull.Value);
                cmd.Parameters.AddWithValue("@factory_id", factory_id);
                cmd.Parameters.AddWithValue("@client_id", client_id);
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                    line.Header = Order_headerDAL.GetFromDataReader(dr);
                    line.Header.Client = CompanyDAL.GetFromDataReader(dr);
                    line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(line);
                    
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids, int? client_id,
                                                           DateTime? eta_from, DateTime? eta_to, int? factory_id)
        {
            return GetByProdIdsAndETA(cprod_ids, mast_ids,client_id != null ? new[] {client_id.Value}.ToList() : null, eta_from, eta_to, factory_id);
        }

        public static List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids,List<int> client_ids = null, DateTime? eta_from = null, DateTime? eta_to = null, int? factory_id = null)
        {
            List<Order_lines> result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                string sql = @"SELECT order_lines.*, cust_products.*, mast_products.*, order_header.*
                              FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                              INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                              INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast";
                if (cprod_ids != null)
                    sql += string.Format(" WHERE order_lines.cprod_id IN ({0})",
                                         Utilities.CreateParametersFromIdList(cmd, cprod_ids));
                else
                {
                    sql += string.Format(" WHERE cust_products.cprod_mast IN ({0})",
                                         Utilities.CreateParametersFromIdList(cmd, mast_ids));
                }
                sql += " AND order_header.stock_order <> 1 ";
                if (client_ids != null && client_ids.Count > 0)
                {
                    if (client_ids[0] != -1)
                    {
                        sql += string.Format(" AND (order_header.userid1 IN ({0})) ",Utilities.CreateParametersFromIdList(cmd,client_ids,"clientid"));
                    }
                    else
                    {
                        sql += " AND (order_header.userid1 IN (SELECT user_id FROM users WHERE distributor > 0))";
                        cmd.Parameters.AddWithValue("@client_id", client_ids[0]);
                    }
                }
                sql += " AND order_header.status NOT IN ('X','Y') ";
                if (eta_from != null)
                    sql += " AND order_header.req_eta >= @etd_from";
                if (eta_to != null)
                    sql += " AND order_header.req_eta <= @etd_to";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@etd_from", eta_from);
                cmd.Parameters.AddWithValue("@etd_to", eta_to);
                cmd.Parameters.AddWithValue("@factory_id", factory_id);
                
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = GetFromDataReader(dr);
                    line.Cust_Product = Cust_productsDAL.GetFromDataReader(dr);
                    line.Header = Order_headerDAL.GetFromDataReader(dr);
                    line.Cust_Product.MastProduct = Mast_productsDAL.GetFromDataReader(dr);
                    result.Add(line);

                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetStockOrderLinesInFactory(IList<int> cprod_ids)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT order_lines.linenum,porder_header.po_req_etd, order_lines.cprod_id, order_lines.orderqty, order_header.req_eta,order_header.orderdate
                                    FROM order_lines
                                    INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                                    INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                    INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                    WHERE order_header.stock_order = 1 AND order_lines.cprod_id IN ({0})
                                    AND porder_header.po_req_etd > NOW() 
                                   OR (order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum) > 0)
                                    OR EXISTS (SELECT co_lines.linenum FROM order_lines co_lines INNER JOIN stock_order_allocation so ON co_lines.linenum = so.so_line INNER JOIN porder_lines co_pline
                                        ON co_lines.linenum = co_pline.soline INNER JOIN porder_header co_pheader ON co_pline.porderid = co_pheader.porderid WHERE so.st_line = order_lines.linenum AND so.alloc_qty > 0
                                         AND co_pheader.po_req_etd > NOW())
                                    ", Utilities.CreateParametersFromIdList(cmd,cprod_ids));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = new Order_lines{linenum = (int) dr["linenum"], Header = new Order_header{po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]),orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"])},
                                            cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),orderqty = Utilities.FromDbValue<double>(dr["orderqty"])};
                    result.Add(line);
                }
                foreach (var line in result)
                {
                    line.AllocatedLines = Stock_order_allocationDAL.GetAllocationCalloffLines(line.linenum);
                }
                dr.Close();
            }
            return result;
        }

        public static List<Order_lines> GetCalloffOrdersInShipment(IList<int> cprod_ids)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText = string.Format(@"SELECT order_lines.linenum,porder_header.po_req_etd, order_lines.cprod_id, order_lines.orderqty, order_header.req_eta
                                    FROM order_lines
                                    INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                                    INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                    INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                    WHERE order_header.stock_order <> 1 AND order_lines.cprod_id IN ({0})
                                    AND porder_header.po_req_etd < NOW() AND order_header.req_eta > NOW() ", Utilities.CreateParametersFromIdList(cmd, cprod_ids));
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = new Order_lines
                    {
                        linenum = (int)dr["linenum"],
                        Header = new Order_header { po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]), req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]) },
                        cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
                        orderqty = Utilities.FromDbValue<double>(dr["orderqty"])
                    };
                    result.Add(line);
                }
                foreach (var line in result)
                {
                    line.AllocatedLines = Order_linesDAL.GetAllocationCalloffLines(line.linenum);
                }
                dr.Close();
            }
            return result;
        }

        private static List<Order_lines> GetAllocationCalloffLines(int linenum)
        {
            var result = new List<Order_lines>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT order_lines.linenum,order_lines.orderqty,order_header.req_eta,stock_order_allocation.alloc_qty,porder_header.po_req_etd,order_lines.cprod_id
                                        FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.so_line 
                                        INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                        INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
                                        INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
                                        WHERE st_line = @stockline", conn);
                cmd.Parameters.AddWithValue("@stockline", linenum);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var line = new Order_lines
                    {
                        linenum = (int)dr["linenum"],
                        Header = new Order_header { po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]), req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]) },
                        AllocQty = Utilities.FromDbValue<int>(dr["alloc_qty"]),
                        cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
                        orderqty = Utilities.FromDbValue<double>(dr["orderqty"])
                    };
                    result.Add(line);
                }
            }
            return result;
        }
	}
}
			
			