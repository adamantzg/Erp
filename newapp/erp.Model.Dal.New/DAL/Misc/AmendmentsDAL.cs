
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;

namespace erp.Model.Dal.New
{
    public class AmendmentsDAL : GenericDal<Amendments>, IAmendmentsDAL
    {
		private static object mapper = 1;
		public AmendmentsDAL(IDbConnection conn) : base(conn)
	    {
			lock (mapper)
			{
				try
				{
					if (!FluentMapper.EntityMaps.ContainsKey(typeof(Amendments)))
						FluentMapper.Initialize(config => config.AddMap(new AmendmentMap()));
				}
				catch (InvalidOperationException)
				{
					//FluentMapper can raise this
				}
			}
		}
		
        public List<Amendments> GetByUserName(string username)
        {
	        return conn.Query<Amendments>("SELECT * FROM amendments WHERE userid=@username", new {username}).ToList();
        }

        public List<Amendments> GetByCriteria(string processName)
        {
	        return conn.Query<Amendments>("SELECT * FROM amendments WHERE process = @name", new {name = processName}).ToList();
        }
		
		protected override string GetAllSql()
		{
			return "SELECT * FROM amendments";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM amendments WHERE processid = @id";
		}

		protected override string GetCreateSql()
		{
			return 
			@"INSERT INTO `amendments`
			(`processid`,`userid`,`timedate`,`tablea`,`ref1`,`ref2`,`old_data`,
			`new_data`,`process`,`checked`,`checked_user`,`checked_date`,`reason`)
			VALUES
			(@processid,@userid,@timedate,@tablea,@ref1,@ref2,@old_data,@new_data,
			@process,@_checked,@checked_user,@checked_date,@reason)";
		}

		protected override string GetUpdateSql()
		{
			return @"UPDATE amendments SET userid = @userid,timedate = @timedate,tablea = @tablea,ref1 = @ref1,ref2 = @ref2,old_data = @old_data,new_data = @new_data,
		process = @process,checked = @_checked,checked_user = @checked_user,checked_date = @checked_date,reason = @reason WHERE processid = @processid";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM amendments WHERE processid = @id";
		}

		public List<Amendments> GetByCriteria(IList<string> processes, DateTime? from, DateTime? to)
		{
			return conn.Query<Amendments>(@"SELECT * FROM amendments WHERE process IN @processes 
										AND (timedate >= @from OR @from IS NULL) 
										AND (timedate <= @to OR @to IS NULL)", new {processes, from, to}).ToList();	
		}

		protected override bool IsAutoKey => true;
	    protected override string IdField => "processid";
    }

	public class AmendmentMap : EntityMap<Amendments>
	{
		public AmendmentMap()
		{
			Map(u => u._checked).ToColumn("checked");			
		}
	}
}
			
			