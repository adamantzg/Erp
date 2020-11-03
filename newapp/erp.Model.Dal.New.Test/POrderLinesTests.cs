using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("porder_lines")]
	public class POrderLinesTests : DatabaseTestBase
	{
		public POrderLinesTests(IDbConnection conn) : base(conn)
		{

		}

		protected override string IdField => "linenum";

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO `asaq`.`porder_lines`
			(`porderid`,`linedate`,`cprod_id`,`desc1`,`orderqty`,
			`pending_orderqty`,`unitprice`,`pending_unitprice`,`unitcurrency`,
			`linestatus`,`mast_id`,`mfg_code`,`asaq_ref`,`soline`,`lme`)
			VALUES
			(@porderid,@linedate,@cprod_id,@desc1,@orderqty,@pending_orderqty,
			@unitprice,@pending_unitprice,@unitcurrency,@linestatus,@mast_id,@mfg_code,@asaq_ref,
			@soline,@lme)";
		}
	}
}
