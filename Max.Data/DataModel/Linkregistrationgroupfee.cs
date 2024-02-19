using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Linkregistrationgroupfee
    {
        public int LinkRegistrationGroupFeeId { get; set; }
        public int? RegistrationFeeTypeId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? LinkEventGroupId { get; set; }
        public DateTime? RegistrationGroupDateTime { get; set; }
        public DateTime? RegistrationGroupEndDateTime { get; set; }

        public virtual Linkeventgroup LinkEventGroup { get; set; }
        public virtual Registrationfeetype RegistrationFeeType { get; set; }
        public virtual Registrationgroup RegistrationGroup { get; set; }
    }
}
