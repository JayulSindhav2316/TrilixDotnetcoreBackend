using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Core.Models
{
    public class EmailModel
    {
        public int EmailId { get; set; }
        public string EmailAddressType { get; set; }
        public string EmailAddress { get; set; }
        public int PersonId { get; set; }
        public int CompanyId { get; set; }
        [JsonIgnore]
        public int IsPrimary { get; set; }
        //Hack to handle PrimeNG controls
        public bool isPrimary
        {
            get
            {
                return IsPrimary == Constants.TRUE ? true : false;
            }
            set
            {
                if (value)
                    IsPrimary = 1;
                else
                    IsPrimary = 0;
            }
        }
    }
}
