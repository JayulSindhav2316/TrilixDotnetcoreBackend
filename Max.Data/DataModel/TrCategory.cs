using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class TrCategory
    {
        public TrCategory()
        {
            TrReports = new HashSet<TrReport>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TrReport> TrReports { get; set; }
    }
}
