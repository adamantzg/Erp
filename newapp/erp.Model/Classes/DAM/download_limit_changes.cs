using System;
using System.Collections.Generic;

namespace asaq2.Model
{
    public class download_limit_changes
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public int user_id { get; set; }
        public int? old_limit { get; set; }
        public int? new_limit { get; set; }
    }
}
