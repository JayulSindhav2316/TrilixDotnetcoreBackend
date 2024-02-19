using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class MembershipInvoiceModel
    {
        public OrganizationModel Organization { get; set; }
        public PersonModel BillablePerson { get; set; }
        public InvoiceModel Invoice { get; set; }
        public InvoiceDetailModel InvoiceDetail { get; set; }

        public MembershipModel Membership { get; set; }
    }
}
