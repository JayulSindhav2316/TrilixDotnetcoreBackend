using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using Max.Reporting.Infrastructure.Persistence;

namespace Max.Reporting.Infrastructure.Repositories
{
    public class ReportDefinitionRepository: RepositoryBase<TrDefinition>, IReportDefinitionRepository
    {
        public ReportDefinitionRepository(ReportContext dbContext) : base(dbContext)
        {
        }
    }
}
