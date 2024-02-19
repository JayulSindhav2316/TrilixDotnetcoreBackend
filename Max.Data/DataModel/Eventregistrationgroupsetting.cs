using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventregistrationgroupsetting
    {
        public int EventRegistrationGroupSettingId { get; set; }
        public int? EventRegistrationSettingId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public DateTime? RegistrationDateTime { get; set; }

        public virtual Eventregistrationsetting EventRegistrationSetting { get; set; }
        public virtual Registrationgroup RegistrationGroup { get; set; }
    }
}
