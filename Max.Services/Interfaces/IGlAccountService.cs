using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IGlAccountService
    {
        Task<List<GlAccountModel>> GetAllGlaccounts();
        Task<Glaccount> GetGlAccountById(int id);
        Task<Glaccount> CreateGlAccount(GlAccountModel model);
        Task<bool> UpdateGlAccount(GlAccountModel model);
        Task<bool> DeleteGlAccount(int id);
        Task<List<SelectListModel>> GetSelectList();
        Task<List<SelectListModel>> GetGlAccountSelectList();
        Task<List<SelectListModel>> GetAllGLAccountsSelectList();
    }
}
