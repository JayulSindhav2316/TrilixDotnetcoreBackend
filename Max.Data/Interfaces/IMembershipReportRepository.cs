
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMembershipReportRepository : IRepository<Membershipreport>
    {
        Task<IEnumerable<Membershipreport>> GetAllMembershipReportsAsync();
    }

}
