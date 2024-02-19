using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class OrganizationModel
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Prefix { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string HeaderImage { get; set; }
        public string FooterImge { get; set; }
        public string Email { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string Createdy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string PrintMessage { get; set; }
        public string WebMessage { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string Code { get; set; }
        public string AccountName { get; set; }

        public AccountingSetUpModel AccountingSetUpModel;
    }
}
