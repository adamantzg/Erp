using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF
{
    public interface IRepository<T>
    {
        void Create(T obj);
        void Update(T obj);
        IList<T> GetAll();
        T GetById(int id);
        void Delete(int id);
    }
}
