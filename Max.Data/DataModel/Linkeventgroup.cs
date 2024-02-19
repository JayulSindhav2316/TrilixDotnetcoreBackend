using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Linkeventgroup
    {
        public Linkeventgroup()
        {
            Linkregistrationgroupfees = new HashSet<Linkregistrationgroupfee>();
        }

        public int LinkEventGroupId { get; set; }
        public int? EventId { get; set; }
        public int? RegistrationGroupId { get; set; }
        public int? EnableOnlineRegistration { get; set; }

        public virtual Event Event { get; set; }
        public virtual Registrationgroup RegistrationGroup { get; set; }
        public virtual ICollection<Linkregistrationgroupfee> Linkregistrationgroupfees { get; set; }
    }
}
