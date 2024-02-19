using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAccessLogRepository : IRepository<Accesslog>
    {
        Task<IEnumerable<Accesslog>> GetAccessLogByUserNameAsync(string userName);
        Task<IEnumerable<LoginStatistics>> GetTop10AccessLogAsync();
    }
}
