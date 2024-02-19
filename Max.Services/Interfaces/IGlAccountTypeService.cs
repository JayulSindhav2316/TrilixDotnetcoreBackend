using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IGlAccountTypeService
    {
        Task<IEnumerable<Glaccounttype>> GetAllGlAccountTypes();
        Task<Glaccounttype> GetGlAccountTypeById(int id);
        Task<Glaccounttype> CreateGlAccountType(GlAccountTypeModel glAccountTypeModel);
        Task<List<SelectListModel>> GetSelectList();
    }
}
