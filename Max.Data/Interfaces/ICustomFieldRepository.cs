using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Max.Data.Interfaces
{
    public interface ICustomFieldRepository : IRepository<Customfield>
    {
        Task<IEnumerable<Fieldtype>> GetCustomFieldTypes();
        Task<Customfield> SaveCustomField(Customfield customfield);
        Task<List<Customfield>> GetCustomFields(int organizationId);
        Task<List<Customfield>> GetCustomFieldsByCustomFieldFor(int forId);
        Task<bool> DeleteCustomField(int id);
        Task<bool> SaveFieldOptions(Customfieldoption customfieldoptions);
        Task<List<Customfieldoption>> GetCustomFieldsOptions(int FieldId);
        Task<bool> DeleteCustomFieldOption(int CustomFieldId);
        Task<Customfield> GetCustomFieldById(int CustomFieldId);
        Task<bool> AddLookupValues(Customfieldlookup lookup);
        Task<List<Customfieldblock>> GetCustomFieldsByModuleAndTab(string moduleName, string tabName, int entityId, int customFieldFor);
        Task<bool> SaveCustomFieldData(Customfielddata data);
        Task<Customfielddata> GetCustomFieldData(int customFieldId, int entityId);
        bool UpdateCustomFieldData(Customfielddata data);
        bool DeleteCustomFieldlookup(int CustomFieldId);
        bool DeleteReportField(int CustomFieldId);
        bool DeleteCustomFieldData(int id);
        Task<bool> CheckCustomFieldData(int customFieldId);
        Task<List<Moduleinfo>> GetModuleList();
        Task<Customfieldblock> AddCustomFieldBlock(Customfieldblock block);
        Task<List<Customfieldblock>> GetBlockList(string module, string tabinfo, int blockfor);
        bool DeleteBlock(int id);
        Task<Customfieldblock> GetCustomFieldBlockById(int blockId);
        bool UpdateCustomFieldBlock(Customfieldblock block);
    }
}
