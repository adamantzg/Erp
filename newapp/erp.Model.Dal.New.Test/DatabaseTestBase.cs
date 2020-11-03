using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using asaq2.Model.Dal.New;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace asaq2.Model.Dal.New.Test
{
	public abstract class DatabaseTestBase
	{
		protected MySqlConnection conn;
		protected abstract string GetCreateSql();
		protected virtual bool IsAutoKey => true;
		protected virtual string IdField => "id";

		
		public DatabaseTestBase()
		{
			conn = GetAndOpenConnection();
		}

		public DatabaseTestBase(IDbConnection conn)
		{
			this.conn = (MySqlConnection) conn;
		}

		[TestInitialize]
		public virtual void Init()
		{

		}
		
		protected MySqlConnection GetAndOpenConnection()
		{
			var c = new MySqlConnection(Properties.Settings.Default.connString);
			c.Open();
			return c;
		}

		protected int GetLastInsertId()
		{
			return conn.ExecuteScalar<int>("SELECT LAST_INSERT_ID()");
		}

		public virtual void GenerateTestData<T>(IEnumerable<T> data, IDbConnection conn = null)
		{
			PropertyInfo propId = null;
			foreach (var d in data)
			{
				GenerateRecord(d, propId, (conn ?? this.conn));
			}
		}

		public virtual void GenerateRecord<T>(T data, PropertyInfo propId = null, IDbConnection conn = null)
		{
			(conn ?? this.conn).Execute(GetCreateSql(), data);
			if (IsAutoKey)
			{
				if (propId == null)
					propId = data.GetType().GetProperty(IdField);
				if(propId != null)
					propId.SetValue(data, GetLastInsertId());
			}
		}

		public virtual void UpdateRecord<T>(T data, IDbConnection conn = null)
		{
			(conn ?? this.conn).Execute(GetUpdateSql(), data);
		}

		[TestCleanup]
		public virtual void Cleanup()
		{
			var tableName = GetTableName();
			if(!string.IsNullOrEmpty(tableName))
				conn.Execute("DELETE FROM " + tableName);
		}

		private string GetTableName()
		{
			var tableAttr = GetType().CustomAttributes
				.FirstOrDefault(a => a.AttributeType == typeof(TableAttribute));			
			if(tableAttr != null)
				return tableAttr.ConstructorArguments[0].Value.ToString();
			return string.Empty;
		}

		//GenericDal test methods
		public virtual void GetAll<T>(GenericDal<T> dal) where T : class
		{
			var props = GetProperties<T>();
			var sql = GetCreateSql();
			var obj = Activator.CreateInstance(typeof(T));
			FillValues(props, obj);
			conn.Execute(sql, obj);

			var result = dal.GetAll();
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
		}
		public virtual void GetById<T>(GenericDal<T> dal) where T: class
		{
			var props = GetProperties<T>();
			var sql = GetCreateSql();
			var obj = Activator.CreateInstance(typeof(T));
			FillValues(props, obj);
			conn.Execute(sql, obj);

			obj = Activator.CreateInstance(typeof(T));
			if(!IsAutoKey)
				obj.GetType().GetProperty(IdField).SetValue(obj, 1);
			conn.Execute(sql, obj);

			var result = dal.GetById(Convert.ToInt32(obj.GetType().GetProperty(IdField).GetValue(obj)));
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.GetType().GetProperty(IdField).GetValue(result));
		}
		public virtual void Create<T>(GenericDal<T> dal) where T : class
		{
			var props = GetProperties<T>();
			var obj = (T) Activator.CreateInstance(typeof(T));
			FillValues(props, obj);
			dal.Create(obj);

			var sql = "SELECT * FROM " + GetTableName();
			var result = conn.Query<T>(sql).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);

		}
		public virtual void Update<T>(GenericDal<T> dal) where T : class
		{
			var props = GetProperties<T>();
			var obj = (T) Activator.CreateInstance(typeof(T));
			FillValues(props, obj);
			var sql = GetCreateSql();
			conn.Execute(sql, obj);
			var prop = props.FirstOrDefault(p=>p.PropertyType == typeof(string));
			if(prop != null)
			{
				prop.SetValue(obj, "test");
				dal.Update(obj);

				sql = "SELECT * FROM " + GetTableName();
				var result = conn.Query<T>(sql).ToList();
				Assert.IsNotNull(result);
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("test", prop.GetValue(result[0]));
			}
			
		}

		public virtual void Delete<T>(GenericDal<T> dal) where T : class
		{
			var props = GetProperties<T>();
			var obj = (T) Activator.CreateInstance(typeof(T));
			FillValues(props, obj);
			var sql = GetCreateSql();
			conn.Execute(sql, obj);

			var id = (int) obj.GetType().GetProperty(IdField).GetValue(obj);

			dal.Delete(id);
			sql = "SELECT * FROM " + GetTableName();
			var result = conn.Query<T>(sql).ToList();
			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count);

		}
		
		private IList<PropertyInfo> GetProperties<T>()
		{
			return typeof(T).GetProperties();
		}

		private void FillValues(IList<PropertyInfo> properties, object obj)
		{
			foreach(var p in properties)
			{
				if(p.PropertyType == typeof(string))
					p.SetValue(obj, string.Empty);
				else if(p.PropertyType == typeof(int))
					p.SetValue(obj, 0);
				else if(p.PropertyType == typeof(DateTime))
					p.SetValue(obj, DateTime.Now);
				else if(p.PropertyType == typeof(double))
					p.SetValue(obj, 0);
			}
		}

		protected virtual string GetUpdateSql()
		{
			return string.Empty;
		}
		
	}
}
