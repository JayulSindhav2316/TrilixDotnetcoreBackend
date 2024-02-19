using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventregisterquestion
    {
        public int EventRegisterQuestionId { get; set; }
        public int EventRegisterId { get; set; }
        public int? SessionId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public int? AnswerOptionId { get; set; }
        public int? EventId { get; set; }

        public virtual Answeroption AnswerOption { get; set; }
        public virtual Event Event { get; set; }
        public virtual Eventregister EventRegister { get; set; }
        public virtual Questionbank Question { get; set; }
        public virtual Session Session { get; set; }
    }
}
