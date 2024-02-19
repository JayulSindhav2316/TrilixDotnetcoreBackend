using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IStaffSearchHistoryRepository:IRepository<Staffusersearchhistory>
    {
        Task<bool> SaveSearchText(int userId, string text);
        Task<IEnumerable<Staffusersearchhistory>> GetSearchHistory(int userId);
    }
}
