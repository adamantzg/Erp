
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class FeedbackSubscriptionsDAL : GenericDal<Feedback_subscriptions>, IFeedbackSubscriptionsDAL
    {
	    public FeedbackSubscriptionsDAL(IDbConnection conn) : base(conn)
	    {
	    }

	    protected override string GetAllSql()
		{
			return "SELECT * FROM feedback_subscriptions";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM feedback_subscriptions WHERE subs_id = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO feedback_subscriptions (subs_returnid,subs_useruserid,subs_type,subs_leader) 
					VALUES(@subs_returnid,@subs_useruserid,@subs_type,@subs_leader)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE feedback_subscriptions SET subs_returnid = @subs_returnid,subs_useruserid = @subs_useruserid,subs_type = @subs_type,
				subs_leader = @subs_leader WHERE subs_id = @subs_id";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM feedback_subscriptions WHERE subs_id = @id";
		}

		protected override string IdField => "subs_id";

		public List<Feedback_subscriptions> GetForReturn(int return_id)
		{
			return conn.Query<Feedback_subscriptions, User, Feedback_subscriptions>(
				@"SELECT feedback_subscriptions.*, userusers.* 
					FROM feedback_subscriptions INNER JOIN userusers ON feedback_subscriptions.subs_useruserid = userusers.useruserid 
                    WHERE subs_returnid = @return_id",
				(s, u) =>
				{
					s.User = u;
					return s;
				}, new {return_id}, splitOn: "useruserid"
			).ToList();
			
        }
		
		
	}
}
			
			