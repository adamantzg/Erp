using asaq2.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.DAL.EF.New.Test
{
	[TestClass]
	[Table("returns_events")]
	public class ReturnsEventsTests : asaq2.Model.Dal.New.Test.ReturnsEventsTests
	{
		UnitOfWork unitOfWork = new UnitOfWork(new Model("name=connString"));

		[TestMethod]
		public void GetWithParams()
		{
			var data = new List<ReturnsEvents>
			{
				new ReturnsEvents
				{
					event_type = (int) ReturnEventType.Recheck,
					mail_sent = false
				},
				new ReturnsEvents
				{
					event_type = (int) ReturnEventType.CorrectiveActionCreate,
					mail_sent = false
				},
				new ReturnsEvents
				{
					event_type = (int) ReturnEventType.CorrectiveActionCreate,
					mail_sent = true
				}
			};
			GenerateTestData(data);
			var result = unitOfWork.ReturnsEventsRepository.Get(r => (r.event_type == (int)ReturnEventType.Recheck 
				|| r.event_type == (int)ReturnEventType.CorrectiveActionCreate) && r.mail_sent == false).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
		}

		[TestInitialize]
		public override void Init()
		{
			Cleanup();
		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();			
		}
	}
}
