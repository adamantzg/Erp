using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public class LocationDAL : ILocationDAL
	{
		private MySqlConnection conn;

		public LocationDAL(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public List<Location> GetAll()
		{
			return conn.Query<Location>("SELECT * FROM location").ToList();
		}

		public List<Location> GetForUser(int userid)
		{
			return conn.Query<Location>(
				@"SELECT location.* FROM location INNER JOIN useruser_location ON  location.id = useruser_location.location_id 
					WHERE useruser_location.useruserid = @userid", new {userid}).ToList();
		}
	}
}
