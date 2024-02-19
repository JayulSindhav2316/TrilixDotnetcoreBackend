using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System.Numerics;
using Max.Core.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Max.Data.Repositories
{
    public class CustomFieldRepository : Repository<Customfield>, ICustomFieldRepository
    {
        public CustomFieldRepository(membermaxContext context)
          : base(context)
        { }

        public Task<bool> DeleteCustomField(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<bool> DeleteCustomFieldOption(int CustomFieldId)
        {
            var list = await membermaxContext.Customfieldoptions.Where(c => c.CustomFieldId == CustomFieldId).ToListAsync();
            membermaxContext.Customfieldoptions.RemoveRange(list);
            return true;
        }

        public bool DeleteCustomFieldlookup(int CustomFieldId)
        {
            var lookup = membermaxContext.Customfieldlookups.Where(c => c.CustomFieldId == CustomFieldId).FirstOrDefault();
            membermaxContext.Customfieldlookups.Remove(lookup);
            return true;
        }

        public bool DeleteReportField(int CustomFieldId)
        {
            var reportField = membermaxContext.Reportfields.Where(c => c.CustomFieldId == CustomFieldId).FirstOrDefault();
            membermaxContext.Reportfields.Remove(reportField);
            return true;
        }

        public async Task<List<Customfield>> GetCustomFields(int organizationId)
        {
            return await membermaxContext.Customfields.Include(x => x.FieldType)
                .Include(x => x.Customfieldoptions).OrderByDescending(x => x.CustomFieldId).ToListAsync();
        }

        public async Task<List<Customfield>> GetCustomFieldsByCustomFieldFor(int forId)
        {
            return await membermaxContext.Customfields.Include(x => x.FieldType)
                .Include(x => x.Customfieldoptions).Where(x=>x.CustomFieldFor == forId).ToListAsync();
        }

        public async Task<Customfield> GetCustomFieldById(int CustomFieldId)
        {
            return await membermaxContext.Customfields
                .Include(x=>x.Reportfields)
                .Include(x=>x.Customfielddata)
                .Include(x => x.Customfieldlookups)
                .Include(x => x.FieldType)
                .FirstOrDefaultAsync(x => x.CustomFieldId == CustomFieldId);
        }

        public async Task<Customfield> SaveCustomField(Customfield customfield)
        {
            var result = await membermaxContext.Customfields.AddAsync(customfield);
            return customfield;
        }
        public async Task<IEnumerable<Fieldtype>> GetCustomFieldTypes()
        {
            var res = await membermaxContext.Fieldtypes.ToListAsync();
            return res;
        }

        public async Task<bool> SaveFieldOptions(Customfieldoption customfieldoptions)
        {
            await membermaxContext.Customfieldoptions.AddAsync(customfieldoptions);
            return true;
        }
        public async Task<List<Customfieldoption>> GetCustomFieldsOptions(int FieldId)
        {
            return await membermaxContext.Customfieldoptions.Where(x => x.CustomFieldId == FieldId).ToListAsync();
            //throw new System.NotImplementedException();
        }
        public async Task<bool> AddLookupValues(Customfieldlookup lookup)
        {
            await membermaxContext.Customfieldlookups.AddAsync(lookup);
            return true;
        }
        public async Task<bool> SaveCustomFieldData(Customfielddata data)
        {
            await membermaxContext.Customfielddata.AddAsync(data);
            return true;
        }
        public async Task<bool> CheckCustomFieldData(int customFieldId)
        {
           var res= await membermaxContext.Customfielddata.AnyAsync(x=>x.CustomFieldId== customFieldId);
           return res;
        }
        public async Task<List<Customfieldblock>> GetCustomFieldsByModuleAndTab(string moduleName, string tabName, int entityId, int customFieldFor)
        {
            var test = await membermaxContext.Customfieldblocks.Include(x => x.Customfieldlookups)
                .Include(x => x.Customfieldlookups).ThenInclude(x=>x.CustomField).ThenInclude
                (x=>x.Customfielddata.Where(x=>x.EntityId==entityId))

                .Include(x => x.Customfieldlookups).ThenInclude(x => x.CustomField)
                 .ThenInclude(x => x.FieldType)

                 .Include(x => x.Customfieldlookups).ThenInclude(x => x.CustomField)
                 .ThenInclude(x => x.Customfieldoptions)

                 .Include(x => x.Customfieldlookups).ThenInclude(x => x.Module)

                 .Include(x => x.Customfieldlookups).ThenInclude(x => x.Module).ThenInclude(x => x.Tabinfos)
                 .Where(x => x.BlockFor == customFieldFor && x.Module.Name == moduleName && x.Tab.Name == tabName)
                 .ToListAsync();
            return test;

            //var list = await membermaxContext.Customfieldlookups.Include(x => x.CustomField)
            //    .Include(x=>x.CustomField).ThenInclude(x => x.Customfielddata.Where(x=>x.EntityId==entityId))
            //    .Include(x => x.CustomField)
            //      .ThenInclude(x => x.Customfieldoptions)
            //     .Include(x => x.CustomField).ThenInclude(x => x.FieldType)
            //     .Include(x => x.Module)
            //     .ThenInclude(x => x.Tabinfos.Where(x => x.Name == tabName)).Where(x => x.Module.Name == moduleName).ToListAsync();
            //return list.Where(x=>x.CustomField.CustomFieldFor==customFieldFor).ToList();
        }
        public async Task<Customfielddata> GetCustomFieldData(int customFieldId, int entityId)
        {
            var data = await membermaxContext.Customfielddata.Where(x=>x.CustomFieldId==customFieldId && x.EntityId==entityId).FirstOrDefaultAsync();
            return data;
        }
        public bool UpdateCustomFieldData(Customfielddata data)
        {
            membermaxContext.Customfielddata.Update(data);
            return true;
        }
        public bool DeleteCustomFieldData(int id)
        {
            var lookup = membermaxContext.Customfielddata.Where(c => c.Id == id).FirstOrDefault();
            membermaxContext.Customfielddata.Remove(lookup);
            return true;
        }
        public async Task<List<Moduleinfo>> GetModuleList()
        {
            return await membermaxContext.Moduleinfos.Include(x => x.Tabinfos).ToListAsync();
        }

        public async Task<Customfieldblock> AddCustomFieldBlock(Customfieldblock block)
        {
            await membermaxContext.Customfieldblocks.AddAsync(block);
            return block;
        }
        public async Task<List<Customfieldblock>> GetBlockList(string module, string tabinfo, int blockfor)
        {
           
            return await membermaxContext.Customfieldblocks.Include(x => x.Customfieldlookups)
                .ThenInclude(x => x.CustomField).ThenInclude(x => x.FieldType)
                .Include(x => x.Customfieldlookups).ThenInclude(x => x.CustomField)
                .ThenInclude(x => x.Customfieldoptions)
                .Include(x => x.Customfieldlookups).ThenInclude(x => x.Module)
                .Include(x => x.Customfieldlookups).ThenInclude(x => x.Module).ThenInclude(x => x.Tabinfos)
                .Where(x=>x.BlockFor==blockfor && x.Module.Name==module && x.Tab.Name==tabinfo)

                .ToListAsync();
        }

        public bool DeleteBlock(int id)
        {
            var block= membermaxContext.Customfieldblocks.Where(x=>x.BlockId==id).FirstOrDefault();
            membermaxContext.Customfieldblocks.Remove(block);
            return true;
        }
        public async Task<Customfieldblock> GetCustomFieldBlockById(int blockId)
        {
            return await membermaxContext.Customfieldblocks
                //.Include(x => x.Customfielddata)
                .Include(x => x.Customfieldlookups)
                .ThenInclude(x=>x.CustomField)
                //.Include(x => x.FieldType)
                .FirstOrDefaultAsync(x => x.BlockId == blockId);
        }
        public bool UpdateCustomFieldBlock(Customfieldblock block)
        {
            membermaxContext.Customfieldblocks.Update(block);
            return true;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
