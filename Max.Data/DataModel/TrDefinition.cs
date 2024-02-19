using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class TrDefinition
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Definition { get; set; }

        public virtual TrReport Report { get; set; }
    }
}
