
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IOpportunityReportRepository : IRepository<Opportunityreport>
    {
        Task<IEnumerable<Opportunityreport>> GetAllOpportunityReportsAsync();
    }

}
