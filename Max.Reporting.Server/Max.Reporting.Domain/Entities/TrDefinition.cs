using Max.Reporting.Domain.Common;
using System;
using System.Collections.Generic;

namespace Max.Reporting.Domain.Entities
{
    public partial class TrDefinition: EntityBase
    {        
        public int ReportId { get; set; }
        public string Definition { get; set; }

        public virtual TrReport Report { get; set; }
    }
}
