using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IGrouphistoryRepository : IRepository<Grouphistory>
    {
        Task<IEnumerable<Grouphistory>> GetGrouphistoryByEntityIdAsync(int entityId);
        Task<IEnumerable<Grouphistory>> GetGrouphistoryByGroupIdAsync(int groupId);
    }
}
