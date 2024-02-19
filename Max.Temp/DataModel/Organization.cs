using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Organization
    {
        public Organization()
        {
            Departments = new HashSet<Department>();
            People = new HashSet<Person>();
        }

        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public byte[] Logo { get; set; }
        public string Website { get; set; }
        public byte[] HeaderImage { get; set; }
        public string FooterImge { get; set; }
        public string Email { get; set; }
        public string PrimaryContacName { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string Createdy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Person> People { get; set; }
    }
}
