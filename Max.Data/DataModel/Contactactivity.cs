using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Contactactivity
    {
        public Contactactivity()
        {
            Contactactivityinteractions = new HashSet<Contactactivityinteraction>();
        }

        public int ContactActivityId { get; set; }
        public int? EntityId { get; set; }
        public int? AccountId { get; set; }
        public DateTime? ActivityDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public int? InteractionType { get; set; }
        public int? ActivityConnection { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int? StaffUserId { get; set; }
        public int? Status { get; set; }
        public int? IsHistoricalDelete { get; set; }
        public int? IsDeleteforPerson { get; set; }
        public int? IsDeleteForAccount { get; set; }
        public int? IsDeleteForRole { get; set; }
        public int? ContactRoleId { get; set; }
        public int? IsDeleted { get; set; }
        public virtual Company Account { get; set; }
        public virtual Contactrole ContactRole { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Staffuser StaffUser { get; set; }
        public virtual ICollection<Contactactivityinteraction> Contactactivityinteractions { get; set; }
    }
}
