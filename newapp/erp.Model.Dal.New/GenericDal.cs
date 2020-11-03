using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
	public abstract class GenericDal<TEntity> : IGenericDal<TEntity> where TEntity : class
	{
		protected abstract string GetAllSql();
		protected abstract string GetByIdSql();
		protected abstract string GetCreateSql();
		protected abstract string GetUpdateSql();
		protected abstract string GetDeleteSql();

		protected virtual bool IsAutoKey => true;
		
		protected virtual string IdField => "id";
		protected virtual string GetNextKeySql => $"SELECT MAX({IdField})+1 FROM {GetTableName(typeof(TEntity))}";

		protected MySqlConnection conn;

		public GenericDal(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		public virtual void Create(TEntity o, IDbTransaction tr = null)
		{
			var insertSql = GetCreateSql();
			var conn = tr != null ? tr.Connection : this.conn;
			var prop = o.GetType().GetProperty(IdField);
			if(!IsAutoKey)
			{
				if(prop != null)
					prop.SetValue(o, conn.ExecuteScalar<int>(GetNextKeySql));
			}
			conn.Execute(insertSql, o, tr);
			if(IsAutoKey && prop != null)
				prop.SetValue(o, conn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()"));
		}

		public virtual void Delete(int id, IDbTransaction tr = null)
		{
			var conn = tr != null ? tr.Connection : this.conn;
			conn.Execute(GetDeleteSql(), new {id}, tr);
		}

		public virtual List<TEntity> GetAll()
		{
			return conn.Query<TEntity>(GetAllSql()).ToList();
		}

		public virtual TEntity GetById(object id)
		{
			return conn.QueryFirstOrDefault<TEntity>(GetByIdSql(), new {id});
		}

		public virtual void Update(TEntity o, IDbTransaction tr = null)
		{
			var conn = tr != null ? tr.Connection : this.conn;
			conn.Execute(GetUpdateSql(), o, tr);
		}

		private string GetTableName(Type type)
		{
			var tableAttr = type.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(TableAttribute));
			if (tableAttr != null)
				return tableAttr.ToString();
			return type.Name;
		}
	}
}
