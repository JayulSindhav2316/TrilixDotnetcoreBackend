using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class QuestionBankModel
    {
        public QuestionBankModel()
        {
            AnswerOptions = new List<AnswerOptionModel>();
        }
        public int QuestionBankId { get; set; }
        public string Question { get; set; }
        public int AnswerTypeLookUpId { get; set; }
        public int Status { get; set; }
        public int EventId { get; set; }
        public int SessionId { get; set; }
        public AnswerTypeLookUpModel AnswerTypeLookUp { get; set; }
        public List<AnswerOptionModel> AnswerOptions { get; set; }
    }
}
