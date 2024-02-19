using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Domain.Entities;
using Max.Reporting.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Infrastructure.Repositories
{
    public class ReportCategoryRepository : RepositoryBase<TrCategory>, IReportCategoryRepository
    {
        public ReportCategoryRepository(ReportContext dbContext) : base(dbContext)
        {
        }
    }
}
