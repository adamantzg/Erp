using erp.DAL.EF.Repositories;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace erp.DAL.EF
{
    public class FormSubmissionRepository : GenericRepository<Form_submission>
    {
        public FormSubmissionRepository(DbContext context) : base(context)
        {
        }

        public override void Insert(Form_submission entity)
        {
            foreach(var a in entity.Answers)
            {
                if (a.QuestionAnswers != null)
                    foreach (var qa in a.QuestionAnswers)
                    {
                        if(qa.Choices != null)
                        {
                            var choices = new List<Form_choice>();
                            foreach(var c in qa.Choices)
                            {
                                var choice = context.Set<Form_choice>().Local.FirstOrDefault(ch => ch.id == c.id);
                                if(choice == null)
                                {
                                    choice = c;
                                    context.Set<Form_choice>().Attach(choice);
                                }
                                choices.Add(choice);                                    
                            }
                            qa.Choices = choices;
                        }
                        
                    }                        
            }

            base.Insert(entity);
        }
    }
}
