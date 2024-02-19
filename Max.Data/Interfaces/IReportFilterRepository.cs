using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IReportFilterRepository: IRepository<Reportfilter>
    {
        Task<IEnumerable<Reportfilter>> GetAllReportFilterssAsync();
        Task<Reportfilter> GetReportFilterByIdAsync(int id);
        Task<IEnumerable<Reportfilter>> GetReportFiltersByReportId(int id);
    }
}
