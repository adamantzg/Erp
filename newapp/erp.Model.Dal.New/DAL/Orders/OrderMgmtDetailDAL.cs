using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public enum SearchCategory
    {
        NotSet,
        New,
        Changed
    }

    public class OrderMgmtDetailDAL : GenericDal<OrderMgtmDetail>, IOrderMgmtDetailDAL
	{
		public OrderMgmtDetailDAL(IDbConnection conn) : base(conn)
		{
		}
		        
        public List<OrderMgtmDetail> GetByProducts(IList<int> cprod_ids, DateTime eta_from, DateTime eta_to)
        {
			return conn.Query<OrderMgtmDetail>(
				$@"SELECT * FROM om_detail2 WHERE cprod_id IN @cprod_ids AND stock_order <> 1 
                AND ( (booked_in_date IS NOT NULL AND (booked_in_date BETWEEN @from AND @to)) 
                OR (booked_in_date IS NULL AND (CASE WHEN req_eta < CURDATE() THEN CURDATE() ELSE req_eta END) BETWEEN @from AND @to) )",
				new {@from = eta_from, to = eta_to, cprod_ids}
				).ToList();            
        }

        public List<OrderMgtmDetail> GetByMastProduct(int mast_id, DateTime eta_from, DateTime eta_to)
        {
			return conn.Query<OrderMgtmDetail>(
				@"SELECT * FROM om_detail2 INNER JOIN cust_products ON om_detail2.cprod_id = cust_products.cprod_id 
				WHERE cprod_mast = @mast_id AND stock_order <> 1 AND (req_eta BETWEEN @eta_from AND @eta_to OR booked_in_date BETWEEN @eta_from AND @eta_to)",
				new {mast_id, eta_from, eta_to}
				).ToList();            
        }

        public List<ProductSaleSummary> GetSaleByProductIds(List<int> ids, DateTime? etd_from=null, DateTime? etd_to=null)
        {
			return conn.Query<ProductSaleSummary>(
				@"SELECT cprod_id, SUM(orderqty) AS QtySold, SUM(orderqty*(CASE unitcurrency WHEN 0 THEN unitprice/1.6 ELSE unitprice END)) AS amount 
					FROM om_detail1 
                    WHERE cprod_id IN @ids AND (po_req_etd >= @etd_from OR @etd_from IS NULL) AND (po_req_etd <= @etd_to OR @etd_to IS NULL)
                    GROUP BY cprod_id",
				new {ids, etd_from, etd_to}				
				).ToList();            
        }

        public List<ProductSaleMonthSummary> GetMonthSaleByFactory(int? factory_Id, int? month21_from = null, int? month21_to = null)
        {
			return conn.Query<ProductSaleMonthSummary>(
				@"SELECT cprod_id, cprod_name, cprod_code1,month21, SUM(orderqty) AS QtySold, 
						SUM(orderqty*(CASE unitcurrency WHEN 0 THEN unitprice/1.6 ELSE unitprice END)) AS amount 
						FROM om_detail1 
                        WHERE (factory_id = @factory_id OR @factory_id IS NULL) AND (month21 >= @month21_from OR @month21_from IS NULL) 
						AND (month21 <= @month21_to OR @month21_to IS NULL)
                        GROUP BY cprod_id,cprod_name, cprod_code1, month21 ORDER BY month21",
				new {factory_Id, month21_from, month21_to }
				).ToList();            
        }

        public List<ProductOrderSummary> GetOrdersTotal(DateTime? from, DateTime? to)
        {
			return conn.Query<ProductOrderSummary>(
				@"SELECT cprod_id, COALESCE(SUM(orderqty),0) AS TotalQty 
				FROM om_detail1 WHERE (po_req_etd >= @from OR @from IS NULL) AND (po_req_etd <= @to OR @to IS NULL) GROUP BY cprod_id",
				new {@from, to}
				).ToList();

        }

        public List<OrderMgtmDetail> SearchLines(string product, string po, int? factory_id, int? client_id,
                                                        DateTime? etd_from, DateTime? etd_to,SearchCategory category ,
                                                        int orderby)
        {
			var sql = @"SELECT om_detail1.* FROM om_detail1 WHERE category1 <> 13
					AND (factory_ref like @product or cprod_code1 like @product OR cprod_name LIKE @product OR @product IS NULL) 
					AND (po_req_etd >= @etd_from OR @etd_from IS NULL) AND (po_req_etd <= @etd_to OR @etd_to IS NULL)
					AND (factory_id = @factory_id OR @factory_id IS NULL) AND (userid1 = @client_id OR @client_id IS NULL) 
					AND (custpo LIKE @po OR @po IS NULL) ";
			if (category == SearchCategory.New)
            {
                sql +=
                    @" AND NOT EXISTS (SELECT om_detail1.linenum FROM om_detail1 oldlines WHERE oldlines.cprod_mast = om_detail1.cprod_mast AND category1 <> 13 
                                        AND orderqty > 0 AND po_req_etd < om_detail1.po_req_etd)";
            }
            else
            {
                sql +=
                    @" AND EXISTS (SELECT * FROM asaq.2011_change_notice_product_table WHERE mastid = om_detail1.cprod_mast AND product_po = om_detail1.custpo)";
            }
            if (orderby == 1)
                sql += " ORDER BY po_req_etd";
            else if (orderby == 2)
                sql += " ORDER BY custpo";
            else
            {
                sql += " ORDER BY factory_code";
            }
			return conn.Query<OrderMgtmDetail>(sql, new {product, etd_from, etd_to, client_id, factory_id, po}).ToList();
			           
        }
        

		protected override string GetAllSql()
		{
			return "SELECT * FROM om_detail2";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}
	}
}
			
			