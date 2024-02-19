using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using Max.Reporting.Infrastructure.Persistence;

namespace Max.Reporting.Infrastructure.Repositories
{
    public class ReportTemplateRepository:RepositoryBase<TrTemplate>,IReportTemplateRepository
    {
        public ReportTemplateRepository(ReportContext dbContext) : base(dbContext)
        {
        }
    }
}
