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
using System.Linq;

namespace Max.Services
{
    public class GlAccountService : IGlAccountService
    {

        private readonly IUnitOfWork _unitOfWork;
        public GlAccountService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<List<GlAccountModel>> GetAllGlaccounts()
        {
            var glAccounts =  await _unitOfWork.GlAccounts
                              .GetAllGlaccountsAsync();
            List<GlAccountModel> glAccountList = new List<GlAccountModel>();
            foreach (var glAccount in glAccounts)
            {
                var model = new GlAccountModel();

                model.GlAccountId = glAccount.GlAccountId;
                model.Code = glAccount.Code;
                model.Name = glAccount.Name;
                model.CostCenter = glAccount.CostCenter.DepartmentId;
                model.CostCenterName = glAccount.CostCenter.Name;
                model.DetailType = glAccount.DetailType;
                model.GlAccountTypeName = glAccount.AccountType.Name;
                model.Type = glAccount.AccountTypeId??0;
                model.Status = glAccount.Status??0;
                glAccountList.Add(model);
            }
            return glAccountList;
        }

        public async Task<Glaccount> GetGlAccountById(int id)
        {
            return await _unitOfWork.GlAccounts
                .GetGlaccountByIdAsync(id);
        }

        public async Task<Glaccount> CreateGlAccount(GlAccountModel model)
        {
            Glaccount glAccount = new Glaccount();
            var isValid = await ValidGlaccount(model);
            if (isValid)
            {
                //Map Model Data
                glAccount.Name = model.Name;
                glAccount.DetailType = model.DetailType;
                glAccount.Code = model.Code;
                glAccount.CostCenterId = model.CostCenter;
                glAccount.GlAccountId = model.GlAccountId;
                glAccount.Status = model.Status;
                glAccount.AccountTypeId = model.Type;

                await _unitOfWork.GlAccounts.AddAsync(glAccount);
                await _unitOfWork.CommitAsync();
            }
            return glAccount;
        }

        public async Task<bool> UpdateGlAccount(GlAccountModel model)
        {
            Glaccount glAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(model.GlAccountId);
            var isValid = await ValidGlaccount(model);

            //var isNotUsed = true;
            //if (model.Status == 0)
            //{
            //    isNotUsed = ValidateGlaccountUsage(glAccount);
            //}
            
            if (isValid)
            {
                glAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(model.GlAccountId);
                if(glAccount != null)

                //Map Model Data
                glAccount.Name = model.Name;
                glAccount.DetailType = model.DetailType;
                glAccount.Code = model.Code;
                glAccount.CostCenterId = model.CostCenter;
                glAccount.Status = model.Status;
                glAccount.AccountTypeId = model.Type;

                _unitOfWork.GlAccounts.Update(glAccount);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Return selecti list with GL account codes and All option.
        /// Shall be used for Reporting purpuse
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListModel>> GetSelectList()
        {
            var Glaccounts = await _unitOfWork.GlAccounts.GetAllGlaccountsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            SelectListModel item = new SelectListModel();
            item.name = "---- ALL ----";
            item.code = "All";
            selectList.Add(item);
            foreach (var Glaccount in Glaccounts)
            {
                if (Glaccount.Status==1)
                {
                    SelectListModel selectListItem = new SelectListModel();
                    selectListItem.code = Glaccount.GlAccountId.ToString();
                    selectListItem.name = Glaccount.Code;
                    selectList.Add(selectListItem);
                }
            }
            return selectList;
        }

        public async Task<List<SelectListModel>> GetAllGLAccountsSelectList()
        {
            var Glaccounts = await _unitOfWork.GlAccounts.GetAllGlaccountsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            SelectListModel item = new SelectListModel();
            item.name = "---- ALL ----";
            item.code = "All";
            selectList.Add(item);
            foreach (var Glaccount in Glaccounts)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = Glaccount.GlAccountId.ToString();
                selectListItem.name = Glaccount.Code;
                selectList.Add(selectListItem);
            }
            return selectList;
        }
        /// <summary>
        /// Return select list with GL account codes
        /// Shall be used where selection is required
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListModel>> GetGlAccountSelectList()
        {
            var Glaccounts = await _unitOfWork.GlAccounts.GetAllGlaccountsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var Glaccount in Glaccounts)
            {
                if (Glaccount.Status == 1)
                {
                    SelectListModel selectListItem = new SelectListModel();
                    selectListItem.code = Glaccount.GlAccountId.ToString();
                    selectListItem.name = Glaccount.Code.ToString() + " - " + Glaccount.Name;
                    selectList.Add(selectListItem);
                }
            }
            return selectList;
        }

        public async Task<bool> DeleteGlAccount(int id)
        {
            Glaccount glAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(id);           

            if (glAccount != null)
            {
                var isValid = ValidateGlaccountUsage(glAccount);
                if (isValid)
                {
                    _unitOfWork.GlAccounts.Remove(glAccount);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;

        }

        private async Task<bool> ValidGlaccount(GlAccountModel model)
        {
            //Validate  Name
            if (model.Name == null)
            {
                throw new NullReferenceException($"GL Chart Of Account Name can not be NULL.");
            }

            if (model.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"GL Chart Of Account Name can not be empty.");
            }            

            var glAccount = await _unitOfWork.GlAccounts.GetGlaccountByNameAsync(model.Name);

            if (glAccount != null)
            {
                //check id ID & UserName are same -> Updating
                if (glAccount.GlAccountId != model.GlAccountId)
                {
                    throw new InvalidOperationException($"Duplicate GL Name.");
                }

            }

            // Check if there is an existing GLAccount type with same code
            var GLAccountTypes = await _unitOfWork.GlAccounts.GetAllAsync();
            if (GLAccountTypes.Where(x => x.Code == model.Code.Trim() && x.GlAccountId != model.GlAccountId).Any())
            {
                throw new InvalidOperationException($"A GL Account already exists with Code:{model.Code}.");
            }

            return true;
        }

        private bool ValidateGlaccountUsage(Glaccount model)
        {  
            //check if used in membership fees
            //var membershipTypes = await _unitOfWork.Membershiptypes.GetAllMembershipTypesAsync();
            //if ((membershipTypes.SelectMany(s => s.Membershipfees).Where(f => f.GlAccountId == model.GlAccountId)).Any())
            //    throw new InvalidOperationException($"GL Account is used in a Membership Type. It cannot be deleted");

            if(model.Membershipfees.Count > 0)
            {
                throw new InvalidOperationException($"GL Account is used in a Membership Type. It cannot be disabled/deleted");
            }

            if (model.Items.Count > 0)
            {
                throw new InvalidOperationException($"GL Account is used in a Invoice item Type. It cannot be disabled/deleted");
            }


            return true;
        }

    }
}
