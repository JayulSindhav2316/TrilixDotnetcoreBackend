using Max.Reporting.Domain.Entities;

namespace Max.Reporting.Application.Contracts.Persistence
{
    public interface IReportTemplateRepository : IAsyncRepository<TrTemplate>
    {
    }
}
