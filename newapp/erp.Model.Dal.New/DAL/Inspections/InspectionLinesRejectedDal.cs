
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionLinesRejectedDal : GenericDal<Inspection_lines_rejected>, IInspectionLinesRejectedDal
	{
		public InspectionLinesRejectedDal(IDbConnection conn) : base(conn)
		{
		}
				
        public List<Inspection_lines_rejected> GetByInspection(int insp_id)
        {
			
			return conn.Query<Inspection_lines_rejected, Inspection_lines_tested, Inspection_lines_rejected>(
				@"SELECT Inspection_lines_rejected.*, inspection_lines_tested.*
				  FROM Inspection_lines_rejected INNER JOIN inspection_lines_tested 
				  ON Inspection_lines_rejected.insp_line_id = inspection_lines_tested.insp_line_unique
                  WHERE Inspection_lines_rejected.insp_unique = @insp_id", 
				(lr, lt) =>
				{
					lr.LineTested = lt;
					return lr;
				}, new { insp_id}, splitOn: "insp_line_unique").ToList();            
        
        }

		protected override string GetAllSql()
		{
			return "SELECT * FROM inspection_lines_rejected";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspection_lines_rejected WHERE insp_line_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO inspection_lines_rejected (insp_unique,insp_line_id,insp_line_type,insp_line_rejection,insp_line_action,insp_po_linenum,
				insp_qty2,insp_qty3,insp_ca,insp_comments,insp_reason,insp_permanent_action,insp_document,insp_pdf,master_line,criteria_id,reworked) 
				VALUES(@insp_unique,@insp_line_id,@insp_line_type,@insp_line_rejection,@insp_line_action,@insp_po_linenum,@insp_qty2,@insp_qty3,@insp_ca,
				@insp_comments,@insp_reason,@insp_permanent_action,@insp_document,@insp_pdf,@master_line,@criteria_id,@reworked)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE inspection_lines_rejected SET insp_unique = @insp_unique,insp_line_id = @insp_line_id,
				insp_line_type = @insp_line_type,insp_line_rejection = @insp_line_rejection,insp_line_action = @insp_line_action,
				insp_po_linenum = @insp_po_linenum,insp_qty2 = @insp_qty2,insp_qty3 = @insp_qty3,insp_ca = @insp_ca,
				insp_comments = @insp_comments,insp_reason = @insp_reason,insp_permanent_action = @insp_permanent_action,insp_document = @insp_document,
				insp_pdf = @insp_pdf,master_line = @master_line,criteria_id = @criteria_id,reworked = @reworked WHERE insp_line_unique = @insp_line_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspection_lines_rejected WHERE insp_line_unique = @id";
		}
	}
}
			
			