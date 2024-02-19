using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AccountContactModel
    {
        public AccountContactModel()
        {
            EntityRoles = new List<string>();
        }
        public int EntityId { get; set; }
        public string Roles { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PrimaryEmail { get; set; } = string.Empty;
        public string PrimaryPhone { get; set; } = string.Empty;
        public int AddRoleEnabled { get; set; } = 0;
        public List<String> EntityRoles { get; set; }  
        public string EffectiveDate
        {
            get
            {
                var defaultEffectiveDate = DateTime.Now;
                return $"{defaultEffectiveDate.Month.ToString()}/{defaultEffectiveDate.Day.ToString()}/{defaultEffectiveDate.Year.ToString()}";
                
            }
        }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
