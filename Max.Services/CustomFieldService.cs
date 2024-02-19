using Max.Services.Interfaces;
using System;
using System.Collections.Generic;
using AutoMapper;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Max.Core;
using Microsoft.IdentityModel.Tokens;

namespace Max.Services
{
    public class CustomFieldService : ICustomFieldService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISociableService _sociableService;
        public CustomFieldService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISociableService sociableService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _sociableService = sociableService;
        }   

        public async Task<bool> DeleteCustomField(int id)
        {
            var field = await _unitOfWork.CustomFields.GetCustomFieldById(id);
            if(field.Customfielddata.Count()>0)
            {
                return false;
            }
            await _unitOfWork.CustomFields.DeleteCustomFieldOption(id);
            await _unitOfWork.CommitAsync();   
            if (field != null)
            {
                _unitOfWork.CustomFields.DeleteCustomFieldlookup(field.CustomFieldId);
                _unitOfWork.CustomFields.DeleteReportField(field.CustomFieldId);
                var customField = await _unitOfWork.CustomFields.GetByIdAsync(id);
                _unitOfWork.CustomFields.Remove(customField);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<List<CustomFieldModel>> GetCustomFields(int organizationId)
        {
            var result = await _unitOfWork.CustomFields.GetCustomFields(1);
            var model = _mapper.Map<List<CustomFieldModel>>(result);
            return model;
        }

        public async Task<List<CustomFieldModel>> GetCustomFieldsByCustomFieldFor(int forId)
        {
            var result = await _unitOfWork.CustomFields.GetCustomFields(forId);
            var model = _mapper.Map<List<CustomFieldModel>>(result);
            return model;
        }

        public async Task<List<SelectListModel>> GetCustomFieldsOptions(int FieldId)
        {
            var results = await _unitOfWork.CustomFields.GetCustomFieldsOptions(FieldId);
            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var item in results)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = item.CustomFieldId.ToString();
                selectListItem.name = item.Option;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public async Task<List<SelectListModel>> GetCustomFieldTypes()
        {
            var result = await _unitOfWork.CustomFields.GetCustomFieldTypes();
            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var itemType in result)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = itemType.FieldTypeId.ToString();
                selectListItem.name = itemType.Type;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        public async Task<Customfield> SaveCustomField(CustomFieldModel customfield)
        {
            //customfield.CustomFieldFor = (int)CustomFieldFor.Contact;
            var model = _mapper.Map<Customfield>(customfield);
            var list = new List<Customfieldoption>();

            model.CustomFieldFor = customfield.CustomFieldFor;
            //model.Customfieldoptions = list;
            var duplicate = _unitOfWork.CustomFields.Find(x => x.FieldTypeId == customfield.FieldTypeId && x.Label == customfield.Label).ToList();
            if (duplicate.Count > 0)
            {
                return null;
                //return response;
            }
             
            await _unitOfWork.CustomFields.AddAsync(model);
            await _unitOfWork.CommitAsync();
            if (customfield.Options != null && customfield.Options.Count > 0)
            {
                foreach (var op in customfield.Options)
                {
                    var model2 = new Customfieldoption();
                    model2.CustomFieldId = model.CustomFieldId;//model.CustomFieldId;
                    model2.Option = op;

                    await _unitOfWork.CustomFields.SaveFieldOptions(model2);
                    await _unitOfWork.CommitAsync();
                }
            }
            var lookupModel = new Customfieldlookup();
            lookupModel.BlockId = customfield.BlockId;
            lookupModel.ModuleId = 1;
            lookupModel.CustomFieldId = model.CustomFieldId;
            lookupModel.OrderOfDisplay = 1;
            lookupModel.TabId = 1;
            lookupModel.Status = 1;
            await _unitOfWork.CustomFields.AddLookupValues(lookupModel);
            await _unitOfWork.CommitAsync();

            // sync with reportfield table
            var reportField = new Reportfield();
            reportField.CustomFieldId = model.CustomFieldId;
            if (model.CustomFieldFor == 1) reportField.ReportCategoryId = 2;
            else reportField.ReportCategoryId = 3;
            reportField.FieldName = model.Label;
            reportField.TableName = "customfield";
            reportField.Label = "Additional Info";
            reportField.FieldTitle = model.Label;
            int?[] textNumbers = {1, 2, 3, 7, 8};
            int?[] dropdownNumbers = { 9, 10, 11 };
            if (textNumbers.Contains(model.FieldTypeId))
            {
                reportField.DataType = "String";
                reportField.DisplayType = "Text";
            }
            else if (dropdownNumbers.Contains(model.FieldTypeId))
            {
                reportField.DataType = "String";
                reportField.DisplayType = "Dropdown";
            }
            else if (model.FieldTypeId == 6)
            {
                reportField.DataType = "Int";
                reportField.DisplayType = "Number";
            }
            else
            {
                reportField.DataType = "Date";
                reportField.DisplayType = "Calendar";
            }
            reportField.DisplayOrder = 7;
            await _unitOfWork.ReportFields.AddAsync(reportField);
            await _unitOfWork.CommitAsync();

            return model;
        }
        public async Task<bool> UpdateField(CustomFieldModel customField)
        {
            var field = await _unitOfWork.CustomFields.GetCustomFieldById(customField.CustomFieldId);
            if(field.FieldType.FieldTypeId!=customField.FieldTypeId)
            {
                await DeleteCustomField(field.CustomFieldId);
                await SaveCustomField(customField);
                return true;
            }
            if (field.FieldType.Type == "Dropdown" || field.FieldType.Type == "Single Choice Field" || field.FieldType.Type == "Multiple Choice Field")
            {
                await _unitOfWork.CustomFields.DeleteCustomFieldOption(field.CustomFieldId);
                await _unitOfWork.CommitAsync();
                if (customField.Options.Count > 0)
                {
                    foreach (var op in customField.Options)
                    {
                        var optionModel = new Customfieldoption();
                        optionModel.CustomFieldId = field.CustomFieldId;//model.CustomFieldId;
                        optionModel.Option = op;

                        await _unitOfWork.CustomFields.SaveFieldOptions(optionModel);
                        await _unitOfWork.CommitAsync();
                    }
                }

            }

            //update report field
            var reportField = field.Reportfields.Where(x => x.CustomFieldId == field.CustomFieldId).FirstOrDefault();
            reportField.FieldName = field.Label;
            reportField.FieldTitle = field.Label;
            int?[] textNumbers = { 1, 2, 3, 7, 8 };
            int?[] dropdownNumbers = { 9, 10, 11 };
            if (textNumbers.Contains(field.FieldTypeId))
            {
                reportField.DataType = "String";
                reportField.DisplayType = "Text";
            }
            else if (dropdownNumbers.Contains(field.FieldTypeId))
            {
                reportField.DataType = "String";
                reportField.DisplayType = "Dropdown";
            }
            else if (field.FieldTypeId == 6)
            {
                reportField.DataType = "Int";
                reportField.DisplayType = "Number";
            }
            else
            {
                reportField.DataType = "Date";
                reportField.DisplayType = "Calendar";
            }
            _unitOfWork.ReportFields.Update(reportField);

            if (field != null)
            {
                field.Label = customField.Label;
                field.Placeholder = customField.Placeholder;
                field.Validations = customField.Validations;
                field.CharacterLimit = customField.CharacterLimit;
                field.Required = customField.Required;
                field.CountryCode = customField.CountryCode;
                field.DateFormat = customField.DateFormat;
                field.DefaultDate = customField.DefaultDate;
                field.TimeFormat = customField.TimeFormat;
                field.DefaultTime = customField.DefaultTime;
                field.Status = customField.Status;
                field.Editable = customField.Editable;
                field.MultipleSelection = customField.MultipleSelection;
            }
            _unitOfWork.CustomFields.Update(field);
            await _unitOfWork.CommitAsync();

            return true;
        }
        public async Task<IEnumerable<CustomfieldblockModel>> GetCustomFieldsByModuleAndTab(string moduleName, string tabName, int entityId, string currentAction)
        {
            int customFieldFor = 0;
            if (entityId!=0)
            {
                var entity = await _unitOfWork.Entities.GetEntityByIdAsync(entityId);
                 customFieldFor = 1;
                if (entity.CompanyId != null)
                    customFieldFor = 2;
            }
            else if(currentAction=="AddNewPerson" && entityId==0)
            {
                customFieldFor = 1;
            }
            else if (currentAction == "AddCompanyBillableContact" && entityId == 0)
            {
                customFieldFor = 1;
            }
            else if(currentAction == "AddNewCompany" && entityId == 0)
            {
                customFieldFor = 2;
            }
                var list = await _unitOfWork.CustomFields.GetCustomFieldsByModuleAndTab(moduleName, tabName, entityId, customFieldFor);
                
                var model = _mapper.Map<List<CustomfieldblockModel>>(list);
                return model;
        }
        public async Task<bool> SaveCustomFieldData(List<Customfielddata> data)
        {
           var staffContext = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
           var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(staffContext.OrganizationId);
           foreach(var x in data)
           {
                var fieldData = await _unitOfWork.CustomFields.GetCustomFieldData(x.CustomFieldId??0, x.EntityId??0);
                if(fieldData == null)
                {
                    if (!string.IsNullOrEmpty(x.Value))
                    {
                        await _unitOfWork.CustomFields.SaveCustomFieldData(x);
                        await _unitOfWork.CommitAsync();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(x.Value))
                    {
                        _unitOfWork.CustomFields.DeleteCustomFieldData(fieldData.Id);
                        await _unitOfWork.CommitAsync();
                    }
                    else
                    {
                        fieldData.Value = x.Value;
                        _unitOfWork.CustomFields.UpdateCustomFieldData(fieldData);
                        await _unitOfWork.CommitAsync();
                    }
                }
                if(configuration != null)
                {
                    if(configuration.SociableSyncEnabled == (int)Status.Active && !string.IsNullOrEmpty(configuration.CustomFieldsToSync))
                    {
                        var customFieldDetails = await _unitOfWork.CustomFields.GetByIdAsync(x.CustomFieldId ?? 0);
                        if(customFieldDetails != null)
                        {
                            if(configuration.CustomFieldsToSync.Replace(" ","").ToLower().Contains(customFieldDetails.Label.Replace(" ", "").ToLower()))
                            {
                                if (x.EntityId != null)
                                {
                                    var entityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(x.EntityId));
                                    if (entityDetails != null && (entityDetails.SociableProfileId != null || entityDetails.SociableProfileId > 0))
                                    {
                                        await _sociableService.UpdateCustomFields(customFieldDetails.Label.Replace(" ", "").ToLower(), x.Value, Convert.ToInt32(entityDetails.SociableProfileId), staffContext.OrganizationId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        public async Task<bool> CheckCustomFieldData(int customFieldId)
        {
            var res = await _unitOfWork.CustomFields.CheckCustomFieldData(customFieldId);
            return res;
        }

        public Task<List<Moduleinfo>> GetModuleList()
        {
            var list=_unitOfWork.CustomFields.GetModuleList();
            return list;
        }
        public async Task<CustomfieldblockModel> AddCustomFieldBlock(CustomfieldblockModel block)
        {
            var model = _mapper.Map<Customfieldblock>(block);
            var module = await GetModuleList();
            var data= module.Where(x => x.Name == block.ModuleName).FirstOrDefault();
            model.ModuleId = data.ModuleId;
            model.TabId = data.Tabinfos.Where(x => x.Name == block.TabName).FirstOrDefault().TabId;
            await _unitOfWork.CustomFields.AddCustomFieldBlock(model);
            await _unitOfWork.CommitAsync();
            return block;
        }
        public async Task<List<CustomfieldblockModel>> GetBlockList(string module, string tabinfo, int blockfor)
        {
            var list = await _unitOfWork.CustomFields.GetBlockList(module,tabinfo,blockfor);
            var model = _mapper.Map<List<CustomfieldblockModel>>(list);
            return model;
        }

        public async Task<bool> DeleteBlock(int id)
        {
            var block=await _unitOfWork.CustomFields.GetCustomFieldBlockById(id);
            var res = true;
            foreach(var field in block.Customfieldlookups)
            {
                var cusId = field.CustomFieldId;
                var data = await CheckCustomFieldData(cusId ?? 0);
                if (data)
                {
                    res = false;
                    break;
                }
                else
                {
                    await DeleteCustomField(cusId ?? 0);
                }
            }
            if(res==true)
            {
                _unitOfWork.CustomFields.DeleteBlock(id);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                return res;
            }

            return true;
        }

        public async Task<bool> UpdateBlock(Customfieldblock block)
        {
            var model = await _unitOfWork.CustomFields.GetCustomFieldBlockById(block.BlockId);
            model.Name = block.Name;
            model.ShowBlock = block.ShowBlock;
            _unitOfWork.CustomFields.UpdateCustomFieldBlock(model);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
