using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public class ContainerTypesDal : GenericDal<Container_types>, IContainerTypesDal
	{
		public ContainerTypesDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM container_types";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM container_types WHERE container_type_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO container_types (container_type_desc,width,length,height) VALUES(@container_type_desc,@width,@length,@height)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM container_types WHERE container_type_id = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE container_types SET container_type_desc = @container_type_desc,width = @width,length = @length,height = @height 
				WHERE container_type_id = @container_type_id";
		}

		protected override string IdField => "container_type_id";
	}
}
