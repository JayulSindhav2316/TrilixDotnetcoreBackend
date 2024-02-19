using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Relation
    {
        public int RelationId { get; set; }
        public int? PersonId { get; set; }
        public int? RelatedPersonId { get; set; }
        public string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string Notes { get; set; }

        public virtual Person Person { get; set; }
        public virtual Person RelatedPerson { get; set; }
    }
}
