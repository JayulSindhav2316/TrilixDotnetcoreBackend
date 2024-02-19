using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Answeroption
    {
        public Answeroption()
        {
            Eventregisterquestions = new HashSet<Eventregisterquestion>();
        }

        public int AnswerOptionId { get; set; }
        public int? QuestionBankId { get; set; }
        public string Option { get; set; }

        public virtual Questionbank QuestionBank { get; set; }
        public virtual ICollection<Eventregisterquestion> Eventregisterquestions { get; set; }
    }
}
