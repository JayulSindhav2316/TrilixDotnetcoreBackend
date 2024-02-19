using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Questionlink
    {
        public int QuestionLinkId { get; set; }
        public int? EventId { get; set; }
        public int? SessionId { get; set; }
        public int? QuestionBankId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Questionbank QuestionBank { get; set; }
        public virtual Session Session { get; set; }
    }
}
