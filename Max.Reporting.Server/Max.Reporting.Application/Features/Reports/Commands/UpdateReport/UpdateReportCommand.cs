using MediatR;

namespace Max.Reporting.Application.Features.Reports.Commands.UpdateReport
{
    public class UpdateReportCommand:IRequest
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
