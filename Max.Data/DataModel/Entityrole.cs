using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Entityrole
    {
        public int EntityRoleId { get; set; }
        public int? EntityId { get; set; }
        public int? ContactRoleId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public int? CompanyId { get; set; }
        public int? IsDeleted { get; set; }

        public virtual Company Company { get; set; }
        public virtual Contactrole ContactRole { get; set; }
        public virtual Entity Entity { get; set; }
    }
}
