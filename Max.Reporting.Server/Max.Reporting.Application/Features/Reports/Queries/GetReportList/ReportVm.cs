
using Max.Reporting.Domain.Entities;

namespace Max.Reporting.Application.Features.Reports.Queries.GetReportList
{
    public class ReportVm
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public ulong? Bookmark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual TrCategory Category { get; set; }
        public virtual TrTemplate Template { get; set; }

    }
}
