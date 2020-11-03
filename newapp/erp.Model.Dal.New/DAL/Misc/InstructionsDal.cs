using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class InstructionsDal : GenericDal<instructions_new>, IInstructionsDal
	{
		
		public InstructionsDal(IDbConnection conn) : base(conn)
		{
		}

		public List<string> GetForSlaveHost(int host_id)
		{
			return conn.Query<string>("SELECT filename FROM instructions_transfer WHERE host_id = @host_id", new {host_id})
				.ToList();
		}

		public void DeleteTransferData(int host_id, string filename)
		{
			conn.Execute("DELETE FROM instructions_transfer WHERE host_id = @host_id AND filename = @filename",
				new {host_id, filename});
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
			throw new NotImplementedException();
		}

		protected override string GetUpdateSql()
		{
			throw new NotImplementedException();
		}

		protected override string GetDeleteSql()
		{
			throw new NotImplementedException();
		}
	}
}
