using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace asaq2.Model
{
    [NotMapped]
    public class DAM_users
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public bool? isAdmin { get; set; }
        public bool? menutype { get; set; }
        public bool? isActivated { get; set; }
        public string company { get; set; }
        public DateTime? date_registered { get; set; }
        public DateTime? date_activated { get; set; }
        public bool? Contact_Optout { get; set; }
        public int? download_limit { get; set; }
        public int type { get; set; }
        public int? dealer_id;

        public int? RemainingDownloadLimit { get; set; }

        public List<DAM_user_permissions> Permissions { get; set; }

        public bool HasPermissions(int perm_id)
        {
            return Permissions != null && Permissions.Count(p => p.function_id == perm_id) > 0;
        }
    }
}
