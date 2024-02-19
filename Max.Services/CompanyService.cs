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
using AutoMapper;
using Serilog;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class CompanyService : ICompanyService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        static readonly ILogger _logger = Serilog.Log.ForContext<CompanyService>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISociableService _sociableService;
        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISociableService sociableService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
            this._sociableService = sociableService;
        }

        public async Task<CompanyModel> GetCompanyById(int id)
        {
            var company = await _unitOfWork.Companies.GetCompanyByIdAsync(id);
            CompanyModel model = _mapper.Map<CompanyModel>(company);

            if (company != null)
            {
                var primaryAddress = model.Addresses.GetPrimaryAddress();
                model.StreetAddress = primaryAddress.StreetAddress;
                model.City = primaryAddress.City;
                model.Country = primaryAddress.Country;
                model.State = primaryAddress.State;
                model.Zip = primaryAddress.Zip.FormatZip();
                model.Phone = model.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                model.Email = model.Emails.GetPrimaryEmail();
                if (model.BillableContactId != 0)
                {
                    var person = await _unitOfWork.Persons.GetPersonByIdAsync(model.BillableContactId);
                    if (person != null)
                    {
                        model.BillablePerson = _mapper.Map<PersonModel>(person);
                        model.BillablePerson.PrimaryPhone = model.BillablePerson.Phones.GetPrimaryPhoneNumber();
                        model.BillablePerson.PrimaryEmail = model.BillablePerson.Emails.GetPrimaryEmail();
                    }
                }
                return model;
            }
            return model;
        }
        public async Task<CompanyModel> CreateCompany(CompanyModel model)
        {
            Company company = new Company();

            //Map Model Data
            company.CompanyName = model.CompanyName;
            company.Website = model.Website;
            company.BillableContactId = model.BillableContactId;
            company.MemberId = model.MemberId;
            //Add  Emails

            //Check if primary email is unique 
            var existingEmail = await _unitOfWork.Emails.GetPrimaryEmailByEmailAddressAsync(model.Emails.Where(x => x.IsPrimary == (int)Status.Active).Select(x => x.EmailAddress).FirstOrDefault());

            if (existingEmail != null)
            {
                throw new Exception("Please use a unique Email Address for Primary Email.");
            }
            foreach (var item in model.Emails)
            {
                if (item != null)
                {
                    var staffUserEmail = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(item.EmailAddress);
                    if (staffUserEmail != null)
                    {
                        throw new Exception($"Please use a different Email Address , {item.EmailAddress} already exists.");
                    }
                    Email email = new Email();
                    email.EmailAddressType = item.EmailAddressType;
                    email.EmailAddress = item.EmailAddress;
                    email.IsPrimary = item.IsPrimary;
                    company.Emails.Add(email);
                }
            }

            //Add  Phone Numbers
            foreach (var item in model.Phones)
            {
                Phone phone = new Phone();
                phone.PhoneType = item.PhoneType;
                phone.PhoneNumber = item.PhoneNumber.GetCleanPhoneNumber();
                phone.IsPrimary = item.IsPrimary;
                company.Phones.Add(phone);
            }

            //Add  Addresses
            foreach (var item in model.Addresses)
            {
                Address address = new Address();
                address.AddressType = item.AddressType;
                address.Address1 = item.Address1;
                address.Address2 = item.Address2;
                address.Address3 = item.Address3;
                address.City = item.City;
                address.Country = item.Country;
                address.CountryCode = item.CountryCode;
                address.State = item.State;
                address.StateCode = item.StateCode;
                address.Zip = item.Zip.Replace("_", "").Length == 6 ? item.Zip.Replace("_", "").Replace("-", "") : item.Zip;
                address.IsPrimary = item.IsPrimary;

                company.Addresses.Add(address);
            }
            var entity = new Entity();
            entity.Name = model.CompanyName;
            entity.OrganizationId = model.OrganizationId;
            company.Entity = entity;
            try
            {
                await _unitOfWork.Companies.AddAsync(company);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to create company:{ex.Message}");
            }

            entity.CompanyId = company.CompanyId;

            try
            {
                _unitOfWork.Entities.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to create entity:{ex.Message}");
            }

            var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
            var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(Convert.ToInt32(staff.OrganizationId));
            // Company add/update operation
            if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
            {
                model.Entity = _mapper.Map<EntityModel>(entity);
                int returnedSociableCompanyId = await _sociableService.CreatePerson(null, model, staff.OrganizationId);
                var sociableCompanyId = returnedSociableCompanyId;
                entity.SociableUserId = returnedSociableCompanyId;

                if (sociableCompanyId > 0)
                {
                    var companyInfo = await _sociableService.GetUserById(Convert.ToInt32(sociableCompanyId), staff.OrganizationId);
                    dynamic companyModel = JObject.Parse(companyInfo);
                    var companyProfileId = companyModel.profile_profiles[0].target_id;
                    if (companyProfileId > 0)
                    {

                        var result = await _sociableService.UpdatePersonProfile(null, model, (int)companyProfileId, staff.OrganizationId);
                    }
                    entity.SociableProfileId = companyProfileId;
                    _unitOfWork.Entities.Update(entity);
                    await _unitOfWork.CommitAsync();
                }
            }

            return _mapper.Map<CompanyModel>(company);
        }

        public async Task<bool> UpdateCompany(CompanyModel model)
        {

            var company = await _unitOfWork.Companies.GetCompanyByIdAsync(model.CompanyId);
            bool updateEntity = false;

            if (company != null)
            {
                if (company.CompanyName != model.CompanyName)
                {
                    updateEntity = true;
                }
                //Check if primary email is unique 

                var existingEmail = await _unitOfWork.Emails.GetPrimaryEmailByEmailAddressAsync(model.Emails.Where(x => x.IsPrimary == (int)Status.Active).Select(x => x.EmailAddress).FirstOrDefault());

                if (existingEmail != null)
                {
                    if (existingEmail.CompanyId != model.CompanyId)
                    {
                        throw new Exception("Please use an unique Email Address for Primary Email.");
                    }

                }
                try
                {
                    company.BillableContactId = model.BillableContactId;
                    company.CompanyName = model.CompanyName;
                    company.Website = model.Website;
                    company.MemberId = model.MemberId;

                    //Add / Update Address
                    var addresses = company.Addresses.ToList();

                    foreach (var item in addresses)
                    {
                        if (model.Addresses.Any(x => x.AddressId == item.AddressId))
                        {
                            AddressModel address = model.Addresses.Where(x => x.AddressId == item.AddressId).FirstOrDefault();
                            item.AddressType = address.AddressType;
                            item.Address1 = address.Address1;
                            item.Address2 = address.Address2;
                            item.Address3 = address.Address3;
                            item.City = address.City;
                            item.Country = address.Country;
                            item.CountryCode = address.CountryCode;
                            item.State = address.State;
                            item.StateCode = address.StateCode;
                            item.Zip = address.Zip.Replace("_", "").Length == 6 ? address.Zip.Replace("_", "").Replace("-", "") : address.Zip;
                            item.IsPrimary = address.IsPrimary;

                            _unitOfWork.Addresses.Update(item);
                            continue;

                        }

                        _unitOfWork.Addresses.Remove(item);
                        company.Addresses.Remove(item);
                    }

                    //Add  Addresses
                    foreach (var item in model.Addresses.Where(x => x.AddressId == 0).ToList())
                    {

                        Address address = _mapper.Map<Address>(item);
                        company.Addresses.Add(address);
                    }
                    //Add / Update Emails
                    var emails = company.Emails.ToList();

                    foreach (var item in emails)
                    {
                        if (model.Emails.Any(x => x.EmailId == item.EmailId))
                        {
                            EmailModel emailModel = model.Emails.Where(x => x.EmailId == item.EmailId).FirstOrDefault();
                            var staffUserEmail = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(emailModel.EmailAddress);
                            if (staffUserEmail != null)
                            {
                                throw new Exception($"Please use a different Email Address , {emailModel.EmailAddress} already exists.");
                            }
                            item.EmailAddressType = emailModel.EmailAddressType;
                            item.EmailAddress = emailModel.EmailAddress;
                            item.IsPrimary = emailModel.IsPrimary;
                            try
                            {
                                _unitOfWork.Emails.Update(item);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"Update Company: Update email failed:  {company.CompanyName} with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                                throw new Exception($"Failed to update company email");
                            }
                            continue;

                        }

                        _unitOfWork.Emails.Remove(item);
                        company.Emails.Remove(item);
                    }
                    //Add  Emails
                    foreach (var item in model.Emails.Where(x => x.EmailId == 0).ToList())
                    {
                        var staffUserEmail = await _unitOfWork.Staffusers.GetStaffUserByEmailAsync(item.EmailAddress);
                        if (staffUserEmail != null)
                        {
                            throw new Exception($"Please use a different Email Address , {item.EmailAddress} already exists.");
                        }
                        Email email = new Email();
                        email.EmailAddressType = item.EmailAddressType;
                        email.EmailAddress = item.EmailAddress;
                        email.IsPrimary = item.IsPrimary;
                        try
                        {
                            company.Emails.Add(email);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Update Company: Update email failed: Company Name: {model.CompanyName}  failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                            throw new Exception($"Failed to update company email");
                        }
                    }

                    //Add / Update Phones
                    var phones = company.Phones.ToList();

                    foreach (var item in phones)
                    {
                        if (model.Phones.Any(x => x.PhoneId == item.PhoneId))
                        {
                            PhoneModel phone = model.Phones.Where(x => x.PhoneId == item.PhoneId).FirstOrDefault();
                            item.PhoneType = phone.PhoneType;
                            item.PhoneNumber = phone.PhoneNumber.GetCleanPhoneNumber();
                            item.IsPrimary = phone.IsPrimary;

                            _unitOfWork.Phones.Update(item);
                            continue;

                        }

                        _unitOfWork.Phones.Remove(item);
                        company.Phones.Remove(item);
                    }

                    //Add  Phones
                    foreach (var item in model.Phones.Where(x => x.PhoneId == 0).ToList())
                    {

                        Phone phone = new Phone();
                        phone.PhoneType = item.PhoneType;
                        phone.PhoneNumber = item.PhoneNumber.GetCleanPhoneNumber();
                        phone.IsPrimary = item.IsPrimary;
                        company.Phones.Add(phone);
                    }

                    _unitOfWork.Companies.Update(company);

                    if (updateEntity)
                    {
                        var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);
                        if (entity != null)
                        {
                            entity.Name = model.CompanyName;
                            _unitOfWork.Entities.Update(entity);
                        }
                    }

                    await _unitOfWork.CommitAsync();

                    var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                    var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(Convert.ToInt32(staff.OrganizationId));
                    // Company add/update operation
                    if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                    {
                        var entity = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);
                        int? sociableUserId = null;
                        sociableUserId = entity.SociableUserId;
                        if (sociableUserId != 0 || sociableUserId > 0)
                        {
                            var companyInfo = await _sociableService.GetUserById(Convert.ToInt32(entity.SociableUserId), staff.OrganizationId);
                            dynamic companyModel = JObject.Parse(companyInfo);
                            if (companyModel.uid == null)
                            {
                                sociableUserId = null;
                            }
                        }

                        if (sociableUserId == null)
                        {
                            int returnedSociableCompanyId = await _sociableService.CreatePerson(null, model, staff.OrganizationId);
                            var sociableCompanyId = returnedSociableCompanyId;
                            entity.SociableUserId = returnedSociableCompanyId;
                            sociableUserId = entity.SociableUserId;
                        }

                        if (entity.SociableUserId > 0)
                        {
                            var companyInfo = await _sociableService.GetUserById(Convert.ToInt32(entity.SociableUserId), staff.OrganizationId);
                            dynamic companyModel = JObject.Parse(companyInfo);
                            var companyProfileId = companyModel.profile_profiles[0].target_id;
                            if (companyProfileId > 0)
                            {

                                var result = await _sociableService.UpdatePersonProfile(null, model, (int)companyProfileId, staff.OrganizationId);
                            }
                            entity.SociableProfileId = companyProfileId;
                            _unitOfWork.Entities.Update(entity);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to update company:{ex.Message}");
                }
            }
            return false;
        }

        public async Task<bool> DeleteCompany(int id)
        {
            var company = await _unitOfWork.Companies.GetCompanyByIdAsync(id);
            if (company != null)
            {
                var companyContacts = await _unitOfWork.Persons.GetActiveCompanyPersonsByCompanyIdAsync(company.CompanyId);
                if (companyContacts != null && companyContacts.Count() > 0)
                {
                    return false;
                }
                if (company.Entity != null)
                {
                    if (company.Entity.Memberships != null && company.Entity.Memberships.Count > 0)
                    {
                        return false;
                    }
                    if (company.Entity.RelationEntities != null && company.Entity.RelationEntities.Count > 0)
                    {
                        return false;
                    }
                    if (company.Entity.Paymenttransactions != null && company.Entity.Paymenttransactions.Count > 0)
                    {
                        return false;
                    }
                    if (company.Entity.InvoiceEntities != null && company.Entity.InvoiceEntities.Count > 0)
                    {
                        return false;
                    }
                    if (company.Entity.Entityroles != null && company.Entity.Entityroles.Where(x => x.Status == (int)Status.Active).Count() > 0)
                    {
                        return false;
                    }
                    if (company.Entity.People != null && company.Entity.People.Count > 0)
                    {
                        return false;
                    }
                    if (company.Entity.Entityrolehistories != null && company.Entity.Entityrolehistories.Count > 0)
                    {
                        return false;
                    }
                    if (company.Entity.Contactactivities != null && company.Entity.Contactactivities.Count > 0)
                    {
                        return false;
                    }
                    _unitOfWork.Companies.Remove(company);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            return false;

        }

        public async Task<List<CompanyModel>> GetCompaniesByName(string name, string exceptMemberIds)
        {
            var companies = await _unitOfWork.Companies.GetCompaniesByName(name);
            var companyList = _mapper.Map<List<CompanyModel>>(companies);

            foreach (var company in companyList)
            {
                var primaryAddress = company.Addresses.GetPrimaryAddress();
                company.StreetAddress = primaryAddress.StreetAddress;
                company.City = primaryAddress.City == null ? String.Empty : primaryAddress.City;
                company.State = primaryAddress.State == null ? String.Empty : primaryAddress.State; ;
                company.Zip = primaryAddress.Zip.FormatZip();
                var primaryPhone = company.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                company.Phone = primaryPhone;
                var primaryEmail = company.Emails.GetPrimaryEmail();
                company.Email = primaryEmail;
                var relationToFindBillable = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(company.EntityId);
                if (relationToFindBillable != null)
                {
                    var billableContactChecked = relationToFindBillable.FirstOrDefault(s => s.Billable == true);
                    if (billableContactChecked != null && billableContactChecked.RelatedEntityId != null)
                    {
                        var billablePersonDetails = await _unitOfWork.Persons.GetPersonByEntityIdAsync(Convert.ToInt32(billableContactChecked.RelatedEntityId));
                        if (billablePersonDetails != null)
                        {
                            PersonModel personModel = new PersonModel();
                            personModel.FirstName = billablePersonDetails.FirstName;
                            personModel.LastName = billablePersonDetails.LastName;
                            personModel.EntityId = billablePersonDetails.EntityId != null ? Convert.ToInt32(billablePersonDetails.EntityId) : 0;
                            company.BillablePerson = personModel;
                        }
                        if (company.BillablePerson == null)
                        {
                            company.BillablePerson = new PersonModel();
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(exceptMemberIds))
            {
                var Ids = await GetExceptedMemberIds(exceptMemberIds);
                if (Ids != null && Ids.Count() > 0)
                {
                    companyList = companyList.Where(f => !Ids.Contains((int)f.EntityId)).ToList();
                }
            }

            return companyList;
        }
        public async Task<List<CompanyModel>> GetCompaniesByQuickSearchAsync(string searchParameter)
        {
            var companies = await _unitOfWork.Companies.GetCompaniesByQuickSearchAsync(searchParameter);
            var companyList = _mapper.Map<List<CompanyModel>>(companies);

            foreach (var company in companyList)
            {
                var primaryAddress = company.Addresses.GetPrimaryAddress();
                company.StreetAddress = primaryAddress.StreetAddress;
                company.City = primaryAddress.City == null ? String.Empty : primaryAddress.City;
                company.State = primaryAddress.State == null ? String.Empty : primaryAddress.State; ;
                company.Zip = primaryAddress.Zip.FormatZip();
                var primaryPhone = company.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                company.Phone = primaryPhone;
                var primaryEmail = company.Emails.GetPrimaryEmail();
                company.Email = primaryEmail;
                var relationToFindBillable = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(company.EntityId);
                if (relationToFindBillable != null)
                {
                    var billableContactChecked = relationToFindBillable.FirstOrDefault(s => s.Billable == true);
                    if (billableContactChecked != null && billableContactChecked.RelatedEntityId != null)
                    {
                        var billablePersonDetails = await _unitOfWork.Persons.GetPersonByEntityIdAsync(Convert.ToInt32(billableContactChecked.RelatedEntityId));
                        if (billablePersonDetails != null)
                        {
                            PersonModel personModel = new PersonModel();
                            personModel.FirstName = billablePersonDetails.FirstName;
                            personModel.LastName = billablePersonDetails.LastName;
                            personModel.EntityId = billablePersonDetails.EntityId != null ? Convert.ToInt32(billablePersonDetails.EntityId) : 0;
                            company.BillablePerson = personModel;
                        }
                        if (company.BillablePerson == null)
                        {
                            company.BillablePerson = new PersonModel();
                        }
                    }
                }
            }
            return companyList;
        }
        public async Task<CompanyProfileModel> GetCompanyProfileById(int id)
        {
            CompanyProfileModel model = new CompanyProfileModel();

            if (id <= 0) return model;

            var company = await _unitOfWork.Companies.GetCompanyByIdAsync(id);
            model = _mapper.Map<CompanyProfileModel>(company);

            //Get Credit Balance

            model.CreditBalance = 0;
            if (model.CreditTransactions.Count > 0)
            {
                var totalCredit = model.CreditTransactions.Where(x => x.EntryType == (int)CreditEntryType.CreditEntry).Sum(x => x.Amount);
                var totalDebit = model.CreditTransactions.Where(x => x.EntryType == (int)CreditEntryType.DebitEntry).Sum(x => x.Amount);
                model.CreditBalance = totalCredit - totalDebit;
            }

            return model;
        }

        public async Task<List<SelectListModel>> GetCompanyEmployeesById(int id)
        {
            List<SelectListModel> Employees = new List<SelectListModel>();

            var relations = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(id);

            foreach (var relation in relations)
            {
                SelectListModel item = new SelectListModel();
                item.name = relation.RelatedEntity.Name;
                item.code = relation.RelatedEntity.EntityId.ToString();
                Employees.Add(item);
            }

            return Employees;
        }

        public async Task<List<SelectListModel>> GetAllCompanies()
        {
            List<SelectListModel> companyList = new List<SelectListModel>();

            var companies = await _unitOfWork.Companies.GetAllCompaniesAsync();

            foreach (var company in companies)
            {
                SelectListModel item = new SelectListModel();
                item.name = company.CompanyName;
                item.code = company.CompanyId.ToString();
                companyList.Add(item);
            }

            return companyList;
        }

        public async Task<CompanyModel> GetCompanyByEmailIdAsync(string email)
        {
            var company = await _unitOfWork.Companies.GetCompanyByEmailIdAsync(email);
            var companyModel = _mapper.Map<CompanyModel>(company);
            return companyModel;

        }

        public async Task<CompanyModel> GetLastAddedCompanyAsync()
        {
            var company = await _unitOfWork.Companies.GetLastAddedCompanyAsync();
            var companyModel = _mapper.Map<CompanyModel>(company);
            return companyModel;

        }

        public async Task<AddressModel> GetPrimaryAddressByCompanyId(int companyid)
        {
            var addresses = await _unitOfWork.Addresses.GetAddressByCompanyIdAsync(companyid);

            var addressModel = _mapper.Map<List<AddressModel>>(addresses);

            return addressModel.GetPrimaryAddress();
        }

        private async Task<int[]> GetExceptedMemberIds(string exceptMemberIds)
        {
            int[] memberIds = new int[] { };
            if (!string.IsNullOrEmpty(exceptMemberIds))
            {
                memberIds = JsonSerializer.Deserialize<int[]>(exceptMemberIds);
                return memberIds;
            }
            return memberIds.ToArray();
        }

    }
}
