using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model.Dal.New
{
	public interface IGenericDal<TEntity> where TEntity : class
	{
		List<TEntity> GetAll();
		TEntity GetById(object id);
		void Create(TEntity o, IDbTransaction tr = null);
		void Update(TEntity o, IDbTransaction tr = null);
		void Delete(int id, IDbTransaction tr = null);
	}
}
