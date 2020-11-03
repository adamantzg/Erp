using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
    public class ProductTrackNumberFcDal : GenericDal<products_track_number_fc>, IProductTrackNumberFcDal
    {
        public ProductTrackNumberFcDal(IDbConnection conn) : base(conn)
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

        public List<products_track_number_fc> GetByCriteria(int? orderid = null, int? mastid = null)
        {
            var sql = @"SELECT * FROM 2012_products_track_number_fc WHERE (orderid = @orderid OR @orderid IS NULL) 
                AND (mastid = @mastid OR @mastid IS NULL)";
            return conn.Query<products_track_number_fc>(sql, new { mastid, orderid }).ToList();
        }
    }
}
