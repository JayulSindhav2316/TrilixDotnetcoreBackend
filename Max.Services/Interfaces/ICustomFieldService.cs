using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ICustomFieldService
    {
        Task<List<SelectListModel>> GetCustomFieldTypes();
        Task<Customfield> SaveCustomField(CustomFieldModel customfield);
        Task<List<CustomFieldModel>> GetCustomFields(int organizationId);
        Task<List<CustomFieldModel>> GetCustomFieldsByCustomFieldFor(int forId);
        Task<List<SelectListModel>> GetCustomFieldsOptions(int FieldId);
        Task<bool> DeleteCustomField(int id);
        Task<bool> UpdateField(CustomFieldModel customField);
        Task<IEnumerable<CustomfieldblockModel>> GetCustomFieldsByModuleAndTab(string moduleName, string tabName, int entityId, string currentAction);
        Task<bool> SaveCustomFieldData(List<Customfielddata> data);
        Task<bool> CheckCustomFieldData(int customFieldId);
        Task<List<Moduleinfo>> GetModuleList();
        Task<CustomfieldblockModel> AddCustomFieldBlock(CustomfieldblockModel block);
        Task<List<CustomfieldblockModel>> GetBlockList(string module, string tabinfo, int blockfor);
        Task<bool> DeleteBlock(int id);
        Task<bool> UpdateBlock(Customfieldblock block);
    }
}
