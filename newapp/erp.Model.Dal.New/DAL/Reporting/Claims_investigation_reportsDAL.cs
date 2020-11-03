using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ClaimsInvestigationReportsDAL : GenericDal<Claims_investigation_reports>, IClaimsInvestigationReportsDAL
	{
		private readonly IClaimsInvestigationReportsActionDAL claimsInvestigationReportsActionDAL;

		public ClaimsInvestigationReportsDAL(IDbConnection conn, IClaimsInvestigationReportsActionDAL claimsInvestigationReportsActionDAL) : base(conn)
		{
			this.claimsInvestigationReportsActionDAL = claimsInvestigationReportsActionDAL;
		}

		        

        public List<Claims_investigation_reports> GetForProduct(int cprod_id, bool reports=false)
        {
            var result = conn.Query<Claims_investigation_reports>(
				@"SELECT * FROM claims_investigation_reports WHERE cprod_id=@cprod_id",
				new {cprod_id}).ToList();
			if(reports)
			{
				foreach(var r in result )
				{
					r.ReportActions = claimsInvestigationReportsActionDAL.GetActionsForReports(r.unique_id,images:true);
				}
			}			 
			return result;

        }


        public Claims_investigation_reports GetLastAddedReport()
        {
			return conn.QueryFirstOrDefault<Claims_investigation_reports>(
				@"SELECT * FROM claims_investigation_reports ORDER BY unique_id DESC LIMIT 1");            
        }       

		protected override string IdField => "unique_id";

		protected override string GetAllSql()
		{
			return "SELECT * FROM claims_investigation_reports";
		}

		protected override string GetByIdSql()
		{
			return @"SELECT *FROM claims_investigation_reports WHERE unique_id = @unique_id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO claims_investigation_reports
                        (unique_id,cprod_id,investigation,extras,created_by,date_created,date_modify,modify_by)
                    VALUES(@unique_id,@cprod_id,@investigation,@extras,@created_by,@date_created,@date_modify,@modify_by)";
		}

		protected override string GetUpdateSql()
		{
			return  @"UPDATE claims_investigation_reports 
                        SET investigation=@investigation,extras=@extras,date_modify=@date_modify,modify_by=@modify_by
                        WHERE unique_id=@unique_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM claims_investigation_reports WHERE unique_id=@id";
		}
	}
}
