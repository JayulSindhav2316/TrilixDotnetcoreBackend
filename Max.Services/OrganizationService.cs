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
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Organization>> GetAllOrganizations()
        {
            return await _unitOfWork.Organizations
                .GetAllOrganizationsAsync();
        }

        public async Task<Organization> GetOrganizationById(int id)
        {
            return await _unitOfWork.Organizations
                .GetOrganizationByIdAsync(id);
        }

        public async Task<Organization> GetOrganizationByName(string name)
        {
            return await _unitOfWork.Organizations
                .GetOrganizationByNameAsync(name);
        }
        public async Task<Organization> CreateOrganization(OrganizationModel model)
        {
            Organization organization = new Organization();
            var isValid = ValidOrganization(model);
            if (isValid)
            {
                //Map Model Data
                organization.Name = model.Name;
                organization.Code = model.Code;
                organization.Title = model.Title;
                organization.Email = model.Email;
                organization.Phone = model.Phone.GetCleanPhoneNumber();
                organization.Address1 = model.Address1;
                organization.Address2 = model.Address2;
                organization.Address3 = model.Address3;
                organization.State = model.State;
                organization.City = model.City;
                organization.Zip = model.Zip;
                organization.WebMessage = model.WebMessage;
                organization.PrintMessage = model.PrintMessage;
                organization.Website = model.Website;
                organization.Facebook = model.Facebook;
                organization.Twitter = model.Twitter;
                organization.LinkedIn = model.LinkedIn;
                organization.AccountName = model.AccountName;
                organization.PrimaryContactName = model.PrimaryContactName;
                organization.PrimaryContactPhone = model.PrimaryContactPhone.GetCleanPhoneNumber();
                organization.PrimaryContactEmail = model.PrimaryContactEmail;

                try
                {
                    await _unitOfWork.Organizations.AddAsync(organization);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                Accountingsetup accountingsetup = new Accountingsetup();

                if (model.AccountingSetUpModel != null)
                {
                    accountingsetup.OnlineCreditGlAccount = model.AccountingSetUpModel.OnlineCreditGlAccount;
                    accountingsetup.OffLinePaymentGlAccount = model.AccountingSetUpModel.OffLinePaymentGlAccount;
                    accountingsetup.SalesReturnGlAccount = model.AccountingSetUpModel.SalesReturnGlAccount;
                    accountingsetup.ProcessingFeeGlAccount = model.AccountingSetUpModel.ProcessingFeeGlAccount;
                    accountingsetup.WriteOffGlAccount = model.AccountingSetUpModel.WriteOffGlAccount;
                    accountingsetup.OrganizationId = organization.OrganizationId;

                    //organization.Accountingsetups.Add(accountingsetup);

                    try
                    {
                        await _unitOfWork.AccountingSetups.AddAsync(accountingsetup);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return organization;
        }

        public async Task<bool> UpdateOrganization(OrganizationModel model)
        {
            var isValid = ValidOrganization(model);
            if (isValid)
            {
                var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(model.OrganizationId);

                if(organization!=null)
                {
                    //Map Model Data
                    //organization.Name = model.Name; // Not Editable
                    organization.Title = model.Title;
                    organization.Code = model.Code;
                    organization.Email = model.Email;
                    organization.Phone = model.Phone.GetCleanPhoneNumber();
                    organization.Address1 = model.Address1;
                    organization.Address2 = model.Address2;
                    organization.Address3 = model.Address3;
                    organization.State = model.State;
                    organization.City = model.City;
                    organization.Zip = model.Zip;
                    organization.WebMessage = model.WebMessage;
                    organization.PrintMessage = model.PrintMessage;
                    organization.Website = model.Website;
                    organization.Facebook = model.Facebook;
                    organization.Twitter = model.Twitter;
                    organization.LinkedIn = model.LinkedIn;
                    organization.AccountName = model.AccountName;
                    organization.PrimaryContactName = model.PrimaryContactName;
                    organization.PrimaryContactPhone = model.PrimaryContactPhone.GetCleanPhoneNumber();
                    organization.PrimaryContactEmail = model.PrimaryContactEmail;

                    _unitOfWork.Organizations.Update(organization);

                    Accountingsetup accountingsetup = await _unitOfWork.AccountingSetups.GetAccountingSetupByOrganizationIdAsync(model.OrganizationId);

                    try
                    {
                        if (accountingsetup == null)
                        {
                            accountingsetup = new Accountingsetup();
                            accountingsetup.OrganizationId = model.OrganizationId;
                            accountingsetup.OnlineCreditGlAccount = model.AccountingSetUpModel.OnlineCreditGlAccount;
                            accountingsetup.OffLinePaymentGlAccount = model.AccountingSetUpModel.OffLinePaymentGlAccount;
                            accountingsetup.SalesReturnGlAccount = model.AccountingSetUpModel.SalesReturnGlAccount;
                            accountingsetup.ProcessingFeeGlAccount = model.AccountingSetUpModel.ProcessingFeeGlAccount;
                            accountingsetup.WriteOffGlAccount = model.AccountingSetUpModel.WriteOffGlAccount;

                            await _unitOfWork.AccountingSetups.AddAsync(accountingsetup);
                        }
                        else
                        {
                            accountingsetup.OrganizationId = model.OrganizationId;
                            accountingsetup.OnlineCreditGlAccount = model.AccountingSetUpModel.OnlineCreditGlAccount;
                            accountingsetup.OffLinePaymentGlAccount = model.AccountingSetUpModel.OffLinePaymentGlAccount;
                            accountingsetup.SalesReturnGlAccount = model.AccountingSetUpModel.SalesReturnGlAccount;
                            accountingsetup.ProcessingFeeGlAccount = model.AccountingSetUpModel.ProcessingFeeGlAccount;
                            accountingsetup.WriteOffGlAccount = model.AccountingSetUpModel.WriteOffGlAccount;

                            _unitOfWork.AccountingSetups.Update(accountingsetup);
                        }                        
                       
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                
            }
            return false;
        }

        public async Task<List<SelectListModel>> GetSelectList()
        {
            var departments = await _unitOfWork.Organizations.GetAllOrganizationsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var department in departments)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = department.OrganizationId.ToString();
                selectListItem.name = department.Name;
                selectList.Add(selectListItem);
            }
            return selectList;
        }
        private bool ValidOrganization(OrganizationModel model)
        {
            //Validate  Name
            if (model.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Organization Name can not be empty.");
            }

            if (model.Name == null)
            {
                throw new NullReferenceException($"Organization Name can not be NULL.");
            }

            return true;
        }
    }
}