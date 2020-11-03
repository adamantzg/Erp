using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ContainerDal : GenericDal<Containers>, IContainerDal
    {
        public ContainerDal(IDbConnection conn) : base(conn)
        {
        }

        public List<Containers> GetForInspection(int insp_id)
        {
            return conn.Query<Containers>("SELECT * FROM containers WHERE insp_id = @insp_id", new { insp_id }).ToList();
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

        protected override string GetDeleteSql()
        {
            throw new NotImplementedException();
        }

        protected override string GetUpdateSql()
        {
            throw new NotImplementedException();
        }
    }
}
