using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IStaffSearchHistoryService
    {
        Task<bool> SaveSearchText(string text, int userId);
        Task<List<string>>GetSearchHistory(int userId);
        Task<bool> DeleteSearchHistory(string text, int id);
    }
}
