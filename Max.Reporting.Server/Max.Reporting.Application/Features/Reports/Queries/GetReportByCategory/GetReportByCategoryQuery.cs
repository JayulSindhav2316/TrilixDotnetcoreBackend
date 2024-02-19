using Max.Reporting.Domain.Entities;
using MediatR;

namespace Max.Reporting.Application.Features.Reports.Queries.GetReportByCategory
{
    public class GetReportByCategoryQuery: IRequest<List<TrReport>>
    {
        public int CategoryId { get; set; }
    }
}
