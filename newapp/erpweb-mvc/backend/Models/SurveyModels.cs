using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend.Models
{
    public class SurveyModel
    {
        public Survey_definition Definition { get; set; }
        public int? dealer_id { get; set; }
        public int? def_id { get; set; }
        public List<Survey_optionset> Optionsets { get; set; }

    }

    public class SurveyResultModel
    {
        public List<Survey_result_answer> Answers { get; set; }
        public Survey_definition Definition { get; set; }
        public List<Survey_optionset> Optionsets { get; set; }
        public bool ProcessDealers { get; set; }
        public List<Dealer> Dealers { get; set; }
        public List<Company> Distributors { get; set; }
    }

    public class QuestionModel
    {
        public int Ordinal { get; set; }
        public Survey_question Question { get; set; }
        public List<Survey_optionset> Optionsets { get; set; }
        public int def_id { get; set; }
    }

    public class ResultCommentsModel
    {
        public List<Survey_result_answer> Answers { get; set; }
        public List<Company> Distributors { get; set; }
    }
}