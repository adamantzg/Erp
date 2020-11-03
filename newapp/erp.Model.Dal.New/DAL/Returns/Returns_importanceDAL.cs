
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ReturnsImportanceDAL : GenericDal<Returns_importance>, IReturnsImportanceDAL
	{
		public ReturnsImportanceDAL(IDbConnection conn) : base(conn)
		{
		}
        public List<Returns_importance> GetForType(int type)
        {
			return conn.Query<Returns_importance>(
				"SELECT * FROM returns_importance WHERE feedback_type_id = @feedback_type",
				new {feedback_type = type}
				).ToList();            
        }

		protected override string IdField => "importance_id";
		protected override bool IsAutoKey => false;

		protected override string GetAllSql()
		{
			return "SELECT * FROM returns_importance";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM returns_importance WHERE importance_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO returns_importance (importance_id,importance_text,feedback_type_id) VALUES(@importance_id,@importance_text,@feedback_type_id)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE returns_importance SET importance_text = @importance_text,feedback_type_id = @feedback_type_id WHERE importance_id = @importance_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM returns_importance WHERE importance_id = @id";
		}
	}
}
			
			