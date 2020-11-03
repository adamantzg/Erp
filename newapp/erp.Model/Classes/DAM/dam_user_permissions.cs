using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model
{
    [NotMapped]
    public class DAM_user_permissions
    {
        public int user_id { get; set; }
        public int function_id { get; set; }
    }
}
