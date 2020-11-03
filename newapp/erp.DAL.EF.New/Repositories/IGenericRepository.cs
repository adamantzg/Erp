using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace erp.DAL.EF.New
{
	public interface IGenericRepository<TEntity> where TEntity : class
	{
		IEnumerable<TEntity> Get(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,int? take = null, int? skip = null,
			string includeProperties = "");

		IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? take = null, int? skip = null,
			string includeProperties = "");

		TEntity GetByID(object id);
		void Insert(TEntity entity);
		void BulkInsert(IList<TEntity> records);
		void Delete(object id);
		void Delete(TEntity entityToDelete);
		void Update(TEntity entityToUpdate);
		void LoadCollection(TEntity entity, string collName);
		void LoadReference(TEntity entity, string refName);
		void Detach(TEntity entity);
		void Copy(TEntity source, TEntity destination);
		void DeleteAll();
		void DeleteByIds(IEnumerable<int> ids, bool exclusion = false);
		
	}
}