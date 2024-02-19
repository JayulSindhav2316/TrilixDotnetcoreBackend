using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AnswerOptionModel
    {
        public int AnswerOptionId { get; set; }
        public int QuestionBankId { get; set; }
        public string Option { get; set; }
    }
}
