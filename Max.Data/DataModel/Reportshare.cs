using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Reportshare
    {
        public int ReportShareId { get; set; }
        public int? UserId { get; set; }
        public int? SharedToUserId { get; set; }
        public int? ReportId { get; set; }

        public virtual Report Report { get; set; }


    }
}
