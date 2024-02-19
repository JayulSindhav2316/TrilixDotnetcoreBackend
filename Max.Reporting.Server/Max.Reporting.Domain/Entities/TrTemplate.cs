using Max.Reporting.Domain.Common;
using System.Text.Json.Serialization;

namespace Max.Reporting.Domain.Entities
{
    public partial class TrTemplate: EntityBase
    {
        public TrTemplate()
        {
            TrReports = new HashSet<TrReport>();
        }

        public int Id { get; set; }
        public string Name { get; set; }        
        public string Definition { get; set; }

        public virtual ICollection<TrReport> TrReports { get; set; }
    }
}
