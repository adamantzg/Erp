using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace backend.Models
{
    public class MeetingListModel
    {
        public List<Meeting> Meetings { get; set; }
    }

    public class MeetingEditModel
    {
        public Meeting Meeting { get; set; }
        public EditMode EditMode { get; set; }
        public List<MeetingUser> Users { get; set; }
    }

    public class MeetingUser
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string userwelcome { get; set; }
    }
}
