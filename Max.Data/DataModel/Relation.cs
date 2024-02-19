using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Relation
    {
        public int RelationId { get; set; }
        public int? EntityId { get; set; }
        public int? RelatedEntityId { get; set; }
        public int RelationshipId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string Notes { get; set; }
        public bool? Primary { get; set; }
        public bool? Billable { get; set; }
        public bool? Social { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Entity RelatedEntity { get; set; }
        public virtual Relationship Relationship { get; set; }
    }
}
