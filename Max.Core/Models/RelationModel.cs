using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RelationModel
    {
        public int RelationId { get; set; }
        public int? EntityId { get; set; }
        public int? RelatedEntityId { get; set; }
        public int RelationshipId { get; set; }
        public string RelationshipType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public string Notes { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public bool PrimaryChecked { get; set; }
        public bool BillableChecked { get; set; }
        public bool SocialChecked { get; set; }
        public RelationshipModel Relationship { get; set; }
        public EntityModel Entity { get; set; }
        public EntityModel RelatedEntity { get; set; }
        public bool IsPeople { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string Age
        {
            get
            {
                return DateOfBirth.CalculateAge();
            }
        }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }


    }
}
