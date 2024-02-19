using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Answertypelookup
    {
        public Answertypelookup()
        {
            Questionbanks = new HashSet<Questionbank>();
        }

        public int AnswerTypeLookUpId { get; set; }
        public string AnswerType { get; set; }

        public virtual ICollection<Questionbank> Questionbanks { get; set; }
    }
}
