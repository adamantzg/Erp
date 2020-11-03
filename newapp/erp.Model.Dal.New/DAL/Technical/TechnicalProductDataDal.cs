using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class TechnicalProductDataDal : GenericDal<Technical_product_data>, ITechnicalProductDataDal
	{
		public TechnicalProductDataDal(IDbConnection conn) : base(conn)
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
			return @"INSERT INTO technical_product_data (mast_id,technical_data_type,technical_data) VALUES(@mast_id,@technical_data_type,@technical_data)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM technical_product_data  WHERE unique_id = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE technical_product_data SET mast_id=@mast_id,technical_data_type=@technical_data_type,technical_data=@technical_data  WHERE unique_id=@unique_id";
		}

		protected override string IdField => "unique_id";

		public override List<Technical_product_data> GetAll()
        {
			return conn.Query<Technical_product_data, Technical_data_type, Technical_product_data>(
				@"SELECT technical_product_data.*, technical_data_type.* FROM technical_product_data 
				INNER JOIN technical_data_type ON technical_product_data.technical_data_type = technical_data_type.data_type_id",
				(tpd, tdt) =>
				{
					tpd.TechnicalDataType = tdt;
					return tpd;
				},splitOn: "data_type_id").ToList();            

        }
        public List<Technical_product_data> GetByProduct(int mast_id)
        {
            return conn.Query<Technical_product_data, Technical_data_type, Technical_product_data>(
				@"SELECT technical_product_data.*, technical_data_type.* FROM technical_product_data 
				INNER JOIN technical_data_type ON technical_product_data.technical_data_type = technical_data_type.data_type_id
				WHERE technical_product_data.mast_id = @mast_id",
				(tpd, tdt) =>
				{
					tpd.TechnicalDataType = tdt;
					return tpd;
				}, new { mast_id }, splitOn: "data_type_id").ToList(); 
                    
        }
	}
}
