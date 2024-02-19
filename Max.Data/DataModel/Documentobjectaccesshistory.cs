using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Documentobjectaccesshistory
    {
        public int AccessId { get; set; }
        public int? DocumentObjectId { get; set; }
        public int? EntityId { get; set; }
        public int? StaffUserId { get; set; }
        public int? AccessType { get; set; }
        public DateTime? AccessDate { get; set; }

        public virtual Documentobject DocumentObject { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Staffuser StaffUser { get; set; }
    }
}
