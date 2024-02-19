using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipFeeModel
    {
        public int FeeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal FeeAmount { get; set; }
        public int? GlAccountId { get; set; }
        public int? BillingFrequency { get; set; }
        public int? IsMandatory { get; set; }
        public int Status { get; set; }
        public string GlAccountCode { get; set; }
        public int? MembershipTypeId { get; set; }
        public bool IsRequired
        {
            get
            {
                if (IsMandatory == 0) return false;
                return true;
            }
            set
            {
                IsMandatory = 0;

                if (value)
                {
                    IsMandatory = 1;
                }
            }
        }

    }
}
