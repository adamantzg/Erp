using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionsV2DAL : GenericDal<Inspection_v2>, IInspectionsV2DAL
    {
	    private ICustproductsDAL custproductsDal;

	    public InspectionsV2DAL(IDbConnection conn, ICustproductsDAL custproductsDal) : base(conn)
	    {
		    this.custproductsDal = custproductsDal;
	    }

        /// <summary>
        /// Optimized version. Fills only some lists and fields
        /// </summary>
        /// <param name="factory_ids"></param>
        /// <param name="client_ids"></param>
        /// <param name="custpo"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="statuses"></param>
        /// <param name="inspectorId"></param>
        /// <returns></returns>
        public List<Inspection_v2> GetByCriteria(IList<int> factory_ids, IList<int> client_ids, string custpo,
            DateTime? from,
            DateTime? to, IList<InspectionV2Status?> statuses = null, int? inspectorId = null)
        {

			var result = new List<Inspection_v2>();

	        conn.Query<Inspection_v2, Company, Company, Order_header, Inspection_v2>(
		        $@"SELECT  inspection_v2.id, inspection_v2.startdate, GROUP_CONCAT(DISTINCT order_header.custpo) AS custpos,
                inspection_v2.type, inspection_v2.custpo,inspection_v2.factory_id,inspection_v2.`code`,inspection_v2.client_id,
                inspection_v2.duration,inspection_v2.comments,inspection_v2.qc_required, inspection_v2.comments_admin,
                inspection_v2.insp_status,inspection_v2.acceptance_fc,inspection_v2.insp_batch_inspection, 
				factory.factory_code, client.customer_code, order_header.custpo
                FROM inspection_v2
                INNER JOIN inspection_v2_line ON inspection_v2_line.insp_id = inspection_v2.id
                INNER JOIN order_lines ON inspection_v2_line.orderlines_id = order_lines.linenum
                INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                INNER JOIN users factory ON inspection_v2.factory_id = factory.user_id
                INNER JOIN users client ON inspection_v2.client_id = client.user_id
                WHERE (startdate >= @from OR @from IS NULL) AND (startdate <= @to OR @to IS NULL) 
				{(factory_ids != null ? " AND inspection_v2.factory_id IN @factory_ids" : "")} 
				{(client_ids != null ? " AND inspection_v2.client_id IN @client_ids" : "")}
				{(!string.IsNullOrEmpty(custpo) ? " AND order_header.custpo LIKE @custpo" : "")}
				{(statuses != null ? " AND insp_status IN @statuses" : "")} 
				{(inspectorId != null
			        ? " AND EXISTS (SELECT * FROM inspection_v2_controller WHERE inspection_id = inspection_v2.id AND controller_id = @inspectorId)"
			        : "")}
                GROUP BY inspection_v2.id",
		        (i, f, c, o) =>
		        {
			        var insp = result.FirstOrDefault(x => x.id == i.id);
			        if (insp == null)
			        {
				        insp = i;
				        insp.Factory = f;
				        insp.Client = c;
				        result.Add(insp);
				        insp.Lines = new List<Inspection_v2_line>();
			        }

			        insp.Lines.Add(new Inspection_v2_line
			        {
				        OrderLine = new Order_lines {Header = new Order_header {custpo = o.custpo}}
			        });
			        return insp;
		        }, new {from, to, factory_ids, client_ids, statuses, inspectorId,custpo = "%" + custpo + "%"},
		        splitOn: "factory_code, customer_code, custpo");
            
                return result;

        }

        
        public Inspection_v2 GetById(int id, bool loadLoadings = false,bool loadImages = false)
        {
	        var result = conn.Query<Inspection_v2, Company, Company, Inspection_v2_type, Inspection_v2>(
		        @"SELECT inspection_v2.*,factory.factory_code, client.customer_code, inspection_v2_type.*
                 FROM inspection_v2 
                 INNER JOIN users factory ON inspection_v2.factory_id = factory.user_id
                 INNER JOIN users client ON inspection_v2.client_id = client.user_id 
                 INNER JOIN inspection_v2_type ON inspection_v2.type = inspection_v2_type.id
                 WHERE inspection_v2.id = @id",
		        (i, f, c, t) =>
		        {
			        i.Factory = f;
			        i.Client = c;
			        i.InspectionType = t;
			        return i;
		        }, new {id}, splitOn: "factory_code, customer_code, id").FirstOrDefault();
                
            if (result != null)
            {
                result.Lines = LoadLines(id, loadLoadings: loadLoadings, insp: result, loadImages: loadImages);

	            result.Containers = conn.Query<Inspection_v2_container>(
		            "SELECT * FROM Inspection_v2_container WHERE insp_id = @id", new {id}).ToList();

	            result.Controllers = conn.Query<Inspection_v2_controller, User, Inspection_v2_controller>(
		            @"SELECT inspection_v2_controller.*, userusers.* FROM inspection_v2_controller 
						INNER JOIN userusers ON inspection_v2_controller.controller_id = userusers.useruserid 
						WHERE inspection_id = @id",
		            (c, u) =>
		            {
			            c.Controller = u;
			            return c;
		            }, new {id}, splitOn: "useruserid").ToList();
				
                var sql = @"SELECT * FROM inspection_v2_mixedpallet WHERE insp_id = @id";
                result.MixedPallets = conn.Query<Inspection_v2_mixedpallet>(sql,new { id }).ToList();                    

            }

	        return result;

        }

        public List<Inspection_v2_line> LoadLines(int? id= null, IList<int> orderids = null, bool loadImages = false, bool loadLoadings = false, Inspection_v2 insp = null)
        {
            
            var sql = $@"SELECT order_header.custpo, order_header.orderdate, cust_products.cprod_id,cust_products.cprod_code1,cust_products.cprod_name, 
						cust_products.cprod_mast, inspection_v2_line.*,order_lines.*,
                       mast_products.mast_id, mast_products.factory_ref, mast_products.factory_id,mast_products.special_comments,
						mast_products.units_per_40nopallet_hc, mast_products.units_per_40pallet_hc,mast_products.category1,
                         factory.user_id, factory.factory_code
                         FROM  
                         order_lines 
                         INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                         INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                         INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                         INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                         RIGHT OUTER JOIN inspection_v2_line ON inspection_v2_line.orderlines_id = order_lines.linenum
                         WHERE {(orderids != null ? $"order_lines.orderid IN @orderids" : "inspection_v2_line.insp_id = @id" )}";
			if(insp == null)
				insp = base.GetById(id ?? 0);
	        var result = conn
		        .Query<Order_header, Cust_products, Inspection_v2_line, Order_lines, Mast_products, Company,
			        Inspection_v2_line>(
			        sql, (h, cp, il, ol, mp, f) =>
			        {
				        il.OrderLine = ol;
						il.Inspection = insp;
						if(ol != null)
						{
							ol.Header = h;
							ol.Cust_Product = cp;
							if(cp != null)
								cp.MastProduct = mp;
							if(mp != null)
								mp.Factory = f;
						}				        
				        return il;
			        }, new {id, orderids}, splitOn: "cprod_id, id, linenum, mast_id, user_id").ToList();
            
           
            foreach (var l in result.Where(l => l.OrderLine == null && l.cprod_id != null))
            {
                l.Product = custproductsDal.GetById(l.cprod_id.Value, true);
            }
            foreach (var l in result.Where(l => l.OrderLine != null && l.OrderLine.Cust_Product != null))
            {
                l.OrderLine.Cust_Product.Parents =
                    custproductsDal.GetSpareParents(l.OrderLine.Cust_Product.cprod_id);
            }

	        var lineIds = result.Select(l => l.id).ToList();
            if (loadImages && result.Count > 0)
            {
	            var images = conn.Query<Inspection_v2_image>(
		            "SELECT * FROM inspection_v2_image WHERE insp_line IN @lineIds", new {lineIds}).ToList();
                
                foreach(var img in images)
                {
                    var line = result.FirstOrDefault(l => l.id == img.insp_line);
                    if (line != null)
                    {
                        img.Line = line;
                        if (line.Images == null)
                            line.Images = new List<Inspection_v2_image>();
                        line.Images.Add(img);
                    }
                }
            }

            if (loadLoadings && result.Count > 0)
            {
                //var areas = conn.Query<Inspection_v2_area>("SELECT * FROM inspection_v2_area").ToList();
	            var loadings = new List<Inspection_v2_loading>();
	            conn.Query<Inspection_v2_loading, Inspection_v2_area, Inspection_v2_container, Inspection_v2_loading>(
		            $@"SELECT inspection_v2_loading.*, inspection_v2_area.*, 
						inspection_v2_container.* FROM inspection_v2_loading 
						LEFT OUTER JOIN inspection_v2_container ON inspection_v2_loading.container_id = 
						inspection_v2_container.id
						LEFT OUTER JOIN inspection_v2_loading_area
                        ON inspection_v2_loading_area.loading_id = inspection_v2_loading.id 						
						LEFT OUTER JOIN inspection_v2_area ON inspection_v2_loading_area.area_id = inspection_v2_area.id
						WHERE inspection_v2_loading.insp_line IN @lineIds",
		            (l,a,c) =>
		            {
			            var lo = loadings.FirstOrDefault(x => x.id == l.id);
			            if (lo == null)
			            {
				            lo = l;
				            lo.Container = c;
							lo.Areas = new List<Inspection_v2_area>();
							loadings.Add(lo);
			            }
						if(a != null)
							lo.Areas.Add(a);
			            return lo;
		            }, new {lineIds});
                
                foreach(var loading in loadings)
                {
                    var line = result.FirstOrDefault(l => l.id == loading.insp_line);
                    if (line != null)
                    {
                        loading.Line = line;
                        if (line.Loadings == null)
                            line.Loadings = new List<Inspection_v2_loading>();
                        line.Loadings.Add(loading);
                    }
                }
                
                foreach (var l in result.Where(r=>r.Loadings != null).SelectMany(r=>r.Loadings))
                {
	                l.QtyMixedPallets = conn.Query<Inspection_v2_loading_mixedpallet>(
		                @"SELECT * FROM inspection_v2_loading_mixedpallet WHERE loading_id = @id", new {l.id}).ToList();
                    
                }
            }
            
            return result;
        }

        
        public List<Inspection_v2> GetOrderInspections(int orderid, bool loadLoadings = false)
        {
            var result = new List<Inspection_v2>();
	        var ids = conn.Query<int>(
		        @"SELECT DISTINCT insp_id FROM inspection_v2_line INNER JOIN order_lines ON inspection_v2_line.orderlines_id = order_lines.linenum 
                   WHERE order_lines.orderid = @orderid", new {orderid}).ToList();
                
                var orderLineNumbers = conn.Query<int?>("SELECT linenum FROM order_lines WHERE orderid = @id", new { id = orderid }).ToList();
                foreach(var id in ids) { 
                    var insp = GetById(id,loadLoadings);
                    insp.Lines = insp.Lines.Where(l => orderLineNumbers.Contains(l.orderlines_id)).ToList();
                    result.Add(insp);
                }

	        return result;
        }

        
		
	    protected override string GetAllSql()
	    {
		    return "SELECT * FROM inspection_v2";
	    }

	    protected override string GetByIdSql()
	    {
		    return "SELECT * FROM inspection_v2 WHERE id = @id";
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
