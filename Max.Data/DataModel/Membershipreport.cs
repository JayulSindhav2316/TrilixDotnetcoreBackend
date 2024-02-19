using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Membershipreport
    {

        public int MembershipReportId { get; set; }
        public string Categories { get; set; }
        public string MembershipTypes { get; set; }
        public string Status { get; set; }
        public int? ReportId { get; set; }

        public virtual Report Report { get; set; }
    }
}
