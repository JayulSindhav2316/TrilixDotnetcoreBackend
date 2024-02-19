using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
namespace Max.Services
{
    public class GlAccountTypeService : IGlAccountTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        public GlAccountTypeService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Glaccounttype>> GetAllGlAccountTypes()
        {
            return await _unitOfWork.GlAccountTypes
                .GetAllGlAccountTypesAsync();
        }

        public async Task<Glaccounttype> GetGlAccountTypeById(int id)
        {
            return await _unitOfWork.GlAccountTypes
                .GetGlAccountTypeByIdAsync(id);
        }

        public async Task<Glaccounttype> CreateGlAccountType(GlAccountTypeModel model)
        {
            Glaccounttype glAccountType = new Glaccounttype();
            var isValid = ValidGlAccountType(model);
            if (isValid)
            {
                //Map Model Data
                glAccountType.Name = model.Name;
                glAccountType.Description = model.Description;
                glAccountType.Status = model.Status;

                await _unitOfWork.GlAccountTypes.AddAsync(glAccountType);
                await _unitOfWork.CommitAsync();
            }
            return glAccountType;
        }

        public async Task<List<SelectListModel>> GetSelectList()
        {
            var glAccountTypes = await _unitOfWork.GlAccountTypes.GetAllGlAccountTypesAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var glAccountType in glAccountTypes)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = glAccountType.AccountId.ToString();
                selectListItem.name = glAccountType.Name;
                selectList.Add(selectListItem);
            }
            return selectList;
        }

        private bool ValidGlAccountType(GlAccountTypeModel model)
        {
            //Validate  Name
            if (model.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"GLAccountType Name can not be empty.");
            }

            if (model.Name == null)
            {
                throw new NullReferenceException($"GLAccountType Name can not be NULL.");
            }

            return true;
        }

    }
}
