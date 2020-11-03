
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ReturnsCommentsFilesDAL : GenericDal<Returns_comments_files>, IReturnsCommentsFilesDAL
	{
		public ReturnsCommentsFilesDAL(IDbConnection conn) : base(conn)
		{
		}				

        public List<Returns_comments_files> GetForComment(int comment_id)
        {
			return conn.Query<Returns_comments_files>(
				"SELECT * FROM returns_comments_files WHERE return_comment_id = @comment_id",
				new {comment_id}).ToList();            
        }	

		protected override string IdField => "return_comment_file_id";

		protected override string GetAllSql()
		{
			return "SELECT * FROM returns_comments_files";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM returns_comments_files WHERE return_comment_file_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO returns_comments_files (return_comment_id,image_id,image_name) VALUES(@return_comment_id,@image_id,@image_name)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE returns_comments_files SET return_comment_id = @return_comment_id,image_id = @image_id,image_name = @image_name 
                                WHERE return_comment_file_id = @return_comment_file_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM returns_comments_files WHERE return_comment_file_id = @id";
		}
	}
}
			
			