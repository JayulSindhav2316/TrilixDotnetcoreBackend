using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Registrationfeetype
    {
        public Registrationfeetype()
        {
            Linkeventfeetypes = new HashSet<Linkeventfeetype>();
            Linkregistrationgroupfees = new HashSet<Linkregistrationgroupfee>();
            Sessionregistrationgrouppricings = new HashSet<Sessionregistrationgrouppricing>();
        }

        public int RegistrationFeeTypeId { get; set; }
        public string RegistrationFeeTypeName { get; set; }

        public virtual ICollection<Linkeventfeetype> Linkeventfeetypes { get; set; }
        public virtual ICollection<Linkregistrationgroupfee> Linkregistrationgroupfees { get; set; }
        public virtual ICollection<Sessionregistrationgrouppricing> Sessionregistrationgrouppricings { get; set; }
    }
}
