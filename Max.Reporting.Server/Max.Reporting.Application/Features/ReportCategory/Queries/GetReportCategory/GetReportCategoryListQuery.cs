using Max.Reporting.Domain.Entities;
using MediatR;


namespace Max.Reporting.Application.Features.ReportCategory.Queries.GetReportCategory
{
    public class GetReportCategoryListQuery:IRequest<List<TrCategory>>
    {
    }
}
