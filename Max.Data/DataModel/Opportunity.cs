using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Opportunity
    {
        public int OpportunityId { get; set; }
        public int? PipelineId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EstimatedCloseDate { get; set; }
        public decimal? Potential { get; set; }
        public int? StaffUserId { get; set; }
        public int? AccountContactId { get; set; }
        public int? CompanyId { get; set; }
        public int? StageId { get; set; }
        public int? Probability { get; set; }
        public string Notes { get; set; }

        public virtual Entity AccountContact { get; set; }
        public virtual Company Company { get; set; }
        public virtual Opportunitypipeline Pipeline { get; set; }
        public virtual Staffuser StaffUser { get; set; }
    }
}