using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model.Dal.New.Test
{
	[Table("returns_events")]
	public class ReturnsEventsTests : DatabaseTestBase
	{
		protected override string GetCreateSql()
		{
			return @"INSERT INTO `asaq`.`returns_events`
					(`event_id`,`return_id`,
					`event_time`,`mail_sent`,
					`event_type`)
					VALUES
					(@event_id,@return_id,
					@event_time,@mail_sent,@event_type)";
		}
	}
}
