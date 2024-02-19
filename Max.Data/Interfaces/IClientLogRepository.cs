using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IClientLogRepository : IRepository<Clientlog>
    {
        Task<IEnumerable<Clientlog>> GetAllLientLogsAsync();
    }
}
