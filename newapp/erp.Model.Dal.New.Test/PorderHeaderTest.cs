using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("porder_header")]
	public class PorderHeaderTest : DatabaseTestBase
	{
		public PorderHeaderTest(IDbConnection conn) : base(conn)
		{

		}

		protected override bool IsAutoKey => false;

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO `asaq`.`porder_header`
				(`porderid`,`orderdate`,`userid`,`locid`,`status`,`po_status`,`delivery_address1`,
				`delivery_address2`,`delivery_address3`,`delivery_address4`,`delivery_address5`,
				`delivery_address6`,`delivery_address7`,`currency`,`poname`,`poadd1`,`poadd2`,
				`poadd3`,`poadd4`,`poadd5`,`poadd6`,`soorderid`,`po_req_etd`,`original_po_req_etd`,
				`pending_po_req_etd`,`po_ready_date`,`po_ready`,`po_reference`,`instructions`,
				`po_cfm_etd`,`FPI`,`process_id`,`system_cbm`,`system_sqm`,`factory_cbm`,
				`factory_sqm`,`comments`,`comments_factory`,`special_comments`,`fi_reviewed`,
				`li_reviewed`,`batch_inspection`,`invoices_paid`,`invoices_bl`,`original_process_id`)
				VALUES
				(@porderid,@orderdate,@userid,@locid,@status,@po_status,@delivery_address1,
				@delivery_address2,@delivery_address3,@delivery_address4,@delivery_address5,
				@delivery_address6,@delivery_address7,@currency,@poname,@poadd1,@poadd2,@poadd3,
				@poadd4,@poadd5,@poadd6,@soorderid,@po_req_etd,@original_po_req_etd,@pending_po_req_etd,
				@po_ready_date,@po_ready,@po_reference,@instructions,@po_cfm_etd,@FPI,@process_id,
				@system_cbm,@system_sqm,@factory_cbm,@factory_sqm,@comments,@comments_factory,
				@special_comments,@fi_reviewed,@li_reviewed,@batch_inspection,@invoices_paid,
				@invoices_bl,@original_process_id)";
		}
	}
}
