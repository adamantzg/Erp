using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IReturnsCommentsFilesDAL : IGenericDal<Returns_comments_files>
	{
		List<Returns_comments_files> GetForComment(int comment_id);
	}
}