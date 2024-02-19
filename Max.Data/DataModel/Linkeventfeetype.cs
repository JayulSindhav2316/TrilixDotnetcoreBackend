using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Linkeventfeetype
    {
        public int LinkEventFeeTypeId { get; set; }
        public int? RegistrationFeeTypeId { get; set; }
        public int? EventId { get; set; }

        public virtual Event Event { get; set; }
        public virtual Registrationfeetype RegistrationFeeType { get; set; }
    }
}
