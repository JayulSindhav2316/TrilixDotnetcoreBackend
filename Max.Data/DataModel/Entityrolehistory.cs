using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Entityrolehistory
    {
        public int EntityRoleHistoryId { get; set; }
        public int? EntityId { get; set; }
        public int? ContactRoleId { get; set; }
        public int? CompanyId { get; set; }
        public string ActivityType { get; set; }
        public DateTime? ActivityDate { get; set; }
        public string Description { get; set; }
        public int? StaffUserId { get; set; }
        public int? Status { get; set; }
        public int? IsDeleted { get; set; }

        public virtual Company Company { get; set; }
        public virtual Contactrole ContactRole { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Staffuser StaffUser { get; set; }
    }
}
