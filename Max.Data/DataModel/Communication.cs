using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Communication
    {
        public int CommunicationId { get; set; }
        public DateTime? Date { get; set; }
        public string From { get; set; }
        public int? EntityId { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Notes { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int? Scheduled { get; set; }
        public int? StaffUserId { get; set; }

        public virtual Entity Entity { get; set; }
        public virtual Staffuser StaffUser { get; set; }
    }
}
