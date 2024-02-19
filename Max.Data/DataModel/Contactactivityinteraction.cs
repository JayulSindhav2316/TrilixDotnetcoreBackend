using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Contactactivityinteraction
    {
        public int ContactActivityInteractionId { get; set; }
        public int? ContactActivityId { get; set; }
        public int? InteractionAccountId { get; set; }
        public int? InteractionEntityId { get; set; }
        public int? InteractionRoleId { get; set; }
        public int? IsDeleted { get; set; } 

        public virtual Contactactivity ContactActivity { get; set; }
        public virtual Company InteractionAccount { get; set; }
        public virtual Entity InteractionEntity { get; set; }
    }
}
