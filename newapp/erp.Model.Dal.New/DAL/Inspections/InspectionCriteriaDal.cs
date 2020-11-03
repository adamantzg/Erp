using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionCriteriaDal : GenericDal<Inspection_criteria>, IInspectionCriteriaDal
    {
        public InspectionCriteriaDal(IDbConnection conn) : base(conn)
        {
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

        public List<Inspection_criteria> GetForCategory1(int category1_id)
        {
            var sql = "SELECT * FROM inspection_criteria WHERE category1_id = @category1_id";
            return conn.Query<Inspection_criteria>(sql, new { category1_id }).ToList();
        }
    }
}
