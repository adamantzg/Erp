
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionLinesAcceptedDal : GenericDal<Inspection_lines_accepted>, IInspectionLinesAcceptedDal
	{
		public InspectionLinesAcceptedDal(IDbConnection conn) : base(conn)
		{
		}			

        public List<Inspection_lines_accepted> GetByInspection(int insp_id)
        {
			return conn.Query<Inspection_lines_accepted, Inspection_lines_tested, Inspection_lines_accepted>(
				@"SELECT inspection_lines_accepted.*, inspection_lines_tested.*
				  FROM inspection_lines_accepted INNER JOIN inspection_lines_tested 
				  ON inspection_lines_accepted.insp_line_id = inspection_lines_tested.insp_line_unique
                  WHERE inspection_lines_accepted.insp_unique = @insp_id", 
				(la, lt) =>
				{
					la.LineTested = lt;
					return la;
				}, new { insp_id}, splitOn: "insp_line_unique").ToList();            
        }

		protected override string IdField => "insp_line_unique";

		protected override string GetAllSql()
		{
			return "SELECT * FROM inspection_lines_accepted";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspection_lines_accepted WHERE insp_line_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO inspection_lines_accepted (insp_unique,insp_line_id,insp_line_type,insp_line_comments,insp_po_linenum,insp_qty2,insp_qty3) 
				VALUES(@insp_unique,@insp_line_id,@insp_line_type,@insp_line_comments,@insp_po_linenum,@insp_qty2,@insp_qty3)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE inspection_lines_accepted SET insp_unique = @insp_unique,insp_line_id = @insp_line_id,insp_line_type = @insp_line_type,
					insp_line_comments = @insp_line_comments,insp_po_linenum = @insp_po_linenum,insp_qty2 = @insp_qty2,insp_qty3 = @insp_qty3 
					WHERE insp_line_unique = @insp_line_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspection_lines_accepted WHERE insp_line_unique = @id";
		}
	}
}
			
			