using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CompanyModel
    {
        public CompanyModel()
        {
            Addresses = new List<AddressModel>();
            Emails = new List<EmailModel>();
            Phones = new List<PhoneModel>();
            SociableManager = new List<int>();
        }

        public int CompanyId { get; set; }
        public int EntityId { get; set; }
        public int OrganizationId { get; set; }
        public string CompanyName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Designation { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int BillableContactId { get; set; }
        public string MailingAddress { get; set; }
        public int SociablePrimaryContact { get; set; }
        public int SociableBillableContact { get; set; }
        public string MemberId { get; set; }
        public List<int> SociableManager { get; set; }
        public PersonModel BillablePerson { get; set; }
        public EntityModel Entity { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public List<EmailModel> Emails { get; set; }
        public List<PhoneModel> Phones { get; set; }
    }
}
