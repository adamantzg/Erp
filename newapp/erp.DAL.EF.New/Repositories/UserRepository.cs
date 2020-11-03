using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.New
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public void RemoveUserFromRole(int user_id, int role_id)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM user_role WHERE user_id = @p0 AND role_id = @p1", user_id, role_id);
        }
    }
}
