using System;
using System.Collections.Generic;

namespace Max.Data.DataModel
{
    public partial class Boardgroup
    {
        public int BoardGroupId { get; set; }
        public string BoardGroupName { get; set; }
        public string BoardGroupDescription { get; set; }
        public int? PreferredNumbers { get; set; }
        public int? ApplyTerm { get; set; }
        public DateTime? TerrmStartDate { get; set; }
        public DateTime? TermEndDate { get; set; }
        public int? OrganizationId { get; set; }
        public int? Status { get; set; }
    }
}
