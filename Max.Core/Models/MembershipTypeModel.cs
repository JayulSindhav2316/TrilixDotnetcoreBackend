using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipTypeModel
    {
        public MembershipTypeModel()
        {
            MembershipFees = new List<MembershipFeeModel>(); ;
        }

        public int MembershipTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PeriodName { get; set; }
        public string CategoryName { get; set; }
        public string PaymentFrequencyName { get; set; }
        public decimal TotalFee { get; set; }
        public decimal MembershipFee { get; set; }
        public int? Period { get; set; }
        public int? PaymentFrequency { get; set; }
        public int? Category { get; set; }
        public int? Status { get; set; }
        public string FeeDetailTag { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Units { get; set; }
        public  MembershipCategoryModel CategoryNavigation { get; set; }
        public List<MembershipFeeModel> MembershipFees { get; set; }
    }
}
