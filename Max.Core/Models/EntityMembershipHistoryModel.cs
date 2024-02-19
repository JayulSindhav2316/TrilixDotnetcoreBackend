using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EntityMembershipHistoryModel
    {
        public EntityMembershipHistoryModel()
        {
            AdditionalMembers = new List<AdditionalMemberModel>();
        }
        public int EntityId { get; set; }
        public int MembershipId { get; set; }
        public string Code { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NextBillDate { get; set; }
        public string CurrentStatus { get; set; }
        public string MemberName { get; set; }
        public string BillableEntityName { get; set; }
        public EntityModel BillableEntity { get; set; }

        public List<AdditionalMemberModel> AdditionalMembers{get;set;}
        public int MaxUnits { get; set; }
    }

    public class AdditionalMemberModel
    {
        public int EntityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }

        public int Age
        {
            get
            {
                if (!DateOfBirth.IsNullOrEmpty())
                {
                    if (DateTime.Parse(DateOfBirth) < DateTime.Now)
                    {
                        return new DateTime(DateTime.Now.Subtract(Convert.ToDateTime(DateOfBirth)).Ticks).Year - 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
