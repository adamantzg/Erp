using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace erp.Model.Dal.New
{
	public class ShipmentsDal : GenericDal<Shipments>, IShipmentsDal
	{
		public ShipmentsDal(IDbConnection conn) : base(conn)
		{
		}

		public List<Shipments> GetForOrder(int orderid)
		{
			return conn.Query<Shipments>("SELECT * FROM shipments WHERE orderid = @orderid", new {orderid}).ToList();
		}

		protected override string GetAllSql()
		{
			return "SELECT * FROM shipments";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM shipments WHERE ship_id = @id";
		}

		protected override string GetCreateSql()
		{
			return @"INSERT INTO shipments (orderid,poid,packing_items,shipped_from,shipper_per,sailing_date,shipped_by,container_no,notes,cbm,qty_type_container,gross_weight,forwarder) VALUES(@orderid,@poid,@packing_items,@shipped_from,@shipper_per,@sailing_date,@shipped_by,@container_no,@notes,@cbm,@qty_type_container,@gross_weight,@forwarder)";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM shipments WHERE ship_id = @id";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE shipments SET orderid = @orderid,poid = @poid,packing_items = @packing_items,shipped_from = @shipped_from,shipper_per = @shipper_per,sailing_date = @sailing_date,shipped_by = @shipped_by,container_no = @container_no,notes = @notes,cbm = @cbm,qty_type_container = @qty_type_container,gross_weight = @gross_weight,forwarder = @forwarder WHERE ship_id = @ship_id";
		}

		protected override bool IsAutoKey => true;
		protected override string IdField => "ship_id";
	}
}
