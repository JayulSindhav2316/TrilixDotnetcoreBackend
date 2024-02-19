using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Registrationgroup
    {
        public Registrationgroup()
        {
            Eventregistrationgroupsettings = new HashSet<Eventregistrationgroupsetting>();
            Linkeventgroups = new HashSet<Linkeventgroup>();
            Linkregistrationgroupfees = new HashSet<Linkregistrationgroupfee>();
            Registrationgroupmembershiplinks = new HashSet<Registrationgroupmembershiplink>();
            Sessionregistrationgrouppricings = new HashSet<Sessionregistrationgrouppricing>();
        }

        public int RegistrationGroupId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Eventregistrationgroupsetting> Eventregistrationgroupsettings { get; set; }
        public virtual ICollection<Linkeventgroup> Linkeventgroups { get; set; }
        public virtual ICollection<Linkregistrationgroupfee> Linkregistrationgroupfees { get; set; }
        public virtual ICollection<Registrationgroupmembershiplink> Registrationgroupmembershiplinks { get; set; }
        public virtual ICollection<Sessionregistrationgrouppricing> Sessionregistrationgrouppricings { get; set; }
    }
}
