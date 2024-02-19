using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Company
    {
        public Company()
        {
            People = new HashSet<Person>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? AddressId { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryConatctPhone { get; set; }
        public string PrimaryContactEmail { get; set; }

        public virtual ICollection<Person> People { get; set; }
    }
}
