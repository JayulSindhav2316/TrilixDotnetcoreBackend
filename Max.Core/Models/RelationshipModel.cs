using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RelationshipModel
    {
        public RelationshipModel()
        {
            Relations = new List<RelationModel>();
        }
        public int RelationshipId { get; set; }
        public string Relation { get; set; }
        public string ReverseRelationMale { get; set; }
        public string ReverseRelationFemale { get; set; }
        public string RelatedGender { get; set; }

        public  List<RelationModel> Relations { get; set; }
    }
}
