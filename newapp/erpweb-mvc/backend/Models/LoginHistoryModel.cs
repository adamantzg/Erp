using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class LoginHistoryModel
    {
        public List<Company> Companies { get; set; }
        public List<login_history_detail_report> History { get; set; }
        public List<Login_history> HistorySimple { get; set; }
        public int company_id { get; set; }
        public bool useDateFrom { get; set; }
        public bool useDateTo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dateFrom { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dateTo { get; set; }
        public bool ShowPages { get; set; }
        public bool ShowUsers { get; set; }
        public bool ExpandUsers { get; set; }
        public List<login_history_page_count> HistoryPageCounts { get; set; }
        public int PagesToShow { get; set; }
        public List<DownloadLogTotal> DownloadLogTotals { get; set; }
        public List<Qc_comments> QcComments { get; set; }
        public bool ShowAllActiveUsers { get; set; }
        public List<User> AllActiveUsers { get; set; }
        public List<Login_history> AllActiveUsersLoginHistory { get; set; }
        public Dictionary<string, DateTime?> LastLoginEntries { get; set; }

        public LoginHistoryModel()
        {
            PagesToShow = 5;
        }
    }

    public class LoginHistoryInspectorsModel
    {
        public List<Login_history> HistorySimple { get; set; }
        public List<Inspections> Inspections { get; set; }
        public List<Qc_comments> QcComments { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dateFrom { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? dateTo { get; set; }
        public bool ShowUsers { get; set; }
        public bool ShowLinks { get; set; }
        //public List<int?> Locations { get; set; }
    }

}