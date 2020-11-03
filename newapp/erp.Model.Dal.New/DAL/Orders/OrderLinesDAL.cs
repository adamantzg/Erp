
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
	public enum OrderDateMode
	{
		Etd,
		Eta,
		Sale,
		EtaPlusWeek,
		OrderDate
	}

	public class OrderLinesDAL : GenericDal<Order_lines>, IOrderLinesDAL
	{
		private IMastProductsDal mastProductsDal;
		private ICompanyDAL companyDAL;
		private IStockOrderAllocationDAL stockOorderAllocationDal;
		private IOrderLinesManualDAL orderLinesManualDal;
		private ICountriesDAL countriesDAL;

		public OrderLinesDAL(IDbConnection conn, IMastProductsDal mastProductsDal, ICompanyDAL companyDAL, 
			IStockOrderAllocationDAL stockOorderAllocationDal, IOrderLinesManualDAL orderLinesManualDal,
			ICountriesDAL countriesDAL) : base(conn)
		{
			this.conn = (MySqlConnection) conn;
			this.mastProductsDal = mastProductsDal;
			this.companyDAL = companyDAL;
			this.stockOorderAllocationDal = stockOorderAllocationDal;
			this.orderLinesManualDal = orderLinesManualDal;
			this.countriesDAL = countriesDAL;
		}

		public const string LinesJoin =
			@"order_lines INNER JOIN order_header ON order_lines.orderid = order_header.orderid 
										  INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
										  INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid";
	
		public List<Order_lines> GetByOrderId(int order_id)
		{
			return conn.Query<Order_header, Order_lines, Cust_products, Mast_products, Company, Order_lines>(
				@"SELECT order_lines.*, cust_products.*,order_header.*, mast_products.*, factory.*,
					(CASE order_header.stock_order 
						WHEN 1 THEN 
							DATE_ADD(order_header.orderdate,INTERVAL (CASE WHEN mast_products.factory_id IN (16,18) THEN 4 ELSE 6 END) MONTH)
						ELSE COALESCE(porder_header.po_req_etd, DATE_ADD(order_header.req_eta, INTERVAL -35 DAY)) END) AS po_req_etd
						FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
					RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN order_header ON 
						order_lines.orderid = order_header.orderid INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN users factory ON mast_products.factory_id = factory.user_id
						WHERE order_lines.orderid = @order_id",
				(oh, ol, cp, mp, co) =>
				{
					ol.Header = oh;
					ol.Cust_Product = cp;
					if(cp != null)
						cp.MastProduct = mp;
					if(mp != null)
						mp.Factory = co;
					return ol;
				}, new {order_id}, splitOn: "linenum, cprod_id, mast_id, user_id").ToList();
						
		}

		/***/
		public List<Order_lines> GetByOrder()
		{
			return conn.Query<Order_lines>(
				@"SELECT 
					Count(order_line_detail2_v6.orderid) AS orderid,
					order_line_detail2_v6.cprod_id
					FROM order_line_detail2_v6
				INNER JOIN cust_products ON cust_products.cprod_id = order_line_detail2_v6.cprod_id
				WHERE order_line_detail2_v6.orderqty > 0
				AND order_line_detail2_v6.req_eta >= DATE_SUB(NOW(),INTERVAL 12 MONTH)
				AND order_line_detail2_v6.stock_order IN (8)
				AND cust_products.cprod_user IN (45,55,149)
				GROUP BY order_line_detail2_v6.cprod_id").ToList();
			
		} 


		/***/
        public List<Order_line_detail2_v6> GetOrderingPatterns(string customer_code,DateTime ordersFrom,DateTime ordersTo)
        {
			return conn.Query<Order_line_detail2_v6>(
				$@"SELECT
                    order_line_detail2_v6.linenum,
                    order_line_detail2_v6.orderid,
                    COUNT(order_line_detail2_v6.orderid)as 'counted_orders',
                    order_line_detail2_v6.linedate,
                DAYNAME(order_line_detail2_v6.linedate) AS 'week_day',
                SUM(order_line_detail2_v6.orderqty),
                    order_line_detail2_v6.cprod_user,
                    order_line_detail2_v6.customer_code,
                    order_line_detail2_v6.container_type
                FROM
                    order_line_detail2_v6
                WHERE
                    order_line_detail2_v6.container_type IN (0, 1, 2, 6) AND
                    order_line_detail2_v6.customer_code IN ({customer_code}) AND
                    order_line_detail2_v6.linedate BETWEEN @ordersFrom and @ordersTo
                GROUP BY
                orderid", new {ordersFrom, ordersTo}).ToList();

            
        }

		public List<Order_lines> GetByCustPo(string custPo, int? company_id = null)
		{
			var lines = conn.Query<Order_header, Cust_products, Order_lines, Order_lines>(
				@"SELECT order_header.*,(CASE order_header.stock_order 
						WHEN 1 THEN 
							DATE_ADD(order_header.orderdate,INTERVAL (CASE WHEN mast_products.factory_id IN (16,18) THEN 4 ELSE 6 END) MONTH)
						ELSE COALESCE(porder_header.po_req_etd, DATE_ADD(order_header.req_eta, INTERVAL -35 DAY)) END) AS po_req_etd,
						cust_products.*,order_lines.*,
					FROM porder_lines INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid 
					RIGHT OUTER JOIN order_lines ON porder_lines.soline = order_lines.linenum INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id INNER JOIN order_header ON 
					order_lines.orderid = order_header.orderid INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
					WHERE order_header.custpo = @custpo AND (order_header.userid1 = @company_id OR @company_id IS NULL)",
				(oh, cp, ol) => {
					ol.Header = oh;
					ol.Cust_Product = cp;
					return ol;
				}, 
			new {custPo, company_id}, splitOn: "cprod_id, linenum").ToList();
			foreach(var l in lines)
			{
				if(l?.Cust_Product?.cprod_mast != null)
					l.Cust_Product.MastProduct = mastProductsDal.GetById(l.Cust_Product.cprod_mast.Value);
				if (l.Cust_Product?.MastProduct?.factory_id != null)
					l.Cust_Product.MastProduct.Factory = companyDAL.GetById(l.Cust_Product.MastProduct.factory_id.Value);
			}

			return lines;
		}

		public List<Order_lines> GetByCustPos(string[] custPos)
		{
			return conn.Query<Porder_header, Order_header, Cust_products, Order_lines, Mast_products, Order_lines>(
				@"SELECT porder_header.special_comments, order_header.*,
					(SELECT MAX(po_req_etd) FROM porder_header WHERE soorderid = order_header.orderid) AS po_req_etd,
					cust_products.*, order_lines.*,mast_products.mast_id, mast_products.factory_ref,mast_products.special_comments
					FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
					INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id INNER JOIN order_header ON 
					order_lines.orderid = order_header.orderid INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum INNER JOIN 
					porder_header ON porder_lines.porderid = porder_header.porderid
					WHERE order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) 
					AND order_header.custpo IN @custPos",
				(ph, oh, cp, ol, mp) =>
				{
					ol.Header = oh;
					ol.POHeader = ph;
					ol.Cust_Product = cp;
					cp.MastProduct = mp;
					return ol;
				}, new {custPos}, splitOn: "orderid, cprod_id, linenum, mast_id").ToList();

		}

		public List<Order_lines> GetByCriteria(int? month, int? year, string criteria, int company_id)
		{
			DateTime? date = null;
			if(month != null)
				date = new DateTime(year.Value, month.Value,1);

			return conn.Query<Order_lines, Cust_products, Order_lines>(
				@"SELECT order_lines.cprod_id, 
			(SELECT line.unitprice FROM order_lines line INNER JOIN order_header oh ON line.orderid = oh.orderid 
			WHERE cprod_id = order_lines.cprod_id AND (oh.req_eta < @date OR @date IS NULL) ORDER BY oh.req_eta DESC LIMIT 1)  AS unitprice,
			cust_products.cprod_name, cust_products.cprod_code1,users.consolidated_port
			FROM
				order_lines INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
			INNER JOIN order_header ON order_lines.orderid = order_header.orderid
			INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
			INNER JOIN users ON mast_products.factory_id = users.user_id
			WHERE (order_header.req_eta < @date OR @date IS NULL) AND (cprod_name LIKE @criteria OR cprod_code1 LIKE @criteria) AND order_header.userid1 = @company_id
			AND order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) AND (cust_products.cprod_returnable = 0) 
			AND order_header.req_eta < now() AND order_header.req_eta > (now() - interval 24 month)
			GROUP BY order_lines.cprod_id,cust_products.cprod_name,cust_products.cprod_code1,users.consolidated_port",
				(ol, cp) =>
				{
					ol.Cust_Product = cp;
					return ol;
				}, new {date, company_id, criteria = "%" + criteria + "%"}, splitOn: "cprod_name").ToList();

		}

		public List<Order_lines> GetForProductAndCriteria(int? month, int? year, int cprod_id, int? company_id = null, bool limitETA = false)
		{
			DateTime? date = null;
			if (month != null)
				date = new DateTime(year.Value, month.Value, 1);
			return conn.Query<Order_header, Cust_products, Order_lines, Order_lines>(
				$@"SELECT order_header.custpo,order_header.orderid, cust_products.cprod_name, cust_products.cprod_code1,
					order_lines.linenum, order_lines.cprod_id, order_lines.unitprice, order_lines.orderqty
					FROM
						order_lines INNER JOIN cust_products ON cust_products.cprod_id = order_lines.cprod_id
					INNER JOIN order_header ON order_lines.orderid = order_header.orderid
					WHERE (order_header.req_eta < @date OR @date IS NULL) AND cust_products.cprod_id =@cprod_id AND (order_header.userid1 = @company_id OR @company_id IS NULL)
					AND order_header.status NOT IN ('X','Y') AND (order_lines.orderqty > 0) AND (cust_products.cprod_returnable = 0) 
					{ (limitETA ? " AND order_header.req_eta < now()" : "") } AND order_header.req_eta > (now() - interval 24 month)
					",
				(oh, cp, ol) =>
				{
					ol.Header = oh;
					ol.Cust_Product = cp;
					return ol;
				}, new {date, company_id, cprod_id}, splitOn: "cprod_name, linenum").ToList();

		}

		public int GetNumOfPreviousShipments(DateTime po_req_etd, int cprod_id)
		{
			return conn.ExecuteScalar<int>(
				@"SELECT COUNT(*) AS numOfLines FROM order_lines INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum 
				INNER JOIN porder_header ON porder_lines.porderid = porder_header.porderid WHERE order_lines.cprod_id = @cprod_id 
				AND porder_header.po_req_etd < @po_req_etd",
				new {cprod_id, po_req_etd});			
		}

		
        public List<Order_lines> GetByProduct(int id)
        {
			return conn.Query<Order_lines>("SELECT * FROM order_lines WHERE cprod_id = @id", new {id}).ToList();            
        }
	
				
		public List<Order_lines> GetByOrderIds(IList<int> order_ids)
		{
			return conn.Query<Order_header, Cust_products, Mast_products, Order_lines, Order_lines>(
				@"SELECT order_header.*, cust_products.*,mast_products.*, order_lines.*
				FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
				INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
				INNER JOIN order_header ON order_lines.orderid = order_header.orderid
				WHERE order_lines.orderid IN @order_ids",
				(oh, cp, mp, ol) =>
				{
					ol.Header = oh;
					ol.Cust_Product = cp;
					cp.MastProduct = mp;
					return ol;
				}, new {order_ids}, splitOn: "cprod_id, mast_id, linenum").ToList();

		}

		public List<Order_lines> GetByExportCriteria(IList<int> cprod_ids=null, List<int> mast_ids=null, int? client_id=null, DateTime? etd_from = null,DateTime? 
            etd_to=null, int? factory_id=null, List<string> custpoList = null, IList<int> factory_ids = null, IList<int> excludedClients = null,
			IList<int> orderids = null, bool useLikeForCustpos = true, int? location_id = null)
		{
			var result = new List<Order_lines>();
			
			var cmd = new MySqlCommand("", conn);
			cmd.CommandTimeout = Properties.Settings.Default.DefaultTimeout;
			if(conn.State != ConnectionState.Open)
				conn.Open();
			var factoryCriteria = factory_ids != null ?  string.Format("AND factory_id IN ({0})",Utils.CreateParametersFromIdList(cmd,factory_ids,"factId")) 
                                    : " AND (factory_id=@factory_id OR @factory_id IS NULL)";
			string sql = string.Format(@"SELECT v_lines.*, po_orderqty*po_unitprice as PORowPrice FROM v_lines", factoryCriteria);

			var criteria = new List<string>();
			criteria.Add(" orderqty > 0");

			if (cprod_ids != null)
					criteria.Add(string.Format(" cprod_id IN ({0})",
										 Utils.CreateParametersFromIdList(cmd, cprod_ids)));
				else if(mast_ids != null)
				{
					criteria.Add(string.Format(" cprod_mast IN ({0})",
										 Utils.CreateParametersFromIdList(cmd, mast_ids)));
				}
			if (client_id != null)
			{
				if(client_id > 0)
					criteria.Add(" (userid1 = @client_id) ");
				else
				{
					if (client_id == -1)
						criteria.Add(" (userid1 IN (SELECT user_id FROM users WHERE distributor > 0))");
					else
						criteria.Add($" (userid1 IN (SELECT user_id FROM users WHERE combined_factory = {-1 * client_id} ))");
				}
			}

			if (excludedClients != null)
				criteria.Add($" userid1 NOT IN ({string.Join(",", excludedClients)}) ");

			if(orderids != null && orderids.Count > 0)
				criteria.Add($" orderid IN ({string.Join(",", orderids)}) ");

			if (custpoList != null && custpoList.Count > 0)
			{
				if (useLikeForCustpos)
				{
					var parts = new List<string>();
					for (int i = 0; i < custpoList.Count; i++)
					{
						cmd.Parameters.AddWithValue(string.Format("@custpo{0}", i + 1),
																"%" + custpoList[i] + "%");
						parts.Add(string.Format(" custpo LIKE @custpo{0} ", i + 1));
					}
					criteria.Add(" (" + string.Join(" OR ", parts) + ")");
				}
				else
					criteria.Add($" custpo IN @custpos");
			}
			criteria.Add("  status NOT IN ('X','Y') ");
			if (etd_from != null)
				criteria.Add(string.Format(" po_req_etd IS NOT NULL AND po_req_etd >= @etd_from",factoryCriteria));
			if (etd_to != null)
				criteria.Add(string.Format(" po_req_etd IS NOT NULL AND po_req_etd <= @etd_to", factoryCriteria));
			if (location_id != null)
				criteria.Add($@"(@location IS NULL OR (location_override IS NOT NULL AND location_override = @location)
								OR (location_override IS NULL AND consolidated_port = @location))");

			sql += " WHERE " + string.Join(" AND ", criteria);
			//sql += " GROUP BY linenum";

			cmd.CommandText = sql;
			cmd.Parameters.AddWithValue("@etd_from", etd_from != null ? (object) etd_from : DBNull.Value );
			cmd.Parameters.AddWithValue("@etd_to", etd_to != null ? (object) etd_to : DBNull.Value);
			cmd.Parameters.AddWithValue("@factory_id", factory_id);
			cmd.Parameters.AddWithValue("@client_id", client_id);
			cmd.Parameters.AddWithValue("@location", location_id);
			var dr = cmd.ExecuteReader();
			result = GetLinesFromReader(dr);
			dr.Close();
			return result;
			
		}

		public List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids, int? client_id,
														   DateTime? eta_from, DateTime? eta_to, int? factory_id)
		{
			return GetByProdIdsAndETA(cprod_ids, mast_ids,client_id != null ? new[] {client_id.Value}.ToList() : null, eta_from, eta_to, factory_id);
		}

		public List<Order_lines> GetByProdIdsAndETA(List<int> cprod_ids, List<int> mast_ids,
			List<int> client_ids = null, DateTime? eta_from = null, DateTime? eta_to = null, int? factory_id = null)
		{
			string sql = @"SELECT order_header.*, porder_header.instructions as po_instructions, cust_products.*, mast_products.*, order_lines.*
							FROM order_lines INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
							INNER JOIN order_header ON order_lines.orderid = order_header.orderid
							INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                            INNER JOIN porder_lines ON order_lines.linenum = porder_lines.soline 
                            INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid";
			if (cprod_ids != null)
				sql += " WHERE order_lines.cprod_id IN @cprod_ids";
			else
			{
				sql += " WHERE cust_products.cprod_mast IN @mast_ids";
			}
			sql += " AND order_header.stock_order <> 1 ";
			if (client_ids != null && client_ids.Count > 0)
			{

                if (client_ids[0] > 0) {
                    sql += " AND order_header.userid1 IN @client_ids ";
                        
				}
                else {
                    if (client_ids[0] == -1) {
                        sql += " AND (order_header.userid1 IN (SELECT user_id FROM users WHERE distributor > 0))";                        
                    }
                    else
                        sql += $" AND (order_header.userid1 IN (SELECT user_id FROM users WHERE combined_factory = {-1*client_ids[0]} ))";
                }
            }
			sql += " AND order_header.status NOT IN ('X','Y') ";
			if (eta_from != null)
				sql += " AND order_header.req_eta >= @eta_from";
			if (eta_to != null)
				sql += " AND order_header.req_eta <= @eta_to";

			return conn.Query<Order_header, Cust_products, Mast_products, Order_lines, Order_lines>(
				sql, 
				(oh, cp, mp, ol) =>
				{
					ol.Header = oh;
					ol.Cust_Product = cp;
					cp.MastProduct = mp;
					return ol;
				}, new {eta_from, eta_to, factory_id, cprod_ids, mast_ids, client_ids}, splitOn: "cprod_id, mast_id, linenum",
				commandTimeout: Properties.Settings.Default.DefaultTimeout).ToList();
				
		}

		public List<Order_lines> GetStockOrderLinesInFactory(IList<int> cprod_ids, DateTime? from = null)
		{
			var date = from ?? DateTime.Today;
			var lines = conn.Query<Order_header, Cust_products, Mast_products, Order_lines, Order_lines>(
				@"SELECT porder_header.po_req_etd, order_header.req_eta,order_header.orderdate,order_header.custpo,
					cust_products.cprod_id, mast_products.mast_id, mast_products.factory_id,
					order_lines.linenum, order_lines.cprod_id, order_lines.orderqty
					FROM order_lines
					INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
					INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
					INNER JOIN order_header ON order_lines.orderid = order_header.orderid
					INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
					INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
					WHERE order_header.stock_order = 1 AND order_lines.cprod_id IN @cprod_ids
					AND porder_header.po_req_etd > NOW() 
					OR (order_lines.orderqty - (SELECT SUM(alloc_qty) FROM stock_order_allocation so WHERE so.st_line = order_lines.linenum) > 0)
					OR EXISTS (SELECT co_lines.linenum FROM order_lines co_lines INNER JOIN stock_order_allocation so ON co_lines.linenum = so.so_line INNER JOIN porder_lines co_pline
						ON co_lines.linenum = co_pline.soline INNER JOIN porder_header co_pheader ON co_pline.porderid = co_pheader.porderid WHERE so.st_line = order_lines.linenum AND so.alloc_qty > 0
							AND co_pheader.po_req_etd > @date)",
				(oh, cp, mp, ol) =>
				{
					ol.Header = oh;
					ol.Cust_Product = cp;
					cp.MastProduct = mp;
					return ol;
				}, new {date, cprod_ids}, splitOn: "cprod_id, mast_id, linenum").ToList();

			
			foreach (var line in lines)
			{
				line.AllocatedLines = stockOorderAllocationDal.GetAllocationCalloffLines(line.linenum);
			}

			return lines;
		}

        public List<Order_line_detail2_v6> GetOrderLinesExport(int? clientId = null, int? factoryId = null, int? categoryId = null, bool? etaetd = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            
            var etaetd_part = etaetd.Value ? " req_eta >= @dateFromQuery AND req_eta <= @dateToQuery" : "po_req_etd >= @dateFromQuery AND po_req_etd <= @dateToQuery";

            //var etaetd_part = $@"po_req_etd >= '2018-01-01' AND po_req_etd <= '2018-12-31'";

            var dateToQuery = dateTo.ToString("yyyy-MM-dd");
            var dateFromQuery = dateFrom.ToString("yyyy-MM-dd");

            var lines = conn.Query<Order_line_detail2_v6, Mast_products, Category1, Company, Company, Order_line_detail2_v6>(
                //$@"SELECT od.*,mp.*,cat1.*,clients.user_id,clients.customer_code,factories.user_id,factories.factory_code
                $@"SELECT /*od.cprod_id,*/ od.cprod_code1,od.cprod_name,od.factory_ref,SUM(od.orderqty) AS orderqty,od.units_per_carton,
	                    od.units_per_pallet_single, 
                    mp.mast_id,mp.category1,
                        cat1.category1_id,cat1.cat1_name,
                            clients.user_id,clients.customer_code,
                                factories.user_id,factories.factory_code
            FROM order_line_detail2_v6 od
	            inner join mast_products mp on od.cprod_mast = mp.mast_id
	            inner join category1 cat1 on mp.category1 = cat1.category1_id
	            inner join users clients on od.userid1 = clients.user_id
	            inner join users factories on od.factory_id = factories.user_id
            WHERE
            { etaetd_part }
            AND (od.factory_id = @factoryId OR @factoryId = -1)
            AND (od.userid1 = @clientId OR @clientID = -1)
            AND (cat1.category1_id = @categoryId OR @categoryId = -1)
            GROUP BY /*od.cprod_id, */ od.cprod_code1,od.cprod_name,od.factory_ref,
                        mp.mast_id,mp.category1,
                            cat1.category1_id,cat1.cat1_name,
                                clients.user_id,clients.customer_code,
                                    factories.user_id,factories.factory_code
            order by cat1_name,od.cprod_code1
            ",
                (ol, mp, cat, cli, fac) =>
                {
                    ol.MastProduct = mp;
                    ol.MastProduct.Category = cat;
                    ol.Client = cli;
                    ol.Factory = fac;
                    return ol;
                },
                new { clientId, factoryId, categoryId, dateFromQuery, dateToQuery }, splitOn: "mast_id,category1_id,user_id,user_id"
                ).ToList();

            var mastidList = lines.Select(l => l.MastProduct.mast_id).ToList();

            var packageMaterialsInfo = Get_Packaging_Materials(mastidList);

            foreach (var l in lines)
            {
                l.MastProduct.PackagingMaterials = packageMaterialsInfo.Where(p => p.mast_id == l.MastProduct?.mast_id).ToList();
            }

            return lines;

        }

        public List<Mast_products_packaging_material> Get_Packaging_Materials(IList<int> mast_ids)
        {
            List<Mast_products_packaging_material> packaging_informations = new List<Mast_products_packaging_material>();

            if (mast_ids != null && mast_ids.Count > 0)
            {
                packaging_informations = conn.Query<Mast_products_packaging_material>(
                    $@"SELECT id,mast_id,factory_ref,packaging_id,material_id,amount FROM 
                    mast_products_packaging_material
                   WHERE mast_id IN @mast_ids
                ", new { mast_ids }).ToList();
            }

            return packaging_informations;
        }

        public List<Order_line_detail2_v6> GetProductsOrder(List<string> cprodcodes, DateTime? from, int? userid)
        {
            var data = conn.Query<Order_line_detail2_v6>(
                $@"SELECT * FROM order_line_detail2_v6
                    WHERE 
                        orderdate >= @from 
                            AND userid1 = @userid 
                                AND cprod_code1 IN @cprodcodes
                    ", new { cprodcodes, from, userid }).ToList();
                
            return data;
        }

        public List<Order_lines> GetNonShippedRegularOrderLines(IList<int> cprod_ids, DateTime? from = null)
		{
			var date = from ?? DateTime.Today;
			return conn.Query<Order_header, Order_lines, Order_lines>(
				@"SELECT porder_header.po_req_etd, order_header.req_eta,order_header.orderdate,order_header.custpo,
				order_lines.linenum,order_lines.cprod_id, order_lines.orderqty
				FROM order_lines
				INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
				INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
				INNER JOIN order_header ON order_lines.orderid = order_header.orderid
				WHERE order_header.stock_order IN (0,2) AND order_lines.cprod_id IN @cprod_ids
				AND order_header.req_eta > @date",
				(oh, ol) =>
				{
					ol.Header = oh;
					return ol;
				}, new {date, cprod_ids}, splitOn: "linenum").ToList();
		}

        public List<Order_lines> GetCalloffOrdersInShipment(IList<int> cprod_ids, DateTime? from = null)
		{
			var date = from ?? DateTime.Today;
			var lines = conn.Query<Order_header, Order_lines, Order_lines>(
				@"SELECT order_header.req_eta,order_header.custpo,porder_header.po_req_etd, order_lines.linenum,order_lines.cprod_id, order_lines.orderqty
				FROM order_lines
				INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
				INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
				INNER JOIN order_header ON order_lines.orderid = order_header.orderid
				WHERE order_header.stock_order <> 1 AND order_lines.cprod_id IN @cprod_ids
				AND porder_header.po_req_etd < @date AND order_header.req_eta > @date ",
				(oh, ol) =>
				{
					ol.Header = oh;
					return ol;
				}, new {date, cprod_ids}, splitOn: "linenum").ToList();
			foreach (var line in lines)
			{
				line.AllocatedLines = GetAllocationCalloffLines(line.linenum);
			}

			return lines;
		}
		
		private List<Order_lines> GetAllocationCalloffLines(int linenum)
		{
			return conn.Query<Order_header, Order_lines, Order_lines>(
				@"SELECT order_header.req_eta, porder_header.po_req_etd, order_lines.linenum,order_lines.orderqty,stock_order_allocation.alloc_qty,order_lines.cprod_id
					FROM order_lines INNER JOIN stock_order_allocation ON order_lines.linenum = stock_order_allocation.so_line 
					INNER JOIN order_header ON order_lines.orderid = order_header.orderid
					INNER JOIN porder_lines ON porder_lines.soline = order_lines.linenum
					INNER JOIN porder_header ON porder_header.porderid = porder_lines.porderid
					WHERE st_line = @linenum",
					(oh, ol) =>
					{
						ol.Header = oh;
						return ol;
					}, new {linenum}, splitOn: "linenum").ToList();
			
		}

		
        public List<Order_lines> GetInvoicedOrderLines2(string custpo, DateTime? ciDateFrom = null,
                DateTime? ciDateTo = null, DateTime? etFrom = null,
                DateTime? etTo = null, IList<int> clientIds = null, OrderDateMode orderDateMode = OrderDateMode.EtaPlusWeek,
                IList<int> excludedClientIds = null, bool loadPorderLines = false, IList<int> factoryIds = null,
                bool usePriceOverride = false, int? invoice_sequence = null, bool? UK = null, bool? checkCompanyEtdFrom = false)
        {
            var result = new List<Order_lines>();
            var searchByInvoiceDate = ciDateFrom != null || ciDateTo != null;
            var ignoreDates = !string.IsNullOrEmpty(custpo);
			DateTime? etFrom2 = null, etTo2 = null;
			var dateCriteria = string.Empty;
			
            /*
			if (orderDateMode == OrderDateMode.EtaPlusWeek )
			{
				if (etFrom != null)
					etFrom = etFrom.AddDays(-7);
				if (etTo != null)
					etTo = etTo.AddDays(-7);
			}
            */

			if(usePriceOverride)
			{
				if (etFrom != null)
					etFrom2 = etFrom.AddDays(-7);
				if (etTo != null)
					etTo2 = etTo.AddDays(-7);
			}
			//var etaField = orderDateMode == OrderDateMode.Eta ? "req_eta" : "req_eta1week";
			var cmd = new MySqlCommand("", conn);
			if(conn.State != ConnectionState.Open)
				conn.Open();

			var sqlBase = @"SELECT v_invoicedlines.* FROM v_invoicedlines
					WHERE stock_order <> 1 {0} {1}";

			var otherCriteria = $@"{Utils.CreateWhereClauseFromIdList(cmd, "userid1", clientIds)}
                        {Utils.CreateWhereClauseFromIdList(cmd, "userid1", excludedClientIds, "exid", negate: true)}
                        {Utils.CreateWhereClauseFromIdList(cmd, "factory_id ", factoryIds, "factId")}
                        AND (sequence_type = @invoice_sequence OR  @invoice_sequence IS NULL {(checkCompanyEtdFrom == true ? " OR po_req_etd >= bbs_etd_from " : "")})
						AND (invdate IS NULL OR invoice_status = 'C')";
			if (!string.IsNullOrEmpty(custpo))
				otherCriteria += " AND (custpo LIKE @custpo OR eb_invoice LIKE @custpo OR @custpo IS NULL)";
			if (!ignoreDates)
			{
				if (searchByInvoiceDate)
					dateCriteria = " AND(cidate >= @cidateFrom OR @cidateFrom IS NULL) AND(cidate <= @cidateTo OR @cidateTo IS NULL)";
				else if(!usePriceOverride)
				{
					var field = orderDateMode == OrderDateMode.Etd ? "po_req_etd" : orderDateMode == OrderDateMode.EtaPlusWeek ? "COALESCE(delivery_date,COALESCE(actual_eta,req_eta))" : "req_eta";
					dateCriteria = $" AND({field} >= @etdFrom OR @etdFrom IS NULL) AND({field} <= @etdTo OR @etdTo IS NULL)";
				}
				if (string.IsNullOrEmpty(dateCriteria))
				{
					dateCriteria = " AND (po_req_etd >= @etdFrom OR @etdFrom IS NULL) AND (po_req_etd <= @etdTo OR @etdTo IS NULL)";
					var dateCriteria2 = orderDateMode == OrderDateMode.EtaPlusWeek ? "AND (COALESCE(delivery_date,COALESCE(actual_eta,req_eta)) >= @etdFrom2 OR @etdFrom2 IS NULL) AND (COALESCE(delivery_date,COALESCE(actual_eta,req_eta)) <= @etdTo2 OR @etdTo2 IS NULL)"
                                                                                            : "AND (req_eta >= @etdFrom2 OR @etdFrom2 IS NULL) AND (req_eta <= @etdTo2 OR @etdTo2 IS NULL)";

                    cmd.CommandText = string.Format(sqlBase, dateCriteria, otherCriteria) + " UNION " +
								string.Format(sqlBase, dateCriteria2, otherCriteria);
				}

			}

			if(string.IsNullOrEmpty(cmd.CommandText))
				cmd.CommandText = string.Format(sqlBase, dateCriteria, otherCriteria);
			                
            cmd.Parameters.AddWithValue("@cidateFrom", Utilities.ToDBNull(ciDateFrom));
            cmd.Parameters.AddWithValue("@cidateTo", Utilities.ToDBNull(ciDateTo));
            cmd.Parameters.AddWithValue("@etdFrom", Utilities.ToDBNull(etFrom));
            cmd.Parameters.AddWithValue("@etdTo", Utilities.ToDBNull(etTo));
            cmd.Parameters.AddWithValue("@custpo", !string.IsNullOrEmpty(custpo) ? (object)("%" + custpo + "%") : DBNull.Value);
            cmd.Parameters.AddWithValue("@invoice_sequence", Utilities.ToDBNull(invoice_sequence));
            

			if(usePriceOverride)
			{
				cmd.Parameters.AddWithValue("@etdFrom2", Utilities.ToDBNull(etFrom2));
				cmd.Parameters.AddWithValue("@etdTo2", Utilities.ToDBNull(etTo2));
			}

            var dr = cmd.ExecuteReader();
            result = GetLinesFromReader(dr, true);

            dr.Close();
            conn.Close();

			if (usePriceOverride)
			{
				var price_Types = new[] { "CIF", "FOB" };
				result = result.Where(l => (l.Header.price_type_override == "FOB" || price_Types.Contains(l.Header.Client.user_price_type) ? l.Header.po_req_etd >= etFrom : l.Header.req_eta >= etFrom2) &&
										   (l.Header.price_type_override == "FOB" || price_Types.Contains(l.Header.Client.user_price_type) ? l.Header.po_req_etd <= etTo : l.Header.req_eta <= etTo2)).ToList();
			}

	        var orderids = result.Select(r => r.orderid).Distinct().ToList();

            

	        if (orderids.Count > 0)
	        {
		        var manualLines = orderLinesManualDal.GetForOrders(orderids);
		        foreach(var g in manualLines.GroupBy(ml=>ml.orderid)) {
			        foreach (var l in result.Where(r => r.orderid == g.Key))
				        l.Header.ManualLines = g.ToList();
		        }
	        }
            
            if (loadPorderLines) {

                foreach (var l in result)
                    l.PorderLines = new List<Porder_lines>() { new Porder_lines { unitcurrency = l.po_unitcurrency, unitprice = l.po_unitprice, orderqty = l.po_orderqty } };
                
            }
			
            //TODO: names of countries shold be in settings not hard coded
            if (UK.HasValue) {
                if (UK ?? false) {
                    result = result.Where(m => new[] { "GB", "IE" }.Contains(m.Header.Client.user_country)).ToList();
                }
                else {
                    result = result.Where(m => m.Header.Client.user_country != "GB" && m.Header.Client.user_country != "IE").ToList();
                }
            }

            return result;
        }

        private List<Order_lines> GetLinesFromReader(IDataReader dr, bool includeInvoice = false)
        {
            var result = new List<Order_lines>();
            var columnNames = Utilities.GetColumnNames(dr);
            while(dr.Read()) {
                result.Add(new Order_lines
                {
                    linenum = (int)dr["linenum"],
                    orderid = Utilities.FromDbValue<int>(dr["orderid"]),
                    linedate = Utilities.FromDbValue<DateTime>(dr["linedate"]),
                    cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]),
                    description = string.Empty + dr["description"],
                    orderqty = Utilities.FromDbValue<double>(dr["orderqty"]),
                    unitprice = Utilities.FromDbValue<double>(dr["unitprice"]),
                    unitcurrency = Utilities.FromDbValue<int>(dr["unitcurrency"]),
                    mc_qty = Utilities.FromDbValue<int>(dr["mc_qty"]),
                    pallet_qty = Utilities.FromDbValue<int>(dr["pallet_qty"]),
                    unit_qty = Utilities.FromDbValue<int>(dr["unit_qty"]),
                    original_orderid = Utilities.FromDbValue<int>(dr["original_orderid"]),
                    po_unitprice = Utilities.FromDbValue<double>(dr["po_unitprice"]),
                    po_unitcurrency = Utilities.FromDbValue<int>(dr["po_unitcurrency"]),
                    po_orderqty = Utilities.FromDbValue<double>(dr["po_orderqty"]),
                    orig_orderqty = Utilities.FromDbValue<double>(dr["orig_orderqty"]),
                    PO_USD = columnNames.Contains("PO_USD") ? Utilities.FromDbValue<double>(dr["PO_USD"]) : null,
                    commission_rate = columnNames.Contains("commission_rate") ? Utilities.FromDbValue<double>(dr["commission_rate"]) : null,
                    PORowPrice = columnNames.Contains("PORowPrice") ? Utilities.FromDbValue<double>(dr["PORowPrice"]) : null,
                    Header = new Order_header
                    {
                        orderid = Utilities.FromDbValue<int>(dr["orderid"]) ?? 0,
                        status = string.Empty + dr["status"],
                        po_req_etd = Utilities.FromDbValue<DateTime>(dr["po_req_etd"]),
                        original_po_req_etd = Utilities.FromDbValue<DateTime>(dr["original_po_req_etd"]),
                        po_comments = string.Empty + dr["special_comments"],
                        po_instructions = string.Empty + dr["instructions"],
                        userid1 = Utilities.FromDbValue<int>(dr["userid1"]),
                        custpo = string.Empty + dr["custpo"],
                        combined_order = Utilities.FromDbValue<int>(dr["combined_order"]),
                        req_eta = Utilities.FromDbValue<DateTime>(dr["req_eta"]),
                        original_eta = Utilities.FromDbValue<DateTime>(dr["original_eta"]),
                        orderdate = Utilities.FromDbValue<DateTime>(dr["orderdate"]),
                        stock_order = Utilities.FromDbValue<int>(dr["stock_order"]),
                        container_type = Utilities.FromDbValue<int>(dr["container_type"]),
                        booked_in_date = Utilities.FromDbValue<DateTime>(dr["booked_in_date"]),
                        price_type_override = string.Empty + dr["price_type_override"],
                        order_eb_invoice = Utilities.FromDbValue<int>(dr["order_eb_invoice"]),
                        process_id = Utilities.FromDbValue<int>(dr["process_id"]),
                        factory_id = Utilities.FromDbValue<int>(dr["po_factory_id"]),
                        po_process_id = Utilities.FromDbValue<int>(dr["po_process_id"]),
						location_override = Utilities.FromDbValue<int>(dr["location_override"]),
						system_eta = Utilities.ColumnExists(dr, "system_eta") ? Utilities.FromDbValue<DateTime>(dr["system_eta"]) : null,
                        delivery_date = Utilities.ColumnExists(dr, "delivery_date") ? Utilities.FromDbValue<DateTime>(dr["delivery_date"]) : null,
                        actual_eta = Utilities.ColumnExists(dr, "actual_eta") ? Utilities.FromDbValue<DateTime>(dr["actual_eta"]) : null,
						Client = new Company
                        {
                            distributor = Utilities.FromDbValue<int>(dr["distributor"]),
                            customer_code = string.Empty + dr["customer_code"],
                            user_country = string.Empty + dr["user_country"],
							user_price_type = string.Empty + dr["user_price_type"],
							vat_rate = Utilities.FromDbValue<double>(dr["vat_rate"])
                        },
                        Invoice = includeInvoice && dr["invdate"] != DBNull.Value ? new Invoices
                        {
                            invdate = Utilities.FromDbValue<DateTime>(dr["invdate"]),
                            eb_invoice = Utilities.FromDbValue<int>(dr["eb_invoice"]),
                            sequence = Utilities.FromDbValue<int>(dr["sequence"]),
                            invoice = (int)(dr["invoice"]),
                            invoice_sequence_type = Utilities.FromDbValue<int>(dr["sequence_type"]),
                            status = string.Empty + dr["invoice_status"],
                            cidate = Utilities.FromDbValue<DateTime>(dr["cidate"]),
                            invoice_from = Utilities.FromDbValue<int>(dr["invoice_from"]),
                            currency = Utilities.FromDbValue<int>(dr["invoice_currency"]),
                            inv_discount = Utilities.FromDbValue<double>(dr["inv_discount"]),
                            inv_amount2 = Utilities.FromDbValue<double>(dr["inv_amount2"]),
                            Client = new Company
                            {
                                distributor = Utilities.FromDbValue<int>(dr["distributor"]),
                                customer_code = string.Empty + dr["customer_code"],
                                user_country = string.Empty + dr["user_country"]
                            }
                        } : null,
                    },
                    Cust_Product = new Cust_products
                    {
                        cprod_id = Utilities.FromDbValue<int>(dr["cprod_id"]) ?? 0,
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]),
                        cprod_stock = Utilities.FromDbValue<int>(dr["cprod_stock"]),
                        cprod_stock_date = Utilities.FromDbValue<DateTime>(dr["cprod_stock_date"]),
                        cprod_user = Utilities.FromDbValue<int>(dr["cprod_user"]),
                        cprod_brand_cat = Utilities.FromDbValue<int>(dr["cprod_brand_cat"]),
                        cprod_status = string.Empty + dr["cprod_status"],
                        brand_userid = Utilities.FromDbValue<int>(dr["brand_user_id"]),
                        MastProduct = new Mast_products
                        {
                            factory_id = Utilities.FromDbValue<int>(dr["factory_id"]),
                            factory_ref = string.Empty + dr["factory_ref"],
                            factory_name = string.Empty + dr["factory_name"],
                            asaq_ref = string.Empty + dr["asaq_ref"],
                            asaq_name = string.Empty + dr["asaq_name"],
                            prod_length = Utilities.FromDbValue<double>(dr["prod_length"]),
                            prod_width = Utilities.FromDbValue<double>(dr["prod_width"]),
                            prod_height = Utilities.FromDbValue<double>(dr["prod_height"]),
                            om_seq_number = Utilities.FromDbValue<int>(dr["om_seq_number"]),
                            range1 = Utilities.FromDbValue<int>(dr["range1"]),
                            stock_qty = Utilities.FromDbValue<int>(dr["stock_qty"]),
                            threemonths = Utilities.FromDbValue<double>(dr["threemonths"]),
                            category1 = Utilities.FromDbValue<int>(dr["category1"]),
                            special_comments = string.Empty + dr["product_special_comments"],
                            lme = Utilities.FromDbValue<int>(dr["lme"]),
                            price_dollar = Utilities.FromDbValue<double>(dr["price_dollar"]),
                            price_pound = Utilities.FromDbValue<double>(dr["price_pound"]),
                            product_group = columnNames.Contains("product_group") ? string.Empty + dr["product_group"] : null,
                            Factory = new Company
                            {
                                user_id = Utilities.FromDbValue<int>(dr["factory_id"]) ?? 0,
                                user_name = string.Empty + dr["user_name"],
                                consolidated_port = Utilities.FromDbValue<int>(dr["consolidated_port"]),
                                combined_factory = Utilities.FromDbValue<int>(dr["combined_factory"]),
                                factory_code = string.Empty + dr["factory_code"]
                            }
                        }
                    }
                });
            }

            return result;

        }

        public List<Order_lines> GetLiveOrdersAnalysis(DateTime from, CountryFilter countryFilter = CountryFilter.UKOnly,int? client_id = null, 
			IList<int> includedNonDistributorsIds = null, bool brands = true)
	    {
	        var lines = new List<Order_lines>();
			if (conn.State != ConnectionState.Open)
				conn.Open();
	        var cmd = new MySqlCommand("", conn);
			string distributorCriteria = client_id == null ? $@" (distributor {(brands ? ">" : "<=")} 0 
                                {(includedNonDistributorsIds != null && includedNonDistributorsIds.Count > 0 ? $" OR userid1 IN ({Utils.CreateParametersFromIdList(cmd, includedNonDistributorsIds, "dist_")})" : "")})"
								: "";

            var testAccountIds = conn.Query<int>("SELECT user_id FROM users WHERE test_account = 1").ToList();

            var criteria = new List<string>()
                {
                    countriesDAL.GetCountryCondition(countryFilter, useAnd: false),                        
                    "po_req_etd > @from",
                    "(userid1 = @client_id OR @client_id IS NULL)",
                    "orderqty > 0 ",
                    $"userid1 NOT IN ({Utils.CreateParametersFromIdList(cmd,testAccountIds)})",
                    "category1 <> 13 ",
                    $"cprod_brand_cat NOT IN ({Utils.ExcludedCprodBrandCats})"
                };
			if (!string.IsNullOrEmpty(distributorCriteria))
				criteria.Add(distributorCriteria);

            var sql = $@"SELECT v_lines.* FROM v_lines 
                                WHERE {string.Join(" AND ", criteria)}  ";
	        cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@from", from);
	        cmd.Parameters.AddWithValue("@client_id", client_id);
	        var dr = cmd.ExecuteReader();
            lines = GetLinesFromReader(dr, false);
            dr.Close();                
	        
	        return lines;
	    }

        public List<int> GetByOriginalOrderids(string ids)
        {
			return conn.Query<int>("SELECT DISTINCT orderid FROM order_lines IN @ids", new {ids}).ToList();            
        }

		public List<Order_lines> GetLinesWithSentToFactoryDate(int? factory_id = null, DateTime? from = null, DateTime? to = null)
		{
			var sql = @"SELECT * FROM v_lines 
						WHERE (factory_id = @factory_id OR @factory_id IS NULL) AND  
						cprod_user IN (42,78,259,260,309,80) AND
						(orderdate >= @from OR @from IS NULL) AND
						(orderdate <= @to OR @to IS NULL)";
			var cmd = new MySqlCommand(sql, conn);
			if (conn.State != ConnectionState.Open)
				conn.Open();
			cmd.Parameters.AddWithValue("@factory_id", factory_id);
			cmd.Parameters.AddWithValue("@from", from);
			cmd.Parameters.AddWithValue("@to", to);
			var dr = cmd.ExecuteReader();
			var lines = GetLinesFromReader(dr, false);
			dr.Close();

			var ids = lines.Select(l => l.orderid).Distinct().ToList();
			var dictTickets = conn.Query<tickets>("SELECT * FROM 2011_tickets WHERE process=90 AND reference IN @ids", new { ids })
				.GroupBy(t => t.reference)
				.ToDictionary(g => g.Key, g=>g.Max(t=>t.process_date));
			foreach(var l in lines)
			{
				if(dictTickets.ContainsKey(l.orderid))
				{
					l.Header.SentToFactoryDate = dictTickets[l.orderid];
				}
			}

			return lines;
		}

		



		protected override string IdField => "linenum";

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM order_lines WHERE linenum = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO order_lines (orderid,original_orderid,linedate,cprod_id,description,spec_code,orderqty,orig_orderqty,
				unitprice,unitcurrency,linestatus,lineunit,factory_group,mc_qty,pallet_qty,unit_qty,lme,allow_change,allow_change_down,
				fi_line,pack_qty,li_line) VALUES(@orderid,@original_orderid,@linedate,@cprod_id,@description,@spec_code,@orderqty,@orig_orderqty,
				@unitprice,@unitcurrency,@linestatus,@lineunit,@factory_group,@mc_qty,@pallet_qty,@unit_qty,@lme,@allow_change,@allow_change_down,
				@fi_line,@pack_qty,@li_line)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE order_lines SET orderid = @orderid,original_orderid = @original_orderid,linedate = @linedate,cprod_id = @cprod_id,
				description = @description,spec_code = @spec_code,orderqty = @orderqty,orig_orderqty = @orig_orderqty,unitprice = @unitprice,
				unitcurrency = @unitcurrency,linestatus = @linestatus,lineunit = @lineunit,factory_group = @factory_group,
				mc_qty = @mc_qty,pallet_qty = @pallet_qty,unit_qty = @unit_qty,lme = @lme,allow_change = @allow_change,
				allow_change_down = @allow_change_down,fi_line = @fi_line,pack_qty = @pack_qty,li_line = @li_line WHERE linenum = @linenum";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM order_lines WHERE linenum = @linenum";
		}
	}
}
			
			