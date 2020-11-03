
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class InspectionLinesTestedDal : GenericDal<Inspection_lines_tested>, IInspectionLinesTestedDal
    {
	    private IInspectionsLoadingDal inspectionsLoadingDal;

	    public InspectionLinesTestedDal(IDbConnection conn, IInspectionsLoadingDal inspectionsLoadingDal) : base(conn)
	    {
		    this.inspectionsLoadingDal = inspectionsLoadingDal;
	    }


	    protected override string GetAllSql()
		{
			return "SELECT * FROM inspection_lines_tested";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspection_lines_tested WHERE insp_line_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO inspection_lines_tested (insp_id,insp_factory_ref,insp_client_ref,insp_client_desc,insp_qty,insp_override_qty,
				insp_custpo,order_linenum,photo_confirm,photo_confirma,photo_confirmm,photo_confirmd,photo_confirmf,photo_confirmp,packaging_rej,
				label_rej,instructions_rej) VALUES(@insp_id,@insp_factory_ref,@insp_client_ref,@insp_client_desc,@insp_qty,@insp_override_qty,
				@insp_custpo,@order_linenum,@photo_confirm,@photo_confirma,@photo_confirmm,@photo_confirmd,@photo_confirmf,@photo_confirmp,
				@packaging_rej,@label_rej,@instructions_rej)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE inspection_lines_tested SET insp_id = @insp_id,insp_factory_ref = @insp_factory_ref,insp_client_ref = @insp_client_ref,
				insp_client_desc = @insp_client_desc,insp_qty = @insp_qty,insp_override_qty = @insp_override_qty,insp_custpo = @insp_custpo,
				order_linenum = @order_linenum,photo_confirm = @photo_confirm,photo_confirma = @photo_confirma,photo_confirmm = @photo_confirmm,
				photo_confirmd = @photo_confirmd,photo_confirmf = @photo_confirmf,photo_confirmp = @photo_confirmp,packaging_rej = @packaging_rej,
				label_rej = @label_rej,instructions_rej = @instructions_rej WHERE insp_line_unique = @insp_line_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspection_lines_tested WHERE insp_line_unique = @id";
		}

		public List<Inspection_lines_tested> GetByInspection(int insp_id, bool includeProducts = false, bool includeLoadings = false)
        {
            var sql = $@"SELECT {(includeProducts ? "cust_products.*," : "")}inspection_lines_tested.*,
							 order_lines.* {(includeProducts ? ", mast_products.*" : "")} 
                            FROM Inspection_lines_tested LEFT OUTER JOIN order_lines ON Inspection_lines_tested.order_linenum = order_lines.linenum 
                            {(includeProducts ? "LEFT OUTER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id LEFT OUTER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id" : "")}
                            WHERE insp_id = @insp_id";
	        var result = includeProducts
		        ? conn.Query<Cust_products, Inspection_lines_tested, Order_lines, Mast_products, Inspection_lines_tested>(
			        sql, (cp, il, ol, mp) =>
			        {
				        il.OrderLine = ol;
						if(ol != null)
							ol.Cust_Product = cp;
						if(cp != null)
							cp.MastProduct = mp;
				        return il;
			        }, new {insp_id}, splitOn: "insp_line_unique, linenum, mast_id").ToList() : 
		        
		        conn.Query<Inspection_lines_tested, Order_lines, Inspection_lines_tested>(
			        sql, (il, ol) =>
			        {
				        il.OrderLine = ol;
				        return il;
			        }, new {insp_id}, splitOn: "linenum").ToList()
		        
		        ;

                if (includeLoadings)
                {
                    FillLoadings(new int[] { insp_id }, result);
                }

	        return result;

        }

        public List<Inspection_lines_tested> GetLinesForOrders(IList<int> orderIds, int? excludedInspId = null, bool includeLoadings = false)
        {
            var sql = 
                    $@"SELECT cust_products.*, inspection_lines_tested.*,order_lines.*, mast_products.* ,inspections.*
                        FROM inspection_lines_tested INNER JOIN inspections ON inspection_lines_tested.insp_id = inspections.insp_unique 
                        LEFT OUTER JOIN order_lines ON inspection_lines_tested.order_linenum = order_lines.linenum 
                        LEFT OUTER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id 
                        LEFT OUTER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                        WHERE {(excludedInspId != null ? " inspection_lines_tested.insp_id <> @excludedInspId AND " : "")} order_lines.orderid IN @orderIds";

	        var result = conn
		        .Query<Cust_products, Inspection_lines_tested, Order_lines, Mast_products, Inspections,
			        Inspection_lines_tested>(
			        sql,
			        (cp, il, ol, mp, i) =>
			        {
				        il.OrderLine = ol;
				        il.Inspection = i;
				        if (ol != null)
					        ol.Cust_Product = cp;
				        if (cp != null)
					        cp.MastProduct = mp;
				        return il;
			        }, new {excludedInspId, orderIds}, splitOn: "insp_line_unique, linenum, mast_id, insp_unique").ToList();
                
                if (includeLoadings)
                {
                    var insp_ids = result.Select(i => i.insp_id ?? 0).Distinct().ToList();
                    FillLoadings(insp_ids, result);
                }
            

            return result;
        }

        private void FillLoadings(IList<int> insp_ids, List<Inspection_lines_tested> lines)
        {
            var loadings = inspectionsLoadingDal.GetForInspection(insp_ids);
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

		
        public List<Inspection_lines_tested> GetInspLines(int insp_id)
        {
	        return conn.Query<Inspection_lines_tested>("SELECT * FROM inspection_lines_tested WHERE insp_id = @insp_id",
		        new {insp_id}).ToList();
            
        }
        /// <summary>
        /// prvi parametar za filtriranje po inspection
        /// druga dva dohvati sve pa filtriraj po factories  po distributeru
        /// </summary>
        /// <param name="insp_id"></param>
        /// <param name="factory_id"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<Inspections_documents> GetByInspId(int insp_id=0, string factory_code="", string customer_code="")
        {
	        return conn.Query<Inspections_documents>($@"SELECT
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
                    WHERE 
					{
				        (insp_id > 0
					        ? "inspection_lines_tested.insp_id = @insp_id"
					        : "inspections.factory_code = @factory_code and inspections.customer_code = @customer_code")
			        }",
		        new {insp_id, factory_code, customer_code}).ToList();
                
            
        }
        public List<Inspections_documents> GetByFactoryRef(int insp_id)
        {
	        return conn.Query<Inspections_documents>($@"SELECT
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
                    inspection_lines_tested.insp_id = @insp_id AND inspection_lines_tested.order_linenum = 0 ",
		        new {insp_id}).ToList();
        }

        public List<Inspections_documents> GetByProducts(int factory_id, int customer_id)
        {
	        return conn.Query<Inspections_documents>(
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
                     where cust_product_details.factory_id = @factory_id 
                        and cust_product_details.cprod_user = @customer_id
                  ", new {factory_id, customer_id}).ToList();
        }

        
		
		
	    protected override string IdField => "insp_line_unique";
    }
}
			
			