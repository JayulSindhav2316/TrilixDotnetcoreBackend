using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Sessionregistrationgrouppricing
    {
        public int SessionRegistrationGroupPricingId { get; set; }
        public int? SessionId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? RegistrationFeeTypeId { get; set; }
        public decimal? Price { get; set; }

        public virtual Registrationfeetype RegistrationFeeType { get; set; }
        public virtual Registrationgroup RegistrationGroup { get; set; }
        public virtual Session Session { get; set; }
    }
}
