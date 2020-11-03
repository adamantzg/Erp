
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class DeliveryLocationsDAL : GenericDal<Delivery_locations>, IDeliveryLocationsDAL
    {
		protected override string GetAllSql()
		{
			return "SELECT * FROM delivery_locations";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM delivery_locations WHERE unique_id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO delivery_locations (cus_id,default_flag,del1,del2,del3,del4,del5,del6,del7,delport,inv_flag) 
				VALUES(@cus_id,@default_flag,@del1,@del2,@del3,@del4,@del5,@del6,@del7,@delport,@inv_flag)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE delivery_locations SET cus_id = @cus_id,default_flag = @default_flag,del1 = @del1,del2 = @del2,
				del3 = @del3,del4 = @del4,del5 = @del5,del6 = @del6,del7 = @del7,delport = @delport,inv_flag = @inv_flag WHERE unique_id = @unique_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM delivery_locations WHERE unique_id = @id";
		}

		protected override string IdField => "unique_id";


		public List<Delivery_locations> GetForClient(int client_id)
		{
			return conn.Query<Delivery_locations>("SELECT * FROM delivery_locations WHERE cus_id = @client_id", new {client_id})
				.ToList();
        }
		

		public DeliveryLocationsDAL(IDbConnection conn) : base(conn)
		{
		}
	}
}
			
			