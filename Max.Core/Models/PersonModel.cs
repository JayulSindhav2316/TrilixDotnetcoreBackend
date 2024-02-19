using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Max.Core.Models;

namespace Max.Core.Models
{
    public  class PersonModel
    {
        public PersonModel()
        {
            Emails = new List<EmailModel>();
            PersonTags = new List<PersonTagModel>();
            Phones = new List<PhoneModel>();
            Addresses = new List<AddressModel>();
            EntityRoles = new List<EntityRoleModel>();
        }
        public int PersonId { get; set; }
        public int EntityId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string CasualName { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? CompanyId { get; set; }
        public string TwitterName { get; set; }
        public string FacebookName { get; set; }
        public string InstagramName { get; set; }
        public string LinkedinName { get; set; }
        public string SkypeName { get; set; }
        public string Salutation { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Website { get; set; }
        public string WebLoginName { get; set; }
        public int? OrganizationId { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Designation { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string PrimaryEmail { get; set; }
        public string PrimaryPhone { get; set; }
        public string PrimaryAddress { get; set; }
        public int AddRelationToContact { get; set; }
        public int AddBillableToContact { get; set; }
        public string RelationshipType { get; set; }
        public int RelationshipId { get; set; }
        public CompanyModel Company { get; set; }
        public EntityModel Entity { get; set; }
        public List<EmailModel> Emails { get; set; }
        public List<PersonTagModel> PersonTags { get; set; }
        public List<PhoneModel> Phones { get; set; }
        public List<AddressModel> Addresses { get; set; }
        public List<EntityRoleModel> EntityRoles { get; set; }
        public int? PreferredContact { get; set; }
        public string CompanyName { get; set; }
        public string MemberId { get; set; }
        public bool IsSociablemanager { get; set; } = false;
        public bool IsBillableManager { get; set; } = false;
        public int SocialCompanyId { get; set; }
        public string BirthDate
        {
            get
            {
                if (DateOfBirth != null)
                {
                    return DateOfBirth.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public int Age
        {
            get { if (DateOfBirth != null)
                {
                    if(DateOfBirth < DateTime.Now)
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

        public string FullName
        {
            get
            {
                if (!CasualName.IsNullOrEmpty())
                {
                    return FirstName + " (" + CasualName + ") " + LastName;
                }
                else
                    return FirstName + " " + LastName;
            }
        }
        public string FormattedPhoneNumber
        {
            get
            {
                return PrimaryPhone.FormatPhoneNumber();
            }
        }

        public string FormattedZip
        {
            get
            {
                return Zip.FormatZip();
            }
        }
    }
}
