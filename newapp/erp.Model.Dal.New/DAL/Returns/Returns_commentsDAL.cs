
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ReturnsCommentsDAL : GenericDal<Returns_comments>, IReturnsCommentsDAL
	{
		private readonly IReturnsCommentsFilesDAL returnsCommentsFilesDAL;

		public ReturnsCommentsDAL(IDbConnection conn, IReturnsCommentsFilesDAL returnsCommentsFilesDAL) : base(conn)
		{
			this.returnsCommentsFilesDAL = returnsCommentsFilesDAL;
		}

		
        public List<Returns_comments> GetByReturn(int return_id)
        {
			var comments = conn.Query<Returns_comments, User, Returns_comments>(
				@"SELECT returns_comments.*, userusers.* FROM returns_comments INNER JOIN userusers ON returns_comments.comments_from = userusers.useruserid 
				WHERE return_id = @return_id",
				(c, u) =>
				{
					c.Creator = u;
					return c;
				}, new {return_id}, splitOn: "useruserid").ToList();
			foreach(var c in comments)
			{
				c.Files = returnsCommentsFilesDAL.GetForComment(c.comments_id);
			}            
			return comments;
        }
		
		
		public override void Create(Returns_comments o, IDbTransaction tr = null)
        {
			if(conn.State != ConnectionState.Open)
				conn.Open();
			if(tr == null)
				tr = conn.BeginTransaction();

		    try
		    {
                base.Create(o, tr);
		        if (o.Files != null)
		        {
                    foreach (var file in o.Files)
                    {
                        file.return_comment_id = o.comments_id;
                        returnsCommentsFilesDAL.Create(file, tr);
                    }
		        }
		        tr.Commit();
		    }
		    catch (Exception)
		    {
		        tr.Rollback();
		        throw;
		    }
		    finally
		    {
		        tr = null;
		        conn = null;
		    }


        }

		protected override string IdField => "comments_id";

		protected override string GetAllSql()
		{
			return "SELECT * FROM returns_comments";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM returns_comments WHERE comments_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO returns_comments (return_id,comments_type,comments_from,comments_to,comments,comments_date,decision_flag,fc_response) 
					VALUES(@return_id,@comments_type,@comments_from,@comments_to,@comments,@comments_date,@decision_flag,@fc_response)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE returns_comments SET return_id = @return_id,comments_type = @comments_type,comments_from = @comments_from,comments_to = @comments_to,
				comments = @comments,comments_date = @comments_date,decision_flag = @decision_flag,fc_response = @fc_response 
				WHERE comments_id = @comments_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM returns_comments WHERE comments_id = @id";
		}
	}
}
			
			