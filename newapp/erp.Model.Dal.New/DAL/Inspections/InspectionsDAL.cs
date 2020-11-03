
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionsDAL : GenericDal<Inspections>, IInspectionsDAL
    {
		private IUserDAL userDal;
	    private ICompanyDAL companyDal;
	    private ICustproductsDAL custproductsDal;
	    private IInspectionControllerDal inspectionControllerDal;
		private readonly IInspectionImagesDAL inspectionImagesDAL;
		private readonly IInspectionLinesAcceptedDal inspectionLinesAcceptedDal;
		private readonly IInspectionLinesRejectedDal inspectionLinesRejectedDal;

		public InspectionsDAL(IDbConnection conn, IUserDAL userDal, ICompanyDAL companyDal, 
		    ICustproductsDAL custproductsDal, IInspectionControllerDal inspectionControllerDal,
			IInspectionImagesDAL inspectionImagesDAL, IInspectionLinesAcceptedDal inspectionLinesAcceptedDal,
			IInspectionLinesRejectedDal inspectionLinesRejectedDal) : base(conn)
	    {
		    this.inspectionControllerDal = inspectionControllerDal;
			this.inspectionImagesDAL = inspectionImagesDAL;
			this.inspectionLinesAcceptedDal = inspectionLinesAcceptedDal;
			this.inspectionLinesRejectedDal = inspectionLinesRejectedDal;
			this.custproductsDal = custproductsDal;
		    this.companyDal = companyDal;
		    this.userDal = userDal;
	    }

		protected override string GetAllSql()
		{
			return "SELECT * FROM inspections";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspections WHERE insp_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO inspections (insp_unique,insp_id,insp_type,insp_start,insp_end,insp_version,insp_days,insp_porderid,
					insp_qc1,insp_qc2,insp_qc3,insp_qc6,insp_fc,customer_code,custpo,batch_no,factory_code,insp_comments,insp_comments_admin,
					insp_status,qc_required,upload,upload_flag,lcl,gp20,gp40,hc40,adjustment,LO_id,acceptance_qc1,acceptance_qc2,acceptance_qc3,
					acceptance_qc4,acceptance_fc,acceptance_cc,insp_qc5,insp_qc4,etd,eta,insp_batch_inspection,insp_executor) 
					VALUES(@insp_unique,@insp_id,@insp_type,@insp_start,@insp_end,@insp_version,@insp_days,@insp_porderid,@insp_qc1,@insp_qc2,
						@insp_qc3,@insp_qc6,@insp_fc,@customer_code,@custpo,@batch_no,@factory_code,@insp_comments,@insp_comments_admin,@insp_status,
						@qc_required,@upload,@upload_flag,@lcl,@gp20,@gp40,@hc40,@adjustment,@LO_id,@acceptance_qc1,@acceptance_qc2,@acceptance_qc3,
						@acceptance_qc4,@acceptance_fc,@acceptance_cc,@insp_qc5,@insp_qc4,@etd,@eta,@insp_batch_inspection,@insp_executor)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE inspections SET insp_id = @insp_id,insp_type = @insp_type,insp_start = @insp_start,insp_end = @insp_end,
				insp_version = @insp_version,insp_days = @insp_days,insp_porderid = @insp_porderid,insp_qc1 = @insp_qc1,insp_qc2 = @insp_qc2,
				insp_qc3 = @insp_qc3,insp_qc6 = @insp_qc6,insp_fc = @insp_fc,customer_code = @customer_code,custpo = @custpo,batch_no = @batch_no,
				factory_code = @factory_code,insp_comments = @insp_comments,insp_comments_admin = @insp_comments_admin,insp_status = @insp_status,
				qc_required = @qc_required,upload = @upload,upload_flag = @upload_flag,lcl = @lcl,gp20 = @gp20,gp40 = @gp40,hc40 = @hc40,
				adjustment = @adjustment,LO_id = @LO_id,acceptance_qc1 = @acceptance_qc1,acceptance_qc2 = @acceptance_qc2,acceptance_qc3 = @acceptance_qc3,
				acceptance_qc4 = @acceptance_qc4,acceptance_fc = @acceptance_fc,acceptance_cc = @acceptance_cc,insp_qc5 = @insp_qc5,insp_qc4 = @insp_qc4,
				etd = @etd,eta = @eta,insp_batch_inspection = @insp_batch_inspection,insp_executor = @insp_executor WHERE insp_unique = @insp_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspections WHERE insp_unique = @id";
		}
		
		public override Inspections GetById(object id)
		{
			Inspections result = null;
			conn.Query<Inspections, Inspection_lines_tested, Inspections>(
				@"SELECT i.*, ilt.* FROM inspections i LEFT OUTER JOIN inspection_lines_tested ilt ON i.insp_unique = ilt.insp_id
					WHERE i.insp_unique = @id",
				(i, ilt) =>
				{
					if(result == null)
					{
						result = i;
						i.LinesTested = new List<Inspection_lines_tested>();						
					}					
					if(ilt != null)
						result.LinesTested.Add(ilt);
					return i;
				}, new {id}, splitOn: "insp_line_unique").FirstOrDefault();
            if(result != null)
            {
                AddLegacyControllers(result);
				result.LinesAccepted = inspectionLinesAcceptedDal.GetByInspection((int) id);
				result.LinesRejected = inspectionLinesRejectedDal.GetByInspection((int) id);
				var images = inspectionImagesDAL.GetByInspection((int) id);
				foreach(var im in images)
				{
					var laLines = result.LinesAccepted.Where(x =>x.insp_line_id == im.insp_line_unique && x.insp_line_type == im.insp_type).ToList();
					foreach(var la in laLines)
					{
						if(la.Images == null)
							la.Images = new List<Inspection_images>();
						la.Images.Add(im);						
					}
					
				}
            }
			return result;
		}

        public Inspections GetByNewInspId(int newid)
        {
	        var result =
		        conn.QueryFirstOrDefault<Inspections>("SELECT * FROM inspections WHERE new_insp_id = @newid", new {newid});
            if(result != null)
            {
	            AddLegacyControllers(result);
            }
			return result;
        }

	    private void AddLegacyControllers(Inspections result)
	    {
		    var id = result.insp_unique;
		    result.Controllers = inspectionControllerDal.GetByInspection(id);
		    if (result.Controllers.Count == 0)
		    {
			    if (result.insp_qc1 > 0)
				    result.Controllers.Add(new Inspection_controller
				    {
					    controller_id = result.insp_qc1.Value,
					    inspection_id = id,
					    Controller = userDal.GetById(result.insp_qc1.Value),
					    startdate = result.insp_start ?? DateTime.Today,
					    duration = result.insp_days ?? 1
				    });
			    if (result.insp_qc2 > 0)
				    result.Controllers.Add(new Inspection_controller
				    {
					    controller_id = result.insp_qc2.Value,
					    inspection_id = id,
					    Controller = userDal.GetById(result.insp_qc2.Value),
					    startdate = result.insp_start ?? DateTime.Today,
					    duration = result.insp_days ?? 1
				    });
			    if (result.insp_qc3 > 0)
				    result.Controllers.Add(new Inspection_controller
				    {
					    controller_id = result.insp_qc3.Value,
					    inspection_id = id,
					    Controller = userDal.GetById(result.insp_qc3.Value),
					    startdate = result.insp_start ?? DateTime.Today,
					    duration = result.insp_days ?? 1
				    });
			    if (result.insp_qc4 > 0)
				    result.Controllers.Add(new Inspection_controller
				    {
					    controller_id = result.insp_qc4.Value,
					    inspection_id = id,
					    Controller = userDal.GetById(result.insp_qc4.Value),
					    startdate = result.insp_start ?? DateTime.Today,
					    duration = result.insp_days ?? 1
				    });
			    if (result.insp_qc5 > 0)
				    result.Controllers.Add(new Inspection_controller
				    {
					    controller_id = result.insp_qc5.Value,
					    inspection_id = id,
					    Controller = userDal.GetById(result.insp_qc5.Value),
					    startdate = result.insp_start ?? DateTime.Today,
					    duration = result.insp_days ?? 1
				    });
		    }
	    }

	    public Inspections GetByLoadingId(int loadingId)
	    {
		    return conn.QueryFirstOrDefault<Inspections>("SELECT * FROM inspections WHERE lo_id = @loadingId",
			    new {loadingId});
	    }
		
	
		
		public override void Create(Inspections o, IDbTransaction tr = null)
        {
            var shouldCommit = tr == null;
			
	        if (tr == null)
		        tr = conn.BeginTransaction();

            try
            {
				base.Create(o, tr);    
				conn.Execute("UPDATE nextinsp SET nextorderid = @id", o.insp_unique+1);
				
                if (o.Controllers != null && o.Controllers.Count > 0)
                {
	                foreach (var c in o.Controllers)
	                {
		                c.inspection_id = o.insp_unique;
						inspectionControllerDal.Create(c, tr);
	                }
                }

				if(shouldCommit)
					tr.Commit();
            }
            catch (Exception)
            {
				if(shouldCommit)
					tr.Rollback();
                throw;
            }
		}
		
		

        public  List<Inspections> GetFinalInspectionsForNS(DateTime? from)
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
                }, new { from }, splitOn: "insp_line_unique"
                );
            return result;
            
        }

        public void Update(Inspections o, List<Inspection_controller> deletedControllers, IDbTransaction tr = null)
		{
			var shouldCommit = tr == null;
			
			if (tr == null)
			{
				if (conn.State != ConnectionState.Open)
					conn.Open();
				tr = conn.BeginTransaction();
			}
				
		    try
		    {
		        base.Update(o, tr);

                if (o.Controllers != null)
                {
                    foreach (var c in o.Controllers)
                    {
                        if (c.id <= 0)
                            inspectionControllerDal.Create(c, tr);
                        else
                        {
                            inspectionControllerDal.Update(c, tr);
                        }
                    }
                }
                foreach (var d in deletedControllers)
                {
                    inspectionControllerDal.Delete(d.id, tr);
                }

				if(shouldCommit)
					tr.Commit();
		    }
		    catch
		    {
				if(shouldCommit)
					tr.Rollback();
		        throw;
		    }
		    
		}
		
		
        public List<Inspections> GetByCriteria(DateTime dateFrom, DateTime dateTo, int? locationId)
        {
            
	        var result = conn.Query<Inspections, Company, Inspections>(
		        @"SELECT inspections.*, factory.* FROM inspections LEFT OUTER JOIN users AS factory ON inspections.factory_code = factory.factory_code 
                WHERE (insp_start BETWEEN @dateFrom AND @dateTo OR DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) BETWEEN @dateFrom AND @dateTo 
				OR (insp_start <= @dateFrom AND DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) >= @dateTo)) 
                AND (factory.consolidated_port2 = @locationid OR @locationid IS NULL OR factory.consolidated_port_mix = 1 
					OR (factory.factory_code IS NULL AND insp_type = 'X')) 
                AND COALESCE(insp_batch_inspection,0) = 0",
		        (i, f) =>
		        {
			        i.Factory = f;
			        return i;
		        }, new {dateFrom, dateTo, locationId}, splitOn: "user_id").ToList();
	        foreach (var r in result)
	        {
			    r.Controllers = inspectionControllerDal.GetByInspection(r.insp_unique);                
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
        public List<Inspections> GetForExport(DateTime dateFrom, DateTime dateTo,string[] factorycodes, int? user_id = null)
        {

	        return conn.Query<Inspections, Company, Company, Inspections>(
		        $@"SELECT inspections.*, factory.*, customer.* FROM inspections LEFT OUTER JOIN users AS factory ON inspections.factory_code = factory.factory_code 
                                LEFT OUTER JOIN users customer ON inspections.customer_code = customer.customer_code {
				        (user_id != null
					        ? " INNER JOIN admin_permissions ON factory.user_id = admin_permissions.cusid AND admin_permissions.userid = @user_id"
					        : "")
			        }
                    WHERE (insp_start BETWEEN @dateFrom AND @dateTo OR DATE_ADD(insp_start,INTERVAL insp_days-1 DAY) 
					BETWEEN @dateFrom AND @dateTo) AND inspections.factory_code IN @factorycodes 
                    ORDER BY inspections.custpo, inspections.factory_code ASC ,insp_start DESC",
		        (i, f, c) =>
		        {
			        i.Factory = f;
			        i.customer_code = c.customer_code;
			        i.customer_id = c.user_id;
			        return i;
		        }, new {dateFrom, dateTo, factorycodes}, splitOn: "user_id, user_id").Distinct(new InspectionComparer()).ToList();

            
        }

        public List<Inspections> GetLoadingInspections(string[] factory_codes, string[] customer_codes,string[] custpos)
        {
	        return conn.Query<Inspections>(
		        @"SELECT * FROM inspections WHERE factory_code IN @factory_codes AND customer_code IN @customer_codes AND custpo IN @custpos AND insp_type = 'LO'",
		        new {factory_codes, customer_codes, custpos}).ToList();
	        
	    }

        public int GetChangedProductCount(string[] custpo,string factory_code)
        {

	        return conn.ExecuteScalar<int>(
		        "SELECT COUNT(DISTINCT mastid) FROM asaq.2011_change_notice_view WHERE factory_code = @factory_code AND product_po IN @custpo",
		        new {custpo, factory_code});

        }

        public Inspections GetInspection(string custpo, string factory_code, string customer_code)
        {
	        return conn.QueryFirstOrDefault<Inspections>(
		        @"SELECT * FROM inspections WHERE custpo LIKE @custpo AND factory_code = @factory_code AND customer_code = @customer_code",
		        new {custpo = "%" + custpo + "%", factory_code, customer_code});
			
        }

        public List<Inspections> GetForCustPo(string custpo)
        {
	        return conn.Query<Inspections>(
		        @"SELECT * FROM inspections WHERE custpo LIKE @custpo",
		        new {custpo = "%" + custpo + "%"}).ToList();
            
        }

        public List<Inspections> GetForFeedback(Returns r, bool pendingOnly = true)
        {
			var customer = companyDal.GetById(r.client_id ?? 0);
            var product = custproductsDal.GetById(r.cprod_id ?? 0);

	        return conn.Query<Inspections>(
		        $@"SELECT * FROM inspections WHERE inspections.customer_code = @customer_code 
                 AND EXISTS (SELECT insp_id FROM inspection_lines_tested WHERE inspection_lines_tested.insp_id = inspections.insp_unique AND inspection_lines_tested.insp_client_ref = @cprod_code)
                AND inspections.insp_id <> 'create report' {
				        (pendingOnly ? "AND inspections.insp_start > NOW()" : "")
			        }",
		        new {customer.customer_code, cprod_code = product.cprod_code1}).ToList();
			
        }

        public List<Inspections> GetForQcs(DateTime? from, DateTime? to, IList<int?> qc_ids)
        {
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
                            ", new { from, to, ids = qc_ids.ToArray() }).ToList();                    
            
        }

		public int GetIdFromIdString(string sId, bool isV2 = false)
		{
			int id;
			var isInt = int.TryParse(sId, out id);
			if (isInt)
				return id;
			var query = !isV2
				? "SELECT insp_unique FROM inspections WHERE id = @id"
				: "SELECT new_insp_id FROM inspections WHERE id = @id ";
			return conn.ExecuteScalar<int>(query, new {id = sId});
		}

		public List<Inspections> GetByCriteria(DateTime? dateFrom = null, DateTime? dateTo = null, int? factoryId = null, 
			int? clientId = null, string custpo = null, string productCode = null, int? statusId = null, int? userId = null)
		{
			var inspections = new List<Inspections>();
			string factoryCode = null, customerCode = null;
			if(factoryId != null)
			{
				factoryCode = companyDal.GetById(factoryId.Value)?.factory_code;
			}
			if(clientId != null)
			{
				customerCode = companyDal.GetById(clientId.Value)?.customer_code;
			}
			conn.Query<Inspections, Inspection_lines_tested, Inspection_v2, Inspections>(
				$@"SELECT inspections.*, inspection_lines_tested.*, inspection_v2.*
				  FROM inspections LEFT OUTER JOIN inspection_lines_tested
				  ON inspections.insp_unique = inspection_lines_tested.insp_id 
				  LEFT OUTER JOIN inspection_v2 ON inspections.new_insp_id = inspection_v2.id
				  WHERE (factory_code = @factoryCode OR @factoryCode IS NULL) AND
						(customer_code = @customerCode OR @customerCode IS NULL) AND
						(insp_start >= @dateFrom OR @dateFrom IS NULL) AND 
						(insp_start <= @dateTo OR @dateTo IS NULL)
				  {(custpo != null ? " AND inspections.custpo LIKE @custpo " : "")}
				  {(productCode != null ? BuildProductCodeClause() : "")}
				  {(statusId != null ? BuildStatusClause(statusId.Value) : "")}
				  {(userId != null ? BuildUserClause() : "")}
				",
				(i, ilt, iv2) =>
				{
					var insp = inspections.FirstOrDefault(x=>x.insp_unique == i.insp_unique);
					if(insp == null)
					{
						i.LinesTested = new List<Inspection_lines_tested>();
						i.Inspection_V2 = iv2;
						inspections.Add(i);
						insp = i;
					}
					if(ilt != null)
						insp.LinesTested.Add(ilt);
					return i;
				}, new {dateFrom, dateTo, factoryCode, customerCode, custpo = custpo != null ? "%" + custpo + "%" : null, 
				productCode = productCode != null ? "%" + productCode + "%" : null, userId }, splitOn: "insp_line_unique, id");

			var newInspIds = inspections.Select(i=>i.new_insp_id).ToList();

			return inspections;
		}

		private string BuildStatusClause(int statusId)
		{
			var result = string.Empty;
			switch(statusId)
			{
				case (int) InspectionStatus.Todo:
					result = @" AND inspections.insp_status = 0 AND inspections.acceptance_FC <> 2 
								AND (inspection_v2.insp_status <> 2 OR isnull(inspection_v2.insp_status) OR insp_type = 'FI')";
					break;
				case (int) InspectionStatus.AwaitingReview:
					result = @" AND ((inspections.insp_status = 1 OR inspection_v2.insp_status = 2) AND inspections.acceptance_FC = 0) ";
					break;
				case (int) InspectionStatus.Rejected:
					result = @" AND (inspections.insp_status IN (0,1) AND inspections.acceptance_FC = 2)";
					break;
				case (int)InspectionStatus.Accepted:
					result =  " AND inspections.insp_status = 1 AND inspections.acceptance_FC = 1";
					break;
			}
			return result;
		}

		private string BuildProductCodeClause()
		{
			return @" AND (EXISTS (SELECT cust_products.cprod_id FROM cust_products INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id
								WHERE order_lines.linenum = inspection_lines_tested.order_linenum AND cust_products.cprod_code1 LIKE @productCode OR
								cust_products.cprod_name LIKE @productCode)
							OR EXISTS (SELECT cust_products.cprod_id FROM cust_products INNER JOIN order_lines ON cust_products.cprod_id = order_lines.cprod_id 
									INNER JOIN inspection_v2_line ON order_lines.linenum = inspection_v2_line.orderlines_id
								WHERE inspection_v2_line.insp_id = inspections.new_insp_id AND cust_products.cprod_code1 LIKE @productCode OR
								cust_products.cprod_name LIKE @productCode)
							)";
		}

		private string BuildUserClause()
		{
			return @" AND (EXISTS (SELECT * FROM inspection_controller WHERE inspection_id = inspections.insp_unique AND controller_id = @userId) 
						 OR EXISTS(SELECT * FROM inspection_v2_controller WHERE inspection_id = inspections.new_insp_id AND controller_id = @userId) 
						OR inspections.insp_qc1 = @userId OR inspections.insp_qc2 = @userId OR inspections.insp_qc3 = @userId
						OR inspections.insp_qc4 = @userId OR inspections.insp_qc5 = @userId OR inspections.insp_qc6 = @userId )";
		}

		public List<Inspections> GetRelatedLoadingInspections(IList<int> ids)
		{
			var inspections = new List<Inspections>();
			conn.Query<Inspection_v2, Inspection_v2_line,Inspections, Inspections>(
				@"SELECT  inspection_v2.*, inspection_v2_line.*, inspections.*
				FROM inspections INNER JOIN inspection_v2 ON inspections.new_insp_id = inspection_v2.id 
				INNER JOIN inspection_v2_line ON inspection_v2.id = inspection_v2_line.insp_id
				WHERE EXISTS
				(SELECT fi_ilt.insp_id FROM inspection_lines_tested fi_ilt 
				WHERE inspection_v2_line.orderlines_id = fi_ilt.order_linenum AND fi_ilt.insp_id IN @ids)",
				(iv2, iv2l, i) =>
				{
					var insp = inspections.FirstOrDefault(x=>x.insp_unique == i.insp_unique);
					if(insp == null)
					{
						i.Inspection_V2 = iv2;
						i.Inspection_V2.Lines = new List<Inspection_v2_line>();
						inspections.Add(i);
						insp = i;
					}
					if(iv2l != null)
						insp.Inspection_V2.Lines.Add(iv2l);
					return i;
				}, new { ids}, splitOn: "id, insp_unique").ToList();
			return inspections;
		}

		public List<nr_line_legacy> GetNRLegacyLines(IList<string> custpos)
		{
			return conn.Query<nr_line_legacy>("SELECT * FROM 2012_nr WHERE insp_custpo IN @custpos", new { custpos}).ToList();
		}

		protected override string IdField => "insp_unique";
		protected override bool IsAutoKey => false;
	}

	public class InspectionComparer : IEqualityComparer<Inspections>
	{
		
		public bool Equals(Inspections x, Inspections y)
		{
			return x.insp_unique == y.insp_unique;
		}

		public int GetHashCode(Inspections obj)
		{
			return obj.insp_unique;
		}
	}
}
			
			
			
			