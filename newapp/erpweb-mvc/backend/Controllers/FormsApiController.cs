using erp.DAL.EF.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.Model;
using backend.ApiServices;

namespace backend.Controllers
{
    public class FormsApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAccountService accountService;

        public FormsApiController(IUnitOfWork unitOfWork, IAccountService accountService)
        {
            this.unitOfWork = unitOfWork;
            this.accountService = accountService;
        }

        public FormsApiController()
        {
            
        }

        [Route("api/forms/getAll")]
        [HttpGet]
        public object GetAll()
        {
            return unitOfWork.FormRepository.Get().Select(f=>GetUIObject(f)).ToList();
        }
        
        [Route("api/forms/getForm")]
        [HttpGet]
        public object GetForm(int id)
        {
            
            return new {
                types = unitOfWork.FormQuestionTypeRepository.Get().ToList(),
                renderMethods = unitOfWork.FormQuestionRenderMethodRepository.Get().ToList(),
                form = GetUIObject(unitOfWork.FormRepository.Get(f => f.id == id, includeProperties: "Sections.Section.Questions.Question.ChoiceGroup").FirstOrDefault())
            };
                
        }

        [Route("api/forms/submitForm")]
        [HttpPost]
        public void SubmitForm(Form_submission s)
        {
            if(s != null)
            {
                s.user_id = accountService.GetCurrentUser()?.userid;
                s.dateCreated = DateTime.Now;
                unitOfWork.FormSubmissionRepository.Insert(s);
                unitOfWork.Save();                
            }                            
        }

        [Route("api/forms/submissions")]
        [HttpGet]
        public object GetSubmissions(int form_id)
        {
            return unitOfWork.FormSubmissionRepository.Get(s => s.form_id == form_id, includeProperties: "User").Select(GetSubmissionUIObject);
        }

        [Route("api/forms/submission")]
        [HttpGet]
        public object GetSubmission(int id)
        {
            
            //return unitOfWork.FormSubmissionRepository.Get(s => s.id == id, includeProperties: "Answers.QuestionAnswers").Select(GetSubmissionUIObject);
            var submission = unitOfWork.FormSubmissionRepository.Get(s => s.id == id, includeProperties: "Form,Answers.QuestionAnswers").FirstOrDefault();
            if(submission != null)
            {
                var answers = submission.Answers.ToDictionary(a => a.formsection_question_id);
                return GetUIObject(submission.Form, answers);
            }
            return null;
        }
                

        private object GetUIObject(Form form, Dictionary<int?, Form_submission_answer> answers = null)
        {
            if (form != null)
                return new
                {
                    form.id, 
                    form.title,
                    sections = form.Sections?.OrderBy(s=>s.sequence).Select(s=> new {
                        s.Section?.id,
                        s.Section?.title,
                        elements = s.Section?.Questions?.OrderBy(q=>q.sequence)?.Select(q=> new {
                            q.id,
                            q.question_group_id,
                            q.question_id,
                            section_id = s.Section?.id,
                            question = q.Question != null ?  GetQuestionUIObject(q.Question) : null,
                            questionGroup = q.QuestionGroup != null ? new {
                                q.QuestionGroup.id,
                                q.QuestionGroup.name,
                                q.QuestionGroup.description,                                
                                questions =  ProcessQuestionGroupQuestions(q.QuestionGroup?.Questions)
                            } : null,
                            answer = answers != null && answers.ContainsKey(q.id) ? new
                            {
                                answers[q.id].id,
                                questionAnswers = answers[q.id]?.QuestionAnswers?.Select(qa => new {
                                    qa.comment_text,
                                    qa.dateValue,
                                    qa.intValue,
                                    qa.textValue,
                                    question = qa.Question != null ? GetQuestionUIObject(qa.Question) : null,
                                    choices = qa?.Choices?.Select(c => new {
                                        c.id,
                                        c.name
                                    })
                                })
                            } : null
                        })
                    })
                };
            return null;
        }

        private object ProcessQuestionGroupQuestions(List<Form_questiongroup_question> questions)
        {
            var result = new List<object>();
            
            foreach (var item in questions.OrderBy(qu => qu.sequence))
            {
                for (int i = 0; i < (item.numRepeat ?? 1); i++)
                {
                    result.Add(GetQuestionUIObject(item.Question));
                }
            }
            return result;
        }

        private object GetQuestionUIObject(Form_question q)
        {
            return new
            {
                q.id,
                q.has_comment,
                q.question_text,
                q.description,
                q.question_type,
                q.render_id,
                q.color,
                q.comment_label,
                q.choice_group_id,
                q.label_editable,
                choiceGroup = q.ChoiceGroup != null ? new
                {
                    q.ChoiceGroup.id,
                    q.ChoiceGroup.name,
                    choices = q.ChoiceGroup?.Choices?.Select(c => new
                    {
                        c.id,
                        c.name
                    })
                } : null
            };
        }

        private object GetSubmissionUIObject(Form_submission s)
        {
            return new
            {
                s.id,
                s.form_id,
                s.dateCreated,
                s.user_id,
                form = new {s.Form?.id, s.Form?.title},
                User = s.User != null ? new { s.User.userid, s.User.userwelcome } : null,
                answers = s?.Answers?.Select(a => new {
                    a.id,
                    questionAnswers = a?.QuestionAnswers?.Select(qa => new {
                        qa.comment_text,
                        qa.dateValue,
                        qa.intValue,
                        qa.textValue,
                        choices = qa?.Choices?.Select(c=> new {
                            c.id,
                            c.name
                        })
                    })                    
                })
            };
        }
    }
}