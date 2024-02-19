using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Glaccount
    {
        public Glaccount()
        {
            AccountingsetupOffLinePaymentGlAccountNavigations = new HashSet<Accountingsetup>();
            AccountingsetupOnlineCreditGlAccountNavigations = new HashSet<Accountingsetup>();
            AccountingsetupProcessingFeeGlAccountNavigations = new HashSet<Accountingsetup>();
            AccountingsetupSalesReturnGlAccountNavigations = new HashSet<Accountingsetup>();
            AccountingsetupWriteOffGlAccountNavigations = new HashSet<Accountingsetup>();
            Items = new HashSet<Item>();
            Membershipfees = new HashSet<Membershipfee>();
            Promocodes = new HashSet<Promocode>();
            Sessions = new HashSet<Session>();
        }

        public int GlAccountId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int? AccountTypeId { get; set; }
        public string DetailType { get; set; }
        public int? Status { get; set; }
        public int? CostCenterId { get; set; }

        public virtual Glaccounttype AccountType { get; set; }
        public virtual Department CostCenter { get; set; }
        public virtual ICollection<Accountingsetup> AccountingsetupOffLinePaymentGlAccountNavigations { get; set; }
        public virtual ICollection<Accountingsetup> AccountingsetupOnlineCreditGlAccountNavigations { get; set; }
        public virtual ICollection<Accountingsetup> AccountingsetupProcessingFeeGlAccountNavigations { get; set; }
        public virtual ICollection<Accountingsetup> AccountingsetupSalesReturnGlAccountNavigations { get; set; }
        public virtual ICollection<Accountingsetup> AccountingsetupWriteOffGlAccountNavigations { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<Membershipfee> Membershipfees { get; set; }
        public virtual ICollection<Promocode> Promocodes { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
