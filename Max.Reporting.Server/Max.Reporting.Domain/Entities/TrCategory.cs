using Max.Reporting.Domain.Common;

namespace Max.Reporting.Domain.Entities
{    
    public partial class TrCategory: EntityBase
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
