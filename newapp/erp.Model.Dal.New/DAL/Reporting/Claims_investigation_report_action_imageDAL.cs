using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ClaimsInvestigationReportActionImageDAL : GenericDal<Claims_investigation_report_action_images>, IClaimsInvestigationReportActionImageDAL
	{
		public ClaimsInvestigationReportActionImageDAL(IDbConnection conn) : base(conn)
		{
		}

		


        public List<Claims_investigation_report_action_images> GetImagesForAction(int id)
        {
            return conn.Query<Claims_investigation_report_action_images>(
				@"SELECT * FROM claims_investigation_report_action_images WHERE  action_id=@action_id", new {action_id = id}).ToList();
        }

        

		protected override string GetAllSql()
		{
			return @"SELECT * FROM claims_investigation_report_action_images";
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO `asaq`.`claims_investigation_report_action_images`
					(`action_id`,`name`,`image_title`)
					VALUES
					(@id,@action_id,@name,@image_title)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE `asaq`.`claims_investigation_report_action_images`
						SET
						`action_id` = @action_id,
						`name` = @name,
						`image_title` = @image_title
						WHERE `id` = @id;";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM claims_investigation_report_action_images WHERE id=@id";
		}
	}
}
