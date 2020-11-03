using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IFeedbackCategoryDAL : IGenericDal<Feedback_category>
	{
		List<Feedback_category> GetForType(int feedback_type);
	}
}