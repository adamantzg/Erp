using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("order_lines")]
	public class OrderLinesTests : DatabaseTestBase
	{
		public OrderLinesTests(IDbConnection conn) : base(conn)
		{

		}

		protected override string IdField => "linenum";

		protected override string GetCreateSql()
		{
			return
			@"INSERT INTO `order_lines`
			(`linenum`,`orderid`,`original_orderid`,`linedate`,`cprod_id`,
			`description`,`spec_code`,`orderqty`,`orig_orderqty`,`unitprice`,
			`unitcurrency`,`override_nw`,`override_gw`,`linestatus`,`lineunit`,
			`factory_group`,`mc_qty`,`pallet_qty`,`unit_qty`,`lme`,`allow_change`,
			`allow_change_down`,`fi_line`,`pack_qty`,`li_line`,`so_ready_date`,
			`special_terms`,`received_qty`)
			VALUES
			(@linenum,@orderid,@original_orderid,@linedate,@cprod_id,
			@description,@spec_code,@orderqty,@orig_orderqty,@unitprice,
			@unitcurrency,@override_nw,@override_gw,@linestatus,@lineunit,
			@factory_group,@mc_qty,@pallet_qty,@unit_qty,@lme,@allow_change,
			@allow_change_down,@fi_line,@pack_qty,@li_line,@so_ready_date,
			@special_terms,@received_qty)";
		}
	}
}
