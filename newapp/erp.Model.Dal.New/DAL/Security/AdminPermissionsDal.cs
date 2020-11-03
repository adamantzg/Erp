using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class AdminPermissionsDal : GenericDal<Admin_permissions>, IAdminPermissionsDal
	{
		public AdminPermissionsDal(IDbConnection conn) : base(conn)
		{
		}

		public List<Admin_permissions> GetByCompany(int company_id)
		{
			return conn.Query<Admin_permissions, User, Admin_permissions>(
				@"SELECT ap.*, u.* FROM admin_permissions ap INNER JOIN userusers u ON ap.userid = u.useruserid
					WHERE ap.cusid = @company_id",
				(ap, u) =>
				{
					ap.User = u;
					return ap;
				}, new {company_id}, splitOn: "useruserid").ToList();
		}

		protected override string GetAllSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetByIdSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO `asaq`.`admin_permissions`
				(`userid`,`cusid`,`agent`,`clientid`,
				`returns`,`processing`,`feedbacks`)
				VALUES
				(@userid,@cusid,@agent,@clientid,
				@returns,@processing,@feedbacks)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM admin_permissions WHERE permission_id = @id";
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string IdField => "permission_id";
	}
}
