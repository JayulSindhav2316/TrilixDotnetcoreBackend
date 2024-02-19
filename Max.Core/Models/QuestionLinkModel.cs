using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class QuestionLinkModel
    {
        public QuestionLinkModel()
        {
            QuestionBank = new List<QuestionBankModel>();
        }

        public int QuestionLinkId { get; set; }
        public int? EventId { get; set; }
        public int? SessionId { get; set; }
        public int? QuestionBankId { get; set; }

        public List<QuestionBankModel> QuestionBank { get; set; }
    }
}
