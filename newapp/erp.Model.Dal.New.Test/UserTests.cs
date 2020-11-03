using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Dapper.FluentMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace asaq2.Model.Dal.New.Test
{
	[TestClass]
	[Table("userusers")]
	public class UserTests : DatabaseTestBase
	{
		private static object mapper = 1;
        private UserDAL userDal;

		public UserTests()
		{

		}
       

        

        public UserTests(IDbConnection conn) : base(conn)
		{
			lock(mapper)
			{
				try
				{
					if (!FluentMapper.EntityMaps.ContainsKey(typeof(User)))
						FluentMapper.Initialize(config => config.AddMap(new UserMap()));
				}
				catch (InvalidOperationException)
				{
					//FluentMapper can raise this
				}
			}
		}

        

        [TestMethod]
        [TestCategory("User Test")]
        public void UserSearchTests()
        {
            var data = new List<User>
            {
                new User{username= "ivica", userpassword="tttt", userwelcome="Ivica Peric" ,user_email="ivica@mail.com" },
                new User{username= "anica", userpassword="aaa", userwelcome="Marica Peric" ,user_email="marica@mail.com" }
            };
            GenerateTestData(data);
            var userDal = new UserDAL(conn, null, null, null, null);
            var result = userDal.GetUsersByCriteria("ivi").Select(s=>s.username).ToArray();

            CollectionAssert.Contains(result, "ivica");
        }
        



        protected override string IdField => "userid";

        [TestInitialize]
        public override void Init()
        {
            base.Init();
            //userDal = new UserDAL(conn, null, null, null, null);

        }

        protected override string GetCreateSql()
		{
			return @"INSERT INTO userusers (userusername,userpassword,userwelcome,user_id,user_level,session,user_email,admin_type,consolidated_port,
					inspection_plan_admin,restrict_ip,ip_address,ip_address1b,ip_address1c,ip_address2,mobilea,mobileb,email_pwd,skype,manager,
					user_initials,status_flag,restricted,qc_technical,login_restriction_from,login_restriction_to,login_restriction_days) 
                    VALUES(@username,@userpassword,@userwelcome,@company_id,@user_level,@session,@user_email,@admin_type,@consolidated_port,
					@inspection_plan_admin,@restrict_ip,@ip_address,@ip_address1b,@ip_address1c,@ip_address2,@mobilea,@mobileb,@email_pwd,@skype,
					@manager,@user_initials,@status_flag,@restricted,@qc_technical,@login_restriction_from,@login_restriction_to,@login_restriction_days)";
		}

        
    }
}
