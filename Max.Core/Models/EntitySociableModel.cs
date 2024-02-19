using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class EntitySociableModel
    {  

        public int EntityId { get; set; }
        public int PersonId { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string CasualName { get; set; }
        public string Suffix { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }

        /// <summary>
        // 1 => Primary Email
        // 2 => Primary Phone
        // 3 => Primary Address
        /// </summary>
        public int PreferredContact { get; set; }

        public string FormattedPhoneNumber
        {
            get
            {
                return Phone.FormatPhoneNumber();
            }
        }

    }
}
