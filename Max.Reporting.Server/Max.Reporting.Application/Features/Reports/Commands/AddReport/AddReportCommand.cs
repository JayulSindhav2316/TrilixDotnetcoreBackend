using MediatR;

namespace Max.Reporting.Application.Features.Reports.Commands.AddReport
{
    public class AddReportCommand:IRequest<int>
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int TemplateId { get; set; }
        public int UserId { get; set; }
        public ulong? Bookmark { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
    }
}
