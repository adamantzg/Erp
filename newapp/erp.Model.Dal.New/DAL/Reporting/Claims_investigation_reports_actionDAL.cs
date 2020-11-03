using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ClaimsInvestigationReportsActionDAL : GenericDal<Claims_investigation_reports_action>, IClaimsInvestigationReportsActionDAL
	{
		private readonly IClaimsInvestigationReportActionImageDAL claimsInvestigationReportActionImageDAL;

		public ClaimsInvestigationReportsActionDAL(IDbConnection conn, IClaimsInvestigationReportActionImageDAL claimsInvestigationReportActionImageDAL) : base(conn)
		{
			this.claimsInvestigationReportActionImageDAL = claimsInvestigationReportActionImageDAL;
		}
		
        public List<Claims_investigation_reports_action> GetActionsForReports(int unique_id,bool images=false)
        {
            var result = conn.Query<Claims_investigation_reports_action>(
				@"SELECT * FROM claims_investigation_report_action WHERE report_id= @unique_id",
				new {unique_id}).ToList();
			if(images)
            {
                foreach(var a in result)
                {
                    a.ActionImages = claimsInvestigationReportActionImageDAL.GetImagesForAction(a.id);
                }
            }

            return result;
        }

        public Claims_investigation_reports_action GetLastAddedAction()
        {
			return conn.QueryFirstOrDefault<Claims_investigation_reports_action>(
				@"SELECT * FROM
                claims_investigation_report_action                                            
                ORDER BY id
                DESC LIMIT 1");
            
        }


		protected override string GetAllSql()
		{
			return "SELECT * FROM claims_investigation_report_action";                                             
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM claims_investigation_report_action WHERE report_id= @unique_id ";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO claims_investigation_report_action
                                        (id,report_id,comments)
                                  VALUES(@id,@report_id,@comments)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE claims_investigation_report_action 
                        SET 
                        report_id=@report_id,comments=@comments
                        WHERE id=@id";
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}
	}
}
