using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Glaccount
    {
        public Glaccount()
        {
            Membershipfees = new HashSet<Membershipfee>();
        }

        public int GlAccountId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int AccountTypeId { get; set; }
        public string DetailType { get; set; }
        public int? Status { get; set; }
        public int CostCenterId { get; set; }

        public virtual Glaccounttype AccountType { get; set; }
        public virtual Department CostCenter { get; set; }
        public virtual ICollection<Membershipfee> Membershipfees { get; set; }
    }
}
