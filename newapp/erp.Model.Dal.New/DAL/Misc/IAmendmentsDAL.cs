using System;
using System.Collections.Generic;

namespace erp.Model.Dal.New
{
	public interface IAmendmentsDAL: IGenericDal<Amendments>
	{
		List<Amendments> GetByUserName(string username);
		List<Amendments> GetByCriteria(string processName);
		List<Amendments> GetByCriteria(IList<string> processes, DateTime? from, DateTime? to);
	}
}