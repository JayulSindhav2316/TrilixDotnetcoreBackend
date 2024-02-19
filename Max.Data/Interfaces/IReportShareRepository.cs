using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IReportShareRepository : IRepository<Reportshare>
    {
        Task<IEnumerable<Reportshare>> GetAllReportSharesAsync();
        Task<Reportshare> GetReportShareByIdAsync(int id);
        Task<IEnumerable<Reportshare>> GetReportSharesByReportId(int id);
    }
}
