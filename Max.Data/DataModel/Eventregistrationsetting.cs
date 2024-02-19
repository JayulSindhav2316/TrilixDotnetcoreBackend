using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Eventregistrationsetting
    {
        public Eventregistrationsetting()
        {
            Eventregistrationgroupsettings = new HashSet<Eventregistrationgroupsetting>();
        }

        public int EventRegistrationSettingId { get; set; }
        public int? EventId { get; set; }
        public int? AllowOnlineRegistration { get; set; }
        public int? AllowNonMember { get; set; }
        public int? AllowWaitlist { get; set; }
        public int? AllowMultipleRegistration { get; set; }

        public virtual Event Event { get; set; }
        public virtual ICollection<Eventregistrationgroupsetting> Eventregistrationgroupsettings { get; set; }
    }
}
