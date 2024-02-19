using Max.Reporting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Reporting.Application.Contracts.Persistence
{
    public interface IReportCategoryRepository : IAsyncRepository<TrCategory>
    {
    }
}
