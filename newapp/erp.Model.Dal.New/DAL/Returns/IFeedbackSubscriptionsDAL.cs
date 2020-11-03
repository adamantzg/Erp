using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IFeedbackSubscriptionsDAL : IGenericDal<Feedback_subscriptions>
	{
		List<Feedback_subscriptions> GetForReturn(int return_id);
	}
}