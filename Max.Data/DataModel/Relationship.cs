using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Relationship
    {
        public Relationship()
        {
            Relations = new HashSet<Relation>();
        }

        public int RelationshipId { get; set; }
        public string Relation { get; set; }
        public string ReverseRelationMale { get; set; }
        public string ReverseRelationFemale { get; set; }

        public virtual ICollection<Relation> Relations { get; set; }
    }
}
