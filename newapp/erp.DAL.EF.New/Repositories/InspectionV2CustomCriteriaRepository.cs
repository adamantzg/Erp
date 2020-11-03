using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.New
{
	public class InspectionV2CustomCriteriaRepository : GenericRepository<Inspv2_customcriteria>, IInspectionV2CustomCriteriaRepository
	{
		public InspectionV2CustomCriteriaRepository(DbContext context) : base(context)
		{
		}

		
	}
}
