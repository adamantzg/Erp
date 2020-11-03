
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionImagesDAL : GenericDal<Inspection_images>, IInspectionImagesDAL
	{
		public InspectionImagesDAL(IDbConnection conn) : base(conn)
		{
		}

		protected override string IdField => "image_unique";


		public List<Inspection_images> GetByInspection(int insp_id)
        {
			return conn.Query<Inspection_images>("SELECT * FROM inspection_images WHERE insp_unique = @insp_id", new { insp_id }).ToList();            
        }

		public void DeleteMissingForLine(int line_id, string insp_type, IList<int> existingIds)
		{
			conn.Execute("DELETE FROM inspection_images WHERE insp_line_unique = @line_id AND insp_type = @insp_type AND image_unique NOT IN @existingIds", 
				new { line_id, insp_type, existingIds});
		}
		
		
		protected override string GetAllSql()
		{
			return "SELECT * FROM inspection_images";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspection_images WHERE image_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO inspection_images (insp_line_unique,insp_unique,insp_image,insp_type,rej_flag) 
				VALUES(@insp_line_unique,@insp_unique,@insp_image,@insp_type,@rej_flag)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE inspection_images SET insp_line_unique = @insp_line_unique,insp_unique = @insp_unique,insp_image = @insp_image,
					insp_type = @insp_type,rej_flag = @rej_flag WHERE image_unique = @image_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspection_images WHERE image_unique = @id";
		}
	}
}
			
			