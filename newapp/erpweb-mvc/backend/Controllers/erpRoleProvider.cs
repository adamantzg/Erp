using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using erp.Model;
using erp.Model.Dal.New;

namespace backend
{
    public class erpRoleProvider : RoleProvider
    {
        public erpRoleProvider()
        {
            
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            this.userDAL = GlobalConfiguration
                 .Configuration
                 .DependencyResolver
                 .GetService(typeof(IUserDAL)) as IUserDAL;

            base.Initialize(name, config);

        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        private string appName = "erp";
        private IUserDAL userDAL;

        public override string ApplicationName
        {
            get
            {
                return appName;
            }
            set
            {
                appName = value;
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            return userDAL.GetRolesForUser(username);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return userDAL.IsUserInRole(username, roleName);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}