using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace backend.Models
{
    public class ChangeNoticeListModel
    {
        public List<Change_notice> ChangeNotices { get; set; }
    }

    public class ChangeNoticeEditModel
    {
        public Change_notice Notice { get; set; }
        public List<LookupItem> Clients { get; set; }
        public List<LookupItem> Factories { get; set; }
        public List<LookupItem> Categories { get; set; }
        public EditMode EditMode { get; set; }
    }
    public  enum NoticeType
    {
        Pending = 0,
        Resolved= 1,
        NA = 2

    }

    public class ChangeNoticeV2Model
    {
       
        public Change_notice Notice { get; set; }
        public EditMode EditMode { get; set; }
        public List<Company> Factories { get; set; }
        public List<Company> Clients { get; set; }
        public List<Category1> Categories { get; set; }
        public List<change_notice_reasons> ChangeNoticeReasons { get; set; }
        public List<Return_category> ChangeNoticeCategories { get; set; }
        public int? cprod_id { get; set; }
        public int? cn_id { get; set; }
        public int? factory_id { get; set; }
        public object Orders { get; set; }
        public Cust_products Product { get; set; }
        public NoticeType NoticeType { get; set; }        
        public List<LookupItem> Statuses { get; set; }

        public string ImageRootFolder { get; set; }
        
    }

    public class ChangeNoticeOrder
    {
        public int? cprod_id { get; set; }
        public int orderid { get; set; }
        public int? userid1 { get; set; }
    }
}
