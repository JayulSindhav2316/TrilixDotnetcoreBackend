using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AddressModel
    {
        public AddressModel()
        {
            AddressId = 0;
            AddressType = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            Address3 = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Zip = string.Empty;
            Country = string.Empty;
            CountryCode = string.Empty;
            StateCode = string.Empty;
        }
        public int AddressId { get; set; }
        public string AddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
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
       
        //TDOD: AKS: Needs to be improved.
        public string StreetAddress   
        {
            get {

                StringBuilder sb = new StringBuilder();
               if(!Address1.IsNullOrEmpty())
                {
                    sb.Append(Address1);
                }
                if(!Address2.IsNullOrEmpty())
                {
                    sb.Append(", " + Address2.Trim());
                }
                if (!Address3.IsNullOrEmpty())
                {
                    sb.Append(", " + Address3.Trim());
                }
                return sb.ToString();
            }

            set { 
                    var items = value.Split(',');
                    for( var i=0; i < items.Length; i++)
                    {
                        if (i == 0) Address1 = items[0];
                        if (i == 1) Address2 = items[1];
                        if (i >= 2) Address3 += items[i];
                    }
                   
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
