using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.Model
{
    public class form_question_answer
    {
        public int id { get; set; }
        public int? form_submission_answer_id { get; set; }
        public int? question_id { get; set; }
        public int? intValue { get; set; }
        public string textValue { get; set; }
        public DateTime? dateValue { get; set; }
        public string comment_text { get; set; }

        public virtual Form_question Question { get; set; }
        public virtual List<Form_choice> Choices { get; set; }

    }
}
