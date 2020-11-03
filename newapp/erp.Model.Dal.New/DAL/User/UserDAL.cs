using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using company.Common;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;
using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;

namespace erp.Model.Dal.New
{
    public class UserDAL : IUserDAL
    {
	    private MySqlConnection conn;
	    private IPermissionDAL permissionDal;
	    private ILocationDAL locationDal;
	    private ICurrenciesDAL currenciesDal;
	    private IRoleDAL roleDal;
	    private static object mapper = 1;

	    public UserDAL(IDbConnection conn, IPermissionDAL permissionDal, ILocationDAL locationDal, ICurrenciesDAL currenciesDal, IRoleDAL roleDal)
	    {
		    this.conn = (MySqlConnection) conn;
		    this.permissionDal = permissionDal;
		    this.locationDal = locationDal;
		    this.currenciesDal = currenciesDal;
		    this.roleDal = roleDal;
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

        public User GetUser(string login, string password)
        {
            var result = conn.Query<Company, User, User>(@"SELECT users.*,userusers.*
			           FROM userusers INNER JOIN users ON userusers.user_id = users.user_id  
					   WHERE userusername = @login AND userpassword = @password",
		        (c, u) =>
		        {
			        u.Company = c;
			        return u;
		        }, new {login, password}, splitOn: "useruserid").FirstOrDefault();
	        if (result != null)
	        {
		        result.Permissions = permissionDal.GetForUser(result.userid);
		        result.Roles = roleDal.GetRolesForUser(result.userid);
		        result.Company.Currency = currenciesDal.GetById(result.Company.user_curr.Value);
		        result.Locations = locationDal.GetForUser(result.userid);
		        result.Groups = conn.Query<UserGroup>(@"SELECT usergroup.id, usergroup.name,usergroup.returns_default 
					FROM usergroup INNER JOIN user_group ON usergroup.id = user_group.group_id WHERE user_group.user_id = @id", new { id = result.userid }).ToList();
	        }
            return result;
        }

        public List<User> GetAll()
        {
	        return conn.Query<User>("SELECT * FROM userusers").ToList();
        }

        public User GetByLogin(string login)
        {
	        var result = conn.Query<Company, User,User>(
		        @"SELECT users.*, userusers.*  FROM userusers INNER JOIN users ON userusers.user_id = users.user_id 
			WHERE userusername = @login",
		        (c, u) =>
		        {
			        u.Company = c;
			        return u;
		        }, new {login}, splitOn: "useruserid").FirstOrDefault();
            if (result != null)
            {
                result.Permissions = permissionDal.GetForUser(result.userid);
                result.Company.Currency = currenciesDal.GetById(result.Company?.user_curr ?? 0);
                result.Locations = locationDal.GetForUser(result.userid);
                result.Groups = conn.Query<UserGroup>(@"SELECT usergroup.id, usergroup.name,usergroup.returns_default 
					FROM usergroup INNER JOIN user_group ON usergroup.id = user_group.group_id WHERE user_group.user_id = @id", 
	                new { id = result.userid }).ToList();
                
            }
            return result;
        }

        public User GetById(int user_id)
        {
	        var result = conn.Query<Company, User, User>(
		        @"SELECT users.*, userusers.* FROM userusers INNER JOIN users ON userusers.user_id = users.user_id 
				WHERE useruserid = @user_id", 
				(c, u) =>
				{
					u.Company = c;
					return u;
				}, new {user_id}, splitOn: "useruserid").FirstOrDefault();
            if (result != null)
            {
                result.Locations = locationDal.GetForUser(result.userid);
                result.Company.Currency = currenciesDal.GetById(result.Company.IfNotNull(c=>c.user_curr ?? 0));    
            }
            
            return result;
        }

        public List<User> GetByCompany(int company_id)
        {
	        return conn.Query<User>("SELECT * FROM userusers WHERE user_id = @company_id", new {company_id}).ToList();
        }

        public List<User> GetByCompanyCodes(IList<string> companyCodes )
        {
	        return conn.Query<User>(@"SELECT * FROM userusers INNER JOIN users ON userusers.user_id = users.user_id 
			WHERE users.customer_code IN @companyCodes", new {companyCodes}).ToList();
        }

        public bool IsUserInRoles(string username, IList<UserRole> roleNames)
        {
            bool result = false;
            string sql = string.Empty;
            
            foreach(var roleName in roleNames) {
                if (roleName == UserRole.Administrator) {
                    sql = "SELECT COUNT(*) FROM userusers WHERE userusername = @username AND admin_type > 0 && admin_type <> 5";
                }
                else if (roleName == UserRole.Inspector) {
                    sql = "SELECT COUNT(*) FROM userusers WHERE userusername = @username AND admin_type = 5";
                }
                else if (roleName == UserRole.Distributor) {
                    sql =
                        "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.distributor > 0";
                }
                else if (roleName == UserRole.MasterDistributor) {
                    sql =
                        string.Format(
                            "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_id IN ({0})",
                            Properties.Settings.Default.MasterDistributors);
                }
                else if (roleName == UserRole.HeadDistributor) {
                    sql =
                        string.Format(
                            "SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_id IN ({0})",
                            Properties.Settings.Default.HeadDistributors);
                }
                else if (roleName == UserRole.ExternalUser) {
                    sql = "SELECT COUNT(*) FROM external_user WHERE username = @username";
                }
                else if (roleName == UserRole.Manufacturer) {
                    sql =
                        @"SELECT COUNT(*) FROM cust_products INNER JOIN mast_products ON mast_products.mast_id = cust_products.cprod_mast
                                                    INNER JOIN admin_permissions ON mast_products.factory_id = admin_permissions.cusid
                                                    INNER JOIN userusers ON userusers.useruserid = admin_permissions.userid
                                                    WHERE admin_permissions.`returns` = 1 AND userusers.userusername = @username";
                }
                else if (roleName == UserRole.EBManagement) {
                    sql =
                        "SELECT COUNT(*) FROM userusers WHERE user_id = 1 AND userusername = @username AND admin_type = 10";
                }
                else if (roleName == UserRole.AccountUser) {
                    sql =
                        "SELECT COUNT(*) FROM userusers WHERE user_id = 1 AND userusername = @username AND admin_type = 3";
                }
                else if(roleName == UserRole.UsUser)
                {
                    sql = "SELECT COUNT(*) FROM userusers WHERE user_id = 1105 AND userusername = @username";
                }
                else if (roleName == UserRole.FactoryController || roleName == UserRole.ClientController || roleName == UserRole.QualityAssurance) {
                    sql = $@"SELECT COUNT(*) FROM users
                    INNER JOIN admin_permissions ON users.user_id = admin_permissions.cusid 
                    INNER JOIN userusers ON admin_permissions.userid = userusers.useruserid
                    WHERE userusers.userusername = @username AND  users.user_type = {(roleName == UserRole.FactoryController ? "1" : "3")} 
                        {(roleName == UserRole.ClientController ? "" : $" AND admin_permissions.processing = {(roleName == UserRole.FactoryController ? "1" : "0")}")}";
                }
                else if (roleName == UserRole.FactoryUser)
                {
                    sql =
                        $"SELECT COUNT(*) FROM userusers INNER JOIN users ON userusers.user_id = users.user_id WHERE userusers.userusername = @username AND users.user_type = {(int) Company_User_Type.Factory}";
                }
				else
				{
					sql = $@"SELECT COUNT(*) FROM userusers INNER JOIN user_role ON userusers.useruserid = user_role.user_id INNER JOIN role ON user_role.role_id = role.id 
						WHERE userusers.userusername = @username AND role.name = @rolename";
				}

                if (!string.IsNullOrEmpty(sql))
                {
					int count = conn.ExecuteScalar<int>(sql, new {username, roleName});
                    result = count > 0;
                    if (result)
                        break;
                }
            }
            
            return result;
        }


        public bool IsUserInRole(string username, UserRole roleName)
        {
            return IsUserInRoles(username, new[] { roleName });
            
        }

        public bool IsUserInRole(string username, string roleName)
        {
            return IsUserInRole(username, (UserRole)Enum.Parse(typeof(UserRole), roleName));                        
        }

		

        public UserRole[] GetDynamicUserRoles(string username)
        {
            
            List<UserRole> roles = new List<UserRole>();
            if (IsUserInRole(username, UserRole.Distributor))
                roles.Add(UserRole.Distributor);
            if (IsUserInRole(username, UserRole.MasterDistributor))
                roles.Add(UserRole.MasterDistributor);
            if (IsUserInRole(username, UserRole.HeadDistributor))
                roles.Add(UserRole.HeadDistributor);
            if (IsUserInRole(username, UserRole.Manufacturer))
                roles.Add(UserRole.Manufacturer);
            if (IsUserInRole(username, UserRole.FactoryController))
                roles.Add(UserRole.FactoryController);
            if(IsUserInRole(username, UserRole.FactoryUser))
                roles.Add(UserRole.FactoryUser);
            if (IsUserInRole(username, UserRole.Administrator))
                roles.Add(UserRole.Administrator);
            if (IsUserInRole(username, UserRole.Inspector))
                roles.Add(UserRole.Inspector);
            if (IsUserInRole(username, UserRole.EBManagement))
                roles.Add(UserRole.EBManagement);
            if (IsUserInRole(username, UserRole.UsUser))
                roles.Add(UserRole.UsUser);
            return roles.ToArray();
        }

        public string[] GetRolesForUser(string username)
        {
            var roles = GetDynamicUserRoles(username).Select(r => r.ToString()).ToList();
            var dbRoles = roleDal.GetRolesForUser(username);
            roles.AddRange(dbRoles.Where(dr => roles.Count(r => r == dr.name) == 0).Select(dr => dr.name));
            return roles.ToArray();

            ////var roles = new List<string>(); //GetDynamicUserRoles(username).Select(r => r.ToString()).ToList();
            //var dbRoles = RoleDAL.GetRolesForUser(username);
            ////roles.AddRange(dbRoles.Where(dr => roles.Count(r => r == dr.name) == 0).Select(dr=>dr.name));
            ////roles = dbRoles.Select(dr=>dr.name).ToList()
            ////return roles.ToArray();
            //return dbRoles.Select(dr => dr.name).ToArray();
        }

        

        public List<User> GetUsersForArea(int area_id)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsersByCriteria(string text)
        {
	        return conn.Query<User>(
		        "SELECT userusers.* FROM userusers WHERE userusername LIKE @text OR userwelcome LIKE @text;",
		        new {text = "%" + text + "%"}).ToList();
        }

        public List<User> GetInspectors(int? location_id, bool includeAdminPermissions = false)
        {
			var where = @" WHERE admin_type = 5 and 
                        (  consolidated_port = @location_id OR
                           @location_id IS NULL OR 
                           EXISTS(SELECT location_id FROM useruser_location WHERE useruser_location.useruserid = userusers.useruserid AND location_id = @location_id)
                        ) AND status_flag <> 1";
			if(!includeAdminPermissions)
				return conn.Query<User>($"SELECT * FROM userusers {where}", new {location_id}).ToList();
			var users = new List<User>();
			conn.Query<User, Admin_permissions, User>($@"SELECT userusers.*, admin_permissions.* FROM 
					userusers LEFT OUTER JOIN admin_permissions ON userusers.useruserid = admin_permissions.userid
					{where}", 
					(u, ap) => { 
						var user = users.FirstOrDefault(x=>x.userid == u.userid);
						if(user == null)
						{
							user = u;
							user.AdminPermissions = new List<Admin_permissions>();
							users.Add(user);
						}
						if(ap != null)
							user.AdminPermissions.Add(ap);
						return user;
					}, new { location_id }, splitOn: "permission_id");
			return users;
        }

		
        public void Create(User o)
        {
            string insertsql = @"INSERT INTO userusers (userusername,userpassword,userwelcome,user_id,user_level,session,user_email,admin_type,consolidated_port,inspection_plan_admin,restrict_ip,
                                ip_address,ip_address1b,ip_address1c,ip_address2,mobilea,mobileb,email_pwd,skype,manager,user_initials,status_flag,restricted,qc_technical,login_restriction_from,login_restriction_to,login_restriction_days) 
                                VALUES(@userusername,@userpassword,@userwelcome,@user_id,@user_level,@session,@user_email,@admin_type,@consolidated_port,@inspection_plan_admin,
                                @restrict_ip,@ip_address,@ip_address1b,@ip_address1c,@ip_address2,@mobilea,@mobileb,@email_pwd,@skype,@manager,@user_initials,@status_flag,@restricted,@qc_technical,@login_restriction_from,@login_restriction_to,@login_restriction_days)";

	        conn.Execute(insertsql, o);
            o.userid = conn.ExecuteScalar<int>("SELECT useruserid FROM userusers WHERE useruserid = LAST_INSERT_ID()");
        }

        
        public void Update(User o)
        {
            string updatesql = @"UPDATE userusers SET userusername = @userusername,userpassword = @userpassword,userwelcome = @userwelcome,user_id = @user_id,user_level = @user_level,
                            session = @session,user_email = @user_email,admin_type = @admin_type,consolidated_port = @consolidated_port,inspection_plan_admin = @inspection_plan_admin,
                            restrict_ip = @restrict_ip,ip_address = @ip_address,ip_address1b = @ip_address1b,ip_address1c = @ip_address1c,ip_address2 = @ip_address2,mobilea = @mobilea,
                            mobileb = @mobileb,email_pwd = @email_pwd,skype = @skype,manager = @manager,user_initials = @user_initials,status_flag = @status_flag,restricted = @restricted,
                            qc_technical = @qc_technical,login_restriction_from = @login_restriction_from,login_restriction_to = @login_restriction_to, login_restriction_days = @login_restriction_days
                            WHERE useruserid = @useruserid";

	        conn.Execute(updatesql, o);
        }

        public void Delete(int useruserid)
        {
	        conn.Execute("DELETE FROM userusers WHERE useruserid = @useruserid", new {useruserid});
        }
    }

	public class UserMap : EntityMap<User>
	{
		public UserMap()
		{
			Map(u => u.userid).ToColumn("useruserid");
			Map(u => u.username).ToColumn("userusername");
			Map(u => u.company_id).ToColumn("user_id");
		}
	}

    

    
}
