
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
    public class InspectionControllerDal : GenericDal<Inspection_controller>, IInspectionControllerDal
    {

		public InspectionControllerDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM inspection_controller";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspection_controller WHERE id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO inspection_controller (inspection_id,controller_id,startdate,duration) 
				VALUES(@inspection_id,@controller_id,@startdate,@duration)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE inspection_controller SET inspection_id = @inspection_id,controller_id = @controller_id,startdate = @startdate,
				duration = @duration WHERE id = @id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspection_controller WHERE id = @id";
		}

		
        public List<Inspection_controller> GetByInspection(int inspection_id)
        {
	        return conn.Query<Inspection_controller, User, Inspection_controller>(
		        @"SELECT inspection_controller.*,userusers.* FROM inspection_controller INNER JOIN userusers ON inspection_controller.controller_id = userusers.useruserid
				 WHERE inspection_id = @inspection_id",
		        (c, u) =>
		        {
			        c.Controller = u;
			        return c;
		        }, new {inspection_id}, splitOn: "useruserid").ToList();
                
        }


		
	}
}
			
			