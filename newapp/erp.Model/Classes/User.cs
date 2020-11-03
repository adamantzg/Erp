using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace erp.Model
{
    public enum UserRole
    {
        Distributor,
        MasterDistributor,  //45,55 CW
        HeadDistributor,     //42 , can create distributors
        Manufacturer,
        ExternalUser,
        Administrator,
        Inspector,
        EBManagement,
        FactoryController,
        ClientController,
        AccountUser,
        QualityAssurance,
        UsUser,
        FactoryUser,
		AccountsCustomer,
		Empty
    }

    
    public class User
    {
        
        [NotMapped]
        public ExtensionDataObject ExtensionData {get;set; }
        
        
        public const int adminType_Qc = 5;
        public const int adminType_PowerUser = 8;

        
        public int userid { get; set; }
        
        public string username { get; set; }
        
        public string userpassword { get; set; }
        
        public string userwelcome { get; set; }
        
        public int company_id { get; set; }
        
        public int? user_level { get; set; }
        
        public int? session { get; set; }
        
        public string user_email { get; set; }
        
        public int? admin_type { get; set; }
        
        public int? consolidated_port { get; set; }
        
        public int? inspection_plan_admin { get; set; }
        
        public int? restrict_ip { get; set; }
        
        public string ip_address { get; set; }
        
        public string ip_address1b { get; set; }
        
        public string ip_address1c { get; set; }
        
        public string ip_address2 { get; set; }
        
        public string mobilea { get; set; }
        
        public string mobileb { get; set; }
        
        public string email_pwd { get; set; }
        
        public string skype { get; set; }
        
        public int? manager { get; set; }
        
        public string user_initials { get; set; }
        
        public int? status_flag { get; set; }
        
        public int? restricted { get; set; }
        
        public int? qc_technical { get; set; }
        
        public bool? after_sales { get; set; }
        
        public bool? newdesign { get; set; }
        
        public TimeSpan? login_restriction_from { get; set; }
        
        public TimeSpan? login_restriction_to { get; set; }
        
        public string login_restriction_days { get; set; }

        
        public virtual Company Company { get; set; }
        [NotMapped]
        
        public virtual List<Permission> Permissions { get; set; }
        
        public virtual List<Role> Roles { get; set; }
        
        public virtual List<Admin_permissions> AdminPermissions { get; set; }

        public virtual List<Returns> Returns { get; set; }
        public virtual List<Returns_comments> ReturnsComments { get; set; }
        
        public virtual List<Dealer_external_comment> ExternalDealerComments { get; set; }
        
        public virtual List<Location> Locations { get; set; }
        
        public virtual List<Admin_pages> AdminPages { get; set; }
        
        public virtual List<Admin_pages_new> AdminPagesNew { get; set; }
        
        public virtual List<Returns> QCAssignedReturns { get; set; }
        [DataMember]
        public virtual List<Returns_qcusers> AssignedQCusers { get; set; }
        [DataMember]
        public virtual List<Returns_qcusers> ReturnsQCUsers{ get; set; }

        [DataMember]
        public virtual List<manual_edit_history> ManualEditHistoryRecords { get; set; }

        public virtual List<UserGroup> Groups { get; set; }
        
        public bool HasPermission(int perm_id)
        {
            return Permissions != null && Permissions.Count(p => p.id == perm_id) > 0;
        }
        
        public bool IsInRole(int role_id)
        {
            return Roles != null && Roles.Count(r => r.id == role_id) > 0;
        }

        public bool IsUserIT()
        {
            return Roles != null && Roles.Count(r => r.id == Role.ITAdmin || r.id == Role.ITUser) > 0;
        }

        public bool IsUserFC()
        {
            return Roles != null && Roles.Count(r => r.id == Role.FCOfficeAdmin || r.id == Role.FCOfficeUser) > 0;
        }
        //public List<Us_call_log> CallLogs { get; set; }
    }

    public class Location
    {
        public int id { get; set; }
        public string Name { get; set; }
		public bool? show_on_plan { get; set; }
		public bool? show_on_omexport { get; set; }
    }
}
