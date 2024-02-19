using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Core.Models
{
    public  class PhoneModel
    {
        public int PhoneId { get; set; }
        public string PhoneType { get; set; }
        public string PhoneNumber { get; set; }
        public int PersonId { get; set; }
        public int CompanyId { get; set; }  
        [JsonIgnore]
        public int IsPrimary { get; set; }
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
