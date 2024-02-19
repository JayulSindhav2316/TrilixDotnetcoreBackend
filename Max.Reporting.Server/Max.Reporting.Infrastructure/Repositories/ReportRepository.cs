using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using Max.Reporting.Infrastructure.Persistence;

namespace Max.Reporting.Infrastructure.Repositories
{
    public class ReportRepository : RepositoryBase<TrReport>, IReportRepository
    {
        public ReportRepository(ReportContext dbContext) : base(dbContext)
        {
        }
    }
}
