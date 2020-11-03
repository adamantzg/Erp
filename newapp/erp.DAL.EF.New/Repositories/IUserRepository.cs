using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
    public interface IUserRepository : IGenericRepository<User>
    {
        void RemoveUserFromRole(int user_id, int role_id);
    }
}
