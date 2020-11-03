
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class FeedbackCategoryDAL : GenericDal<Feedback_category>, IFeedbackCategoryDAL
	{
		public FeedbackCategoryDAL(IDbConnection conn) : base(conn)
		{
		}			

        public List<Feedback_category> GetForType(int feedback_type)
        {
			return conn.Query<Feedback_category>(
				"SELECT * FROM feedback_category WHERE feedback_type = @feedback_type",
				new {feedback_type}).ToList();            
        }
		
		protected override string IdField => "feedback_cat_id";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM feedback_category";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM feedback_category WHERE feedback_cat_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO feedback_category (feedback_cat_id,name,feedback_type) VALUES(@feedback_cat_id,@name,@feedback_type)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE feedback_category SET name = @name,feedback_type = @feedback_type WHERE feedback_cat_id = @feedback_cat_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM feedback_category WHERE feedback_cat_id = @id";
		}
	}
}
			
			