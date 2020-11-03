
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace erp.Model.Dal.New
{
    public class FeedbackTypeDAL : GenericDal<Feedback_type>, IFeedbackTypeDAL
	{
		public FeedbackTypeDAL(IDbConnection conn) : base(conn)
		{
		}
		
		protected override string IdField => "type_id";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM feedback_type";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM feedback_type WHERE type_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO feedback_type (type_id,typename) VALUES(@type_id,@typename)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE feedback_type SET typename = @typename WHERE type_id = @type_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM feedback_type WHERE type_id = @id";
		}
	}
}
			
			