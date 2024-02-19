using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Questionbank
    {
        public Questionbank()
        {
            Answeroptions = new HashSet<Answeroption>();
            Answertoquestions = new HashSet<Answertoquestion>();
            Eventregisterquestions = new HashSet<Eventregisterquestion>();
            Questionlinks = new HashSet<Questionlink>();
        }

        public int QuestionBankId { get; set; }
        public string Question { get; set; }
        public int? AnswerTypeLookUpId { get; set; }
        public int? Status { get; set; }

        public virtual Answertypelookup AnswerTypeLookUp { get; set; }
        public virtual ICollection<Answeroption> Answeroptions { get; set; }
        public virtual ICollection<Answertoquestion> Answertoquestions { get; set; }
        public virtual ICollection<Eventregisterquestion> Eventregisterquestions { get; set; }
        public virtual ICollection<Questionlink> Questionlinks { get; set; }
    }
}
