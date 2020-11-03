using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using System.Data.Entity;

namespace erp.DAL.EF.Repositories
{
    public class AutomatedTaskRepository
    {
        public static List<AutomatedTask> GetByIds(IList<int> ids)
        {
            using (var m = Model.CreateModel())
            {
                return m.AutomatedTasks.Include(t=>t.Attachments).Include(t=>t.Iterator).Where(t => ids.Contains(t.id)).ToList();
            }
        }
    }
}
