using Max.Reporting.Domain.Entities;

namespace Max.Reporting.Application.Contracts.Persistence
{
    public interface IReportDefinitionRepository : IAsyncRepository<TrDefinition>
    {
    }
}
