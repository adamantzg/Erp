using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class InspectionLinesNotifiedDal : GenericDal<inspection_lines_notified>, IInspectionLinesNotifiedDal
	{
		public InspectionLinesNotifiedDal(IDbConnection conn) : base(conn)
		{
		}

		public List<inspection_lines_notified> GetByInspection(int id)
		{
			return conn.Query<inspection_lines_notified>(
				"SELECT * FROM inspection_lines_notified WHERE insp_unique = @id", new {id}).ToList();
		}

		protected override string IdField => "insp_line_unique";

		protected override string GetAllSql()
		{
			return "SELECT * FROM inspection_lines_notified";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspection_lines_notified WHERE insp_line_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO `asaq`.`inspection_lines_notified`
					(`insp_line_unique`,`insp_unique`,
					`insp_line_id`,`insp_line_type`,
					`insp_line_rejection`,`insp_line_action`,
					`insp_po_linenum`,`insp_qty2`,`insp_qty3`,
					`insp_ca2`,`insp_comments`,`insp_reason`,
					`insp_permanent_action`,`insp_document`,
					`insp_pdf`,`changed_details`,
					`etd`,`eta`,`master_line`,`insp_container_number`)
					VALUES
					(@insp_line_unique,@insp_unique,@insp_line_id,
					@insp_line_type,@insp_line_rejection,
					@insp_line_action,@insp_po_linenum,@insp_qty2,
					@insp_qty3,@insp_ca2,@insp_comments,
					@insp_reason,@insp_permanent_action,@insp_document,
					@insp_pdf,@changed_details,@etd,@eta,
					@master_line,@insp_container_number)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspection_lines_notified WHERE insp_line_unique = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE `asaq`.`inspection_lines_notified`
					SET
					`insp_unique` = @insp_unique,
					`insp_line_id` = @insp_line_id,
					`insp_line_type` = @insp_line_type,
					`insp_line_rejection` = @insp_line_rejection,
					`insp_line_action` = @insp_line_action,
					`insp_po_linenum` = @insp_po_linenum,
					`insp_qty2` = @insp_qty2,
					`insp_qty3` = @insp_qty3,
					`insp_ca2` = @insp_ca2,
					`insp_comments` = @insp_comments,
					`insp_reason` = @insp_reason,
					`insp_permanent_action` = @insp_permanent_action,
					`insp_document` = @insp_document,
					`insp_pdf` = @insp_pdf,
					`changed_details` = @changed_details,
					`etd` = @etd,
					`eta` = @eta,
					`master_line` = @master_line,
					`insp_container_number` = @insp_container_number
					WHERE `insp_line_unique` = @insp_line_unique";
		}
	}
}
