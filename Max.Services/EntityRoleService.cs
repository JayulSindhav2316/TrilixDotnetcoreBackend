using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Max.Core;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.Design;
using static Max.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Max.Data;

namespace Max.Services
{
    public class EntityRoleService : IEntityRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EntityRoleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Entityrole> CreateEntityRole(EntityRoleModel model)
        {
            Entityrole entityRole = new Entityrole();

            //Validate if the user has valid role asigned;

            var isValidRole = await ValidateRole(model);
            if (isValidRole)
            {
                entityRole.ContactRoleId = model.ContactRoleId;
                entityRole.EntityId = model.EntityId;
                entityRole.CompanyId = model.CompanyId;
                entityRole.EffectiveDate = model.EffectiveDate;
                entityRole.Status = (int)Status.Active;
                await _unitOfWork.EntityRoles.AddAsync(entityRole);
                await _unitOfWork.CommitAsync();

                //Add Audit log in EntityRoleHistory
                //TODO:AKS MVP says not required in Phase#1 but needs to be reviewed

                model.EntityRoleId = entityRole.EntityRoleId;
                await AddAssignRoleHistory(model);
                await AddRoleChangeContactActivity(model, entityRole.EntityRoleId, true);
            }

            return entityRole;
        }

        public async Task<bool> UpdateEntityRole(EntityRoleModel model)
        {
            Entityrole entityRole = await _unitOfWork.EntityRoles.GeActiveEntityRoleByIdAsync(model.EntityRoleId);

            if (entityRole != null)
            {

                entityRole.EffectiveDate = model.EffectiveDate;
                entityRole.Status = model.Status;
                entityRole.CompanyId = model.CompanyId;
                entityRole.EndDate = model.EndDate;
                _unitOfWork.EntityRoles.Update(entityRole);
                await _unitOfWork.CommitAsync();
                return true;

            }
            return false;
        }

        public async Task<List<Entityrole>> GetAllEntityRoles()
        {
            return (List<Entityrole>)await _unitOfWork.EntityRoles.GetAllEntityRolesAsync();
        }

        public async Task<List<Entityrole>> GetAllEntityRolesByEntityId(int entityId)
        {
            return (List<Entityrole>)await _unitOfWork.EntityRoles.GetAllEntityRolesByEntityIdAsync(entityId);
        }
        public async Task<List<ContactRoleModel>> GetEntityRolesByCompanyId(int companyId)
        {
            var contactRoles = new List<ContactRoleModel>();
            var roles = await _unitOfWork.EntityRoles.GetAllEntityRolesByCompanyIdAsync(companyId);
            foreach (var role in roles)
            {
                var contactRole = new ContactRoleModel();
                contactRole.ContactRoleId = role.ContactRole.ContactRoleId;
                contactRole.Name = role.ContactRole.Name;
                contactRole.Description = role.ContactRole.Description;
                if (!contactRoles.Any(x => x.Name == contactRole.Name))
                {
                    contactRoles.Add(contactRole);
                }
            }
            return contactRoles;
        }
        public async Task<List<Entityrole>> GetActiveEntityRolesByEntityId(int entityId)
        {
            var activeEntityRoles = await _unitOfWork.EntityRoles.GetActiveEntityRolesByEntityIdAsync(entityId);
            var entityRoles = new List<Entityrole>();
            if (activeEntityRoles != null)
            {
                foreach (var role in activeEntityRoles)
                {
                    role.EffectiveDate = role.EffectiveDate ?? EntityRole_MinDate;
                    entityRoles.Add(role);
                }
            }
            return entityRoles;
        }

        public async Task<List<SelectListModel>> GetActiveEntityRoleListByEntityId(int entityId)
        {
            var activeEntityRoles = await _unitOfWork.EntityRoles
                .GetActiveEntityRolesByEntityIdAsync(entityId);
            var entityRoles = new List<SelectListModel>();
            if (activeEntityRoles != null)
            {
                foreach (var role in activeEntityRoles)
                {
                    var model = new SelectListModel
                    {
                        code = role.ContactRoleId.ToString(),
                        name = role.ContactRole.Name.ToString()
                    };
                    entityRoles.Add(model);
                }
            }
            return entityRoles;
        }
        public async Task<List<AccountContactRoleModel>> GetActiveEntityRolesByCompanyId(int companyId)
        {
            var accountContacts = new List<AccountContactRoleModel>();
            var entityRoles = await _unitOfWork.EntityRoles.GetActiveEntityRolesByCompanyIdAsync(companyId);
            if (entityRoles != null)
            {
                foreach (var entityRole in entityRoles)
                {
                    var person = entityRole.Entity.People.FirstOrDefault();
                    if (person != null)
                    {
                        var personModel = _mapper.Map<PersonModel>(person);
                        var accountContact = new AccountContactRoleModel();
                        accountContact.EntityId = entityRole.EntityId ?? 0;
                        accountContact.Status = entityRole.Status ?? 0;
                        accountContact.EntityRoleId = entityRole.EntityRoleId;
                        accountContact.ContactRoleId = entityRole.ContactRoleId ?? 0;
                        accountContact.Role = entityRole.ContactRole.Name;
                        accountContact.FirstName = personModel.FirstName;
                        accountContact.LastName = personModel.LastName;
                        accountContact.EffectiveDate = entityRole.EffectiveDate ?? EntityRole_MinDate;
                        accountContact.EndDate = entityRole.EndDate ?? EntityRole_MinDate;
                        accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                        accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                        accountContact.AccountName = entityRole.Company.CompanyName;
                        accountContacts.Add(accountContact);
                    }

                }
            }
            return accountContacts;
        }

        public async Task<List<SelectListModel>> GetActiveEntityRoleListByCompanyId(int companyId)
        {
            var activeRoles = await _unitOfWork.EntityRoles
                .GetActiveEntityRolesByCompanyIdAsync(companyId);
            var entityRoles = new List<SelectListModel>();
            if (activeRoles != null)
            {
                foreach (var role in activeRoles)
                {
                    var model = new SelectListModel
                    {
                        code = role.ContactRoleId.ToString(),
                        name = role.ContactRole.Name.ToString()
                    };
                    entityRoles.Add(model);
                }
            }
            return entityRoles;
        }
        public async Task<List<AccountContactRoleModel>> GetContactsByRoleAndCompanyId(int roleId, int comapnyId)
        {
            var accountContacts = new List<AccountContactRoleModel>();
            var entityRoles = await _unitOfWork.EntityRoles.GetEntityByRoleAndCompanyIdAsync(roleId, comapnyId);
            if (entityRoles != null)
            {
                foreach (var entityRole in entityRoles)
                {
                    var person = entityRole.Entity.People.FirstOrDefault();
                    //bool hasHistoricData = true;
                    //if (entityRole.Status == (int)Status.InActive
                    //         && entityRole.ContactRoleId != 0
                    //         && entityRole.EntityId != null
                    //         && entityRole.CompanyId != null
                    //         && entityRole.EffectiveDate != null)
                    //{
                    //    var checkHistoricData = await _unitOfWork.ContactActivities
                    //        .GetContactActivityByEffectiveDateAndEndDateAsync(
                    //        Convert.ToInt32(entityRole.EntityId),
                    //        Convert.ToInt32(entityRole.CompanyId),
                    //        Convert.ToInt32(entityRole.ContactRoleId),
                    //        entityRole.EffectiveDate, entityRole.EndDate);
                    //    if (checkHistoricData.Any())
                    //    {
                    //        hasHistoricData = true;
                    //    }
                    //    else
                    //    {
                    //        hasHistoricData = false;
                    //    }
                    //}
                    //if (person != null && hasHistoricData)
                    if (person != null)
                    {
                        var personModel = _mapper.Map<PersonModel>(person);
                        var accountContact = new AccountContactRoleModel();
                        accountContact.EntityId = entityRole.EntityId ?? 0;
                        accountContact.Status = entityRole.Status ?? 0;
                        accountContact.EntityRoleId = entityRole.EntityRoleId;
                        accountContact.ContactRoleId = entityRole.ContactRoleId ?? 0;
                        accountContact.Role = entityRole.ContactRole.Name;
                        accountContact.FirstName = personModel.FirstName;
                        accountContact.LastName = personModel.LastName;
                        accountContact.EffectiveDate = entityRole.EffectiveDate ?? EntityRole_MinDate;
                        accountContact.EndDate = entityRole.EndDate;
                        accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                        accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                        accountContact.AccountName = entityRole.Company.CompanyName;
                        accountContacts.Add(accountContact);
                    }

                }
            }
            return accountContacts;
        }
        public async Task<List<AccountContactRoleModel>> GetContactsByRoleAndEntityId(int roleId, int entityId)
        {
            var accountContacts = new List<AccountContactRoleModel>();
            var currentPerson = await _unitOfWork.Persons.GetPersonByEntityIdAsync(entityId);
            if (currentPerson == null)
            {
                return accountContacts;
            }
            if (currentPerson.CompanyId > 0)
            {
                var entityRoles = await _unitOfWork.EntityRoles
                    .GetEntityByRoleAndCompanyIdAsync(roleId, currentPerson.CompanyId ?? 0);
                if (entityRoles != null)
                {
                    foreach (var entityRole in entityRoles)
                    {
                        var person = entityRole.Entity.People.FirstOrDefault();
                        if (person != null)
                        {
                            var personModel = _mapper.Map<PersonModel>(person);
                            var accountContact = new AccountContactRoleModel();
                            accountContact.EntityId = entityRole.EntityId ?? 0;
                            accountContact.Status = entityRole.Status ?? 0;
                            accountContact.EntityRoleId = entityRole.EntityRoleId;
                            accountContact.ContactRoleId = entityRole.ContactRoleId ?? 0;
                            accountContact.Role = entityRole.ContactRole.Name;
                            accountContact.FirstName = personModel.FirstName;
                            accountContact.LastName = personModel.LastName;
                            accountContact.EffectiveDate = entityRole.EffectiveDate ?? EntityRole_MinDate;
                            accountContact.EndDate = entityRole.EndDate ?? EntityRole_MinDate;
                            accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                            accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                            accountContact.AccountName = entityRole.Company.CompanyName;
                            accountContacts.Add(accountContact);
                        }

                    }
                }
            }
            return accountContacts;
        }
        public async Task<List<AccountContactRoleHistoryModel>> GetContactRoleHistoryByEntityId(int entityId)
        {
            var roleHistory = new List<AccountContactRoleHistoryModel>();

            //Get current roles

            var entityRoles = await _unitOfWork.EntityRoles.GetEntityRolesByEntityIdAsync(entityId);
            if (entityRoles != null)
            {
                foreach (var entityRole in entityRoles)
                {
                    //bool hasHistoricData = true;
                    //if (entityRole.Status == (int)Status.InActive
                    //    && entityRole.ContactRoleId != 0
                    //    && entityRole.EntityId != null
                    //    && entityRole.CompanyId != null
                    //    && entityRole.EffectiveDate != null)
                    //{
                    //    var checkHistoricData = await _unitOfWork.ContactActivities
                    //        .GetContactActivityByEffectiveDateAndEndDateAsync(
                    //        Convert.ToInt32(entityRole.EntityId),
                    //        Convert.ToInt32(entityRole.CompanyId),
                    //        Convert.ToInt32(entityRole.ContactRoleId),
                    //        entityRole.EffectiveDate, entityRole.EndDate);
                    //    if (checkHistoricData.Any())
                    //    {
                    //        hasHistoricData = true;
                    //    }
                    //    else
                    //    {
                    //        hasHistoricData = false;
                    //    }
                    //}
                    //if (hasHistoricData)
                    //{
                    var accountContact = new AccountContactRoleHistoryModel();
                    accountContact.EntityId = entityRole.EntityId ?? 0;
                    accountContact.Status = entityRole.Status ?? 0;
                    accountContact.EntityRoleId = entityRole.EntityRoleId;
                    accountContact.ContactRoleId = entityRole.ContactRole.ContactRoleId;
                    accountContact.AccountId = entityRole.Company?.EntityId ?? 0;
                    accountContact.AccountName = entityRole.Company?.CompanyName;
                    accountContact.Role = entityRole.ContactRole.Name;
                    accountContact.EffectiveDate = entityRole.EffectiveDate ?? EntityRole_MinDate;
                    accountContact.EndDate = entityRole.EndDate;
                    roleHistory.Add(accountContact);
                    //}
                }
            }


            var entityRoleHistories = await _unitOfWork.EntityRoleHistories
                .GetAllEntityRoleHistoryByEntityIdAsync(entityId);
            entityRoleHistories = entityRoleHistories.DistinctBy(x => x.ContactRoleId).ToList();
            if (entityRoleHistories != null)
            {
                foreach (var entityRole in entityRoleHistories)
                {
                    if (!entityRoles.Any(x => x.ContactRoleId == entityRole.ContactRoleId))
                    {
                        var contactRole = entityRole.Entity.Entityroles
                            .FirstOrDefault(x => x.ContactRoleId == entityRole.ContactRoleId);
                        var accountContact = new AccountContactRoleHistoryModel();
                        accountContact.EntityId = entityRole.EntityId ?? 0;
                        accountContact.Status = contactRole?.Status ?? 0;
                        accountContact.EntityRoleHistoryId = entityRole.EntityRoleHistoryId;
                        accountContact.EntityRoleId = contactRole.EntityRoleId;
                        accountContact.ContactRoleId = entityRole.ContactRoleId ?? 0;
                        accountContact.Role = entityRole.ContactRole.Name;
                        accountContact.AccountId = entityRole.Company.EntityId ?? 0;
                        accountContact.AccountName = entityRole.Company.CompanyName;
                        accountContact.EffectiveDate = contactRole?.EffectiveDate ?? EntityRole_MinDate;
                        accountContact.EndDate = contactRole?.EndDate;
                        roleHistory.Add(accountContact);
                    }
                }
            }
            return roleHistory;
        }

        public async Task<List<AccountContactRoleHistoryModel>> GetActiveContactRolesByEntityId(int entityId)
        {
            var roleHistory = new List<AccountContactRoleHistoryModel>();
            //Get current roles
            var entityRoles = await _unitOfWork.EntityRoles
                .GetActiveEntityRolesByEntityIdAsync(entityId);
            if (entityRoles != null)
            {
                foreach (var entityRole in entityRoles)
                {
                    var accountContact = new AccountContactRoleHistoryModel();
                    accountContact.EntityId = entityRole.EntityId ?? 0;
                    accountContact.Status = entityRole.Status ?? 0;
                    accountContact.EntityRoleId = entityRole.EntityRoleId;
                    accountContact.ContactRoleId = entityRole.ContactRole.ContactRoleId;
                    accountContact.AccountId = entityRole.Company.EntityId ?? 0;
                    accountContact.AccountName = entityRole.Company.CompanyName;
                    accountContact.Role = entityRole.ContactRole.Name;
                    accountContact.EffectiveDate = entityRole.EffectiveDate ?? EntityRole_MinDate;
                    accountContact.EndDate = entityRole.EndDate ?? EntityRole_MinDate;
                    roleHistory.Add(accountContact);
                }
            }
            return roleHistory;
        }

        public async Task<List<AccountContactModel>> GetContactsByFirstAndLastName(string firstName, string lastName, int companyId)
        {
            var accountContacts = new List<AccountContactModel>();
            var persons = await _unitOfWork.Persons
                .GetCompanyPersonsByFirstAndLastNameAsync(firstName, lastName, companyId);

            if (persons != null)
            {
                foreach (var person in persons)
                {
                    if (!accountContacts.Any(x => x.EntityId == person.EntityId))
                    {
                        if (person != null)
                        {
                            var personModel = _mapper.Map<PersonModel>(person);
                            var accountContact = new AccountContactModel();
                            accountContact.EntityId = person.EntityId ?? 0;
                            accountContact.FirstName = personModel.FirstName;
                            accountContact.LastName = personModel.LastName;
                            accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                            accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                            if (person.Entity.Entityroles.Count > 0)
                            {
                                var entityRoles = person.Entity.Entityroles;
                                var roles = entityRoles.Select(x => x.ContactRole.Name).ToList();
                                accountContact.Roles = string.Join(",", roles);

                                //Create a list for display in array

                                foreach (var role in entityRoles)
                                {
                                    accountContact.EntityRoles.Add(role.ContactRole.Name);
                                }
                            }
                            accountContacts.Add(accountContact);
                        }
                    }
                }
            }
            return accountContacts;
        }

        public async Task<List<AccountContactModel>> GetContactsByName(string name, int companyId)
        {
            var accountContacts = new List<AccountContactModel>();
            var persons = await _unitOfWork.Persons
                .GetCompanyPersonsByNameAsync(name, companyId);

            if (persons != null)
            {
                foreach (var person in persons)
                {
                    if (!accountContacts.Any(x => x.EntityId == person.EntityId))
                    {
                        if (person != null)
                        {
                            var personModel = _mapper.Map<PersonModel>(person);
                            var accountContact = new AccountContactModel();
                            accountContact.EntityId = person.EntityId ?? 0;
                            accountContact.FirstName = personModel.FirstName;
                            accountContact.LastName = personModel.LastName;
                            accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                            accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                            if (person.Entity.Entityroles.Count > 0)
                            {
                                var entityRoles = person.Entity.Entityroles.Where(s => s.IsDeleted == (int)Status.InActive || s.IsDeleted == null);
                                var roles = entityRoles.Select(x => x.ContactRole.Name).ToList();
                                accountContact.Roles = string.Join(",", roles);

                                //Create a list for display in array

                                foreach (var role in entityRoles)
                                {
                                    accountContact.EntityRoles.Add(role.ContactRole.Name);
                                }
                            }
                            accountContacts.Add(accountContact);
                        }
                    }
                }
            }
            return accountContacts;
        }

        public async Task<List<AccountContactRoleModel>> GetContactsByCompanyId(int companyId)
        {
            var accountContacts = new List<AccountContactRoleModel>();
            var persons = await _unitOfWork.Persons.GetCompanyPersonsByCompanyIdAsync(companyId);
            if (persons != null)
            {
                foreach (var person in persons)
                {
                    if (!accountContacts.Any(x => x.EntityId == person.EntityId))
                    {
                        if (person != null)
                        {
                            var personModel = _mapper.Map<PersonModel>(person);
                            var accountContact = new AccountContactRoleModel();
                            accountContact.EntityId = person.EntityId ?? 0;
                            accountContact.FirstName = personModel.FirstName;
                            accountContact.LastName = personModel.LastName;
                            accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                            accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                            if (person.Entity.Entityroles.Count > 0)
                            {
                                //Add one row each for every role
                                foreach (var role in person.Entity.Entityroles)
                                {
                                    accountContact.Role = role.ContactRole.Name;
                                    accountContacts.Add(accountContact);
                                }

                            }
                            else
                            {
                                accountContacts.Add(accountContact);
                            }
                        }
                    }
                }
            }
            return accountContacts;
        }
        public async Task<List<AccountContactRoleModel>> GetAccountContactsByEntityId(int entityId)
        {
            var accountContacts = new List<AccountContactRoleModel>();

            //Check if entiy is a company or person
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(entityId);
            if (entity != null)
            {
                var companyId = 0;
                if (entity.CompanyId > 0)
                {
                    companyId = entity.CompanyId ?? 0;
                }
                else
                {
                    companyId = entity.People.Select(x => x.CompanyId).FirstOrDefault() ?? 0;
                }

                if (companyId <= 0)
                {
                    return accountContacts;
                }
                var persons = await _unitOfWork.Persons
                    .GetActiveCompanyPersonsByCompanyIdAsync(companyId);
                if (persons != null)
                {
                    foreach (var person in persons)
                    {
                        if (person.EntityId != entityId)
                        {
                            if (person != null)
                            {

                                if (person.Entity.Entityroles.Count > 0)
                                {
                                    //Add one row each for every role
                                    foreach (var role in person.Entity.Entityroles)
                                    {
                                        var personModel = _mapper.Map<PersonModel>(person);
                                        var accountContact = new AccountContactRoleModel();
                                        accountContact.EntityId = person.EntityId ?? 0;
                                        accountContact.FirstName = personModel.FirstName;
                                        accountContact.LastName = personModel.LastName;
                                        accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                                        accountContact.Role = role.ContactRole.Name;
                                        accountContact.EntityRoleId = role.EntityRoleId;
                                        accountContact.ContactRoleId = role.ContactRole.ContactRoleId;
                                        accountContact.AccountId = person.Company.EntityId ?? 0;
                                        accountContact.AccountName = person.Company.CompanyName;
                                        accountContacts.Add(accountContact);
                                    }

                                }
                                else
                                {
                                    var personModel = _mapper.Map<PersonModel>(person);
                                    var accountContact = new AccountContactRoleModel();
                                    accountContact.EntityId = person.EntityId ?? 0;
                                    accountContact.FirstName = personModel.FirstName;
                                    accountContact.LastName = personModel.LastName;
                                    accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                    accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                                    accountContacts.Add(accountContact);
                                }
                            }
                        }
                    }
                }
            }


            return accountContacts.OrderBy(x => x.LastName)
                     .ThenBy(x => x.FirstName)
                     .ThenBy(x => x.Role)
                     .ToList();
        }

        public async Task<List<AccountContactRoleModel>> GetActiveAccountContactsByEntityId(int entityId)
        {
            var accountContacts = new List<AccountContactRoleModel>();
            var isContact = true;
            List<Entityrole> entityRoles = new List<Entityrole>();
            //Check if it is company or person

            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);
            if (entity == null)
            {
                return accountContacts;
            }

            if (entity.CompanyId.HasValue)
            {
                isContact = false;
                entityRoles = await _unitOfWork.EntityRoles
                    .GetActiveEntityRolesByCompanyIdAsync(entity.CompanyId ?? 0);

            }
            else
            {
                var primaryPerson = await _unitOfWork.Persons.GetPersonByEntityIdAsync(entityId);
                entityRoles = await _unitOfWork.EntityRoles
                    .GetActiveEntityRolesByCompanyIdAsync(primaryPerson.CompanyId ?? 0);

            }

            if (entityRoles != null)
            {
                foreach (var entityRole in entityRoles)
                {
                    if (isContact && entityRole.EntityId == entityId)
                    {
                        continue;
                    }
                    var person = entityRole.Entity.People.FirstOrDefault();
                    if (person != null)
                    {
                        var personModel = _mapper.Map<PersonModel>(person);
                        var accountContact = new AccountContactRoleModel();
                        accountContact.EntityId = entityRole.EntityId ?? 0;
                        accountContact.Status = entityRole.Status ?? 0;
                        accountContact.EntityRoleId = entityRole.ContactRoleId ?? 0;
                        accountContact.Role = entityRole.ContactRole.Name;
                        accountContact.FirstName = personModel.FirstName;
                        accountContact.LastName = personModel.LastName;
                        accountContact.EffectiveDate = entityRole.EffectiveDate ?? EntityRole_MinDate;
                        accountContact.EndDate = entityRole.EndDate ?? EntityRole_MinDate;
                        accountContact.PrimaryPhone = personModel.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                        accountContact.PrimaryEmail = personModel.Emails.GetPrimaryEmail();
                        accountContacts.Add(accountContact);
                    }

                }
            }
            return accountContacts;
        }
        public async Task<Entityrole> GetEntityRoleById(int id)
        {
            return await _unitOfWork.EntityRoles.GetByIdAsync(id);
        }

        private async Task<bool> ValidateRole(EntityRoleModel model)
        {
            var entityRoleList = await _unitOfWork.EntityRoles.GetActiveEntityRolesByEntityIdAsync(model.EntityId);
            if (entityRoleList != null)
            {
                if (entityRoleList.Any(x => x.ContactRoleId == model.ContactRoleId && x.CompanyId == model.CompanyId))
                {
                    throw new InvalidOperationException($"Duplicate Contact Role assignment.");
                }
            }
            return true;
        }

        public async Task<bool> UnassignEntityRole(EntityRoleModel model)
        {
            Entityrole entityRole = await _unitOfWork.EntityRoles
                .GeActiveEntityRoleByIdAsync(model.EntityRoleId);

            if (entityRole != null)
            {
                entityRole.EndDate = model.EndDate;
                entityRole.Status = model.Status;
                _unitOfWork.EntityRoles.Update(entityRole);
                await _unitOfWork.CommitAsync();
                //if (model.HaveHistoricRecords)
                //{
                await AddUnassignRoleHistory(entityRole, model, (int)Status.Active);
                await AddRoleChangeContactActivity(model, entityRole.EntityRoleId, false);

                await RemoveContactActivityByRole(entityRole, model);

                //var getRoleByEntity = await GetActiveEntityRolesByEntityId(model.EntityId);
                //if (!getRoleByEntity.Any())
                //{
                //await RemoveRoleContactActivity(entityRole, model);
                //}
                //}
                //else
                //{
                //await AddUnassignRoleHistory(entityRole, model, (int)Status.InActive);
                //await UpdateRoleChangeContactActivity(entityRole.EntityRoleId);
                //await UpdateAssignedRoleHistory(entityRole.EntityId ?? 0, entityRole.CompanyId ?? 0, AuditEntrtyType.CREATED);
                //}
                return true;
            }
            return false;
        }

        public async Task<bool> AddAssignRoleHistory(EntityRoleModel model)
        {
            if (model != null)
            {
                var entityRole = await _unitOfWork.EntityRoles.GeActiveEntityRoleByIdAsync(model.EntityRoleId);
                Entityrolehistory roleHistory = new Entityrolehistory();
                roleHistory.ActivityDate = model.EffectiveDate;
                roleHistory.ActivityType = Constants.AuditEntrtyType.CREATED;
                roleHistory.StaffUserId = model.StaffUserId;
                roleHistory.CompanyId = model.CompanyId;
                roleHistory.EntityId = model.EntityId;
                roleHistory.ContactRoleId = model.ContactRoleId;
                roleHistory.Status = (int)Status.Active;
                var person = entityRole.Entity.People.FirstOrDefault();
                roleHistory.Description = $"Assigned {person.FirstName} {person.LastName} to {entityRole.ContactRole?.Name} role at {entityRole.Company?.CompanyName}.";
                await _unitOfWork.EntityRoleHistories.AddAsync(roleHistory);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> AddUnassignRoleHistory(Entityrole entityRole, EntityRoleModel model, int status)
        {
            if (entityRole != null)
            {
                Entityrolehistory entityRoleHistory = new Entityrolehistory();
                entityRoleHistory.EntityId = entityRole.EntityId;
                entityRoleHistory.CompanyId = entityRole.CompanyId;
                entityRoleHistory.ContactRoleId = entityRole.ContactRoleId;
                entityRoleHistory.StaffUserId = model.StaffUserId;
                var person = entityRole.Entity.People.FirstOrDefault();
                entityRoleHistory.Description = $"Unassigned {person.FirstName} {person.LastName} from {entityRole.ContactRole?.Name} role at {entityRole.Company?.CompanyName}.";
                entityRoleHistory.ActivityType = AuditEntrtyType.DELETED;
                entityRoleHistory.ActivityDate = model.EndDate;
                entityRoleHistory.Status = status;
                await _unitOfWork.EntityRoleHistories.AddAsync(entityRoleHistory);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> AddRoleChangeContactActivity(EntityRoleModel model, int entityRoleId, bool isRoleAssigned)
        {
            if (entityRoleId > 0)
            {
                var entityRole = await _unitOfWork.EntityRoles.GetEntityRoleByIdAsync(entityRoleId);
                Contactactivity contactActivity = new Contactactivity();
                contactActivity.EntityId = entityRole.EntityId;
                contactActivity.AccountId = entityRole.CompanyId;
                contactActivity.ActivityConnection = (int)ContactActivityConnectionType.RoleOnly;
                contactActivity.ActivityDate = model.EffectiveDate;
                contactActivity.InteractionType = (int)ContactActivityInteractionType.RoleChange;
                contactActivity.StaffUserId = model.StaffUserId ?? null;
                contactActivity.ContactRoleId = entityRole.ContactRoleId;

                var person = entityRole.Entity.People.FirstOrDefault();
                contactActivity.Subject = $"Role Change for { person?.FirstName} { person?.LastName}";
                if (isRoleAssigned)
                {
                    contactActivity.Status = (int)Status.Active;
                    contactActivity.Description = $"{person?.FirstName} {person?.LastName} assigned to {entityRole.ContactRole.Name} role at {entityRole.Company?.CompanyName}";
                }
                else
                {
                    contactActivity.ActivityDate = model.EndDate;
                    //if (haveHistoricRecords)
                    //{
                    contactActivity.Status = (int)Status.Active;
                    //}
                    //else
                    //{
                    //    contactActivity.Status = (int)Status.InActive;
                    //}
                    contactActivity.Description = $"{person?.FirstName} {person?.LastName} unassigned from {entityRole.ContactRole.Name} role at {entityRole.Company?.CompanyName}";

                }
                await _unitOfWork.ContactActivities.AddAsync(contactActivity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveRoleContactActivity(Entityrole entityRole, EntityRoleModel model)
        {
            if (entityRole != null)
            {
                entityRole.EffectiveDate = entityRole.EffectiveDate.HasValue ? entityRole.EffectiveDate.Value : EntityRole_MinDate;
                var result = await _unitOfWork.ContactActivities
                    .GetContactActivityByActivityDateAsync(entityRole.EntityId ?? 0, entityRole.EffectiveDate.Value);
                result = result.Where(x => x.AccountId == entityRole.CompanyId.Value);
                var assignedRoleActivities = result.Where(x => x.InteractionType == (int)ContactActivityInteractionType.RoleChange);
                var data = result.Where(x => x.ActivityConnection == (int)ContactActivityConnectionType.RoleContact).ToList();
                var finalData = data.Except(assignedRoleActivities);
                if (finalData.Any())
                {
                    finalData.ToList().ForEach(s => s.IsDeleteforPerson = 1);
                    finalData.ToList().ForEach(s => s.IsHistoricalDelete = 1);
                }
                foreach (var item in finalData)
                {
                    _unitOfWork.ContactActivities.Update(item);
                }
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }


        public async Task<bool> RemoveContactActivityByRole(Entityrole entityRole, EntityRoleModel model)
        {
            if (entityRole != null)
            {
                entityRole.EffectiveDate = entityRole.EffectiveDate.HasValue ? entityRole.EffectiveDate.Value : EntityRole_MinDate;
                var result = await _unitOfWork.ContactActivities
                    .GetContactActivityByActivityDateAsync(entityRole.EntityId ?? 0, entityRole.EffectiveDate.Value);
                result = result.Where(x => x.AccountId == entityRole.CompanyId.Value
                && x.Contactactivityinteractions.Any(m => m.InteractionRoleId == entityRole.ContactRoleId));
                var assignedRoleActivities = result
                    .Where(x => x.InteractionType == (int)ContactActivityInteractionType.RoleChange);
                var data = result.Where(x => x.ActivityConnection == (int)ContactActivityConnectionType.RoleContact).ToList();
                var finalData = data.Except(assignedRoleActivities);
                if (finalData.Any())
                {
                    foreach (var activity in finalData)
                    {
                        var contactActivityInteraction = activity.Contactactivityinteractions
                            .Where(x => x.InteractionRoleId == entityRole.ContactRoleId
                            && x.InteractionEntityId == entityRole.EntityId)
                            .FirstOrDefault();
                        contactActivityInteraction.IsDeleted = (int)ActivityDeleteStatus.Deleted;
                        _unitOfWork.ContactActivityInteractions.Update(contactActivityInteraction);
                        if (activity.Contactactivityinteractions
                            .Any(x => x.IsDeleted == (int)ActivityDeleteStatus.NotDeleted
                            || x.IsDeleted == null))
                        {
                            activity.IsDeleteforPerson = (int)ActivityDeleteStatus.PartialDeleted;
                        }
                        else
                        {
                            activity.IsDeleteforPerson = (int)ActivityDeleteStatus.Deleted;
                        }
                        activity.IsHistoricalDelete = (int)ActivityDeleteStatus.Deleted;
                        _unitOfWork.ContactActivities.Update(activity);
                    }
                    await _unitOfWork.CommitAsync();
                }
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAssignedRoleHistory(int entityId, int companyId, string type)
        {
            if (entityId != 0)
            {
                var entityHistories = await _unitOfWork.EntityRoleHistories
                    .GetEntityRoleHistoryByTypeAsync(entityId, companyId, type);
                if (entityHistories.Count() > 0)
                {
                    var entityHistory = entityHistories.FirstOrDefault();
                    entityHistory.Status = 0;
                    _unitOfWork.EntityRoleHistories.Update(entityHistory);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> UpdateRoleChangeContactActivity(int entityRoleId)
        {
            if (entityRoleId > 0)
            {

                var entityRole = await _unitOfWork.EntityRoles.GetEntityRoleByIdAsync(entityRoleId);
                var contactActivities = await _unitOfWork.ContactActivities
                    .GetContactActivityByAccountAndEntityIdAsync(
                    entityRole.CompanyId ?? 0, entityRole.EntityId ?? 0);
                contactActivities = contactActivities
                    .Where(x => x.InteractionType == (int)ContactActivityInteractionType.RoleChange)
                    .OrderByDescending(s => s.ContactActivityId);
                if (contactActivities.Count() > 0)
                {
                    var contactActivity = contactActivities.FirstOrDefault();
                    contactActivity.Status = (int)Status.InActive;
                    _unitOfWork.ContactActivities.Update(contactActivity);
                    await _unitOfWork.CommitAsync();
                }
            }
            return true;
        }

        public async Task<bool> UpdateEntityRoleEffectiveDates(EntityRoleModel entityRoleModel)
        {
            bool isSuccess = false;
            var entityRoleDetails = await _unitOfWork.EntityRoles.GetEntityRoleByIdAsync(entityRoleModel.EntityRoleId);
            var oldEffectiveDate = entityRoleDetails.EffectiveDate;
            var oldEndDate = entityRoleDetails.EndDate;
            if (entityRoleDetails != null)
            {
                entityRoleDetails.EffectiveDate = entityRoleModel.EffectiveDate;
                entityRoleDetails.EndDate = entityRoleModel.EndDate;
                _unitOfWork.EntityRoles.Update(entityRoleDetails);

                if (entityRoleDetails.ContactRoleId != null
                    && entityRoleDetails.CompanyId != null
                    && entityRoleDetails.EntityId != null
                    && oldEffectiveDate != null)
                {
                    var contactRoleDetails = await _unitOfWork.ContactRoles
                        .GetByIdAsync(Convert.ToInt32(entityRoleDetails.ContactRoleId));
                    if (contactRoleDetails != null)
                    {
                        var roleAssignContactActivities = await _unitOfWork.ContactActivities
                            .GetAccountActivityByEntityAccountDateAndRole(
                            Convert.ToInt32(entityRoleDetails.CompanyId),
                            Convert.ToInt32(entityRoleDetails.EntityId),
                            contactRoleDetails.Name, Convert.ToDateTime(oldEffectiveDate), true);
                        if (roleAssignContactActivities.Any())
                        {
                            foreach (var item in roleAssignContactActivities)
                            {
                                var contactActivity = await _unitOfWork.ContactActivities
                                    .GetByIdAsync(item.ContactActivityId);
                                if (contactActivity != null)
                                {
                                    contactActivity.ActivityDate = entityRoleModel.EffectiveDate;
                                    _unitOfWork.ContactActivities.Update(item);
                                }
                            }
                        }

                        if (oldEndDate != null)
                        {
                            var roleUnAssignContactActivities = await _unitOfWork.ContactActivities
                                    .GetAccountActivityByEntityAccountDateAndRole(
                                     Convert.ToInt32(entityRoleDetails.CompanyId),
                                     Convert.ToInt32(entityRoleDetails.EntityId),
                                     contactRoleDetails.Name,
                                     Convert.ToDateTime(oldEndDate), false);
                            if (roleUnAssignContactActivities.Any())
                            {
                                var activity = roleUnAssignContactActivities.FirstOrDefault();
                                var contactActivity = await _unitOfWork.ContactActivities.GetByIdAsync(activity.ContactActivityId);
                                if (contactActivity != null)
                                {
                                    contactActivity.ActivityDate = entityRoleModel.EndDate;
                                    _unitOfWork.ContactActivities.Update(activity);
                                }
                            }
                        }
                    }
                }

                await _unitOfWork.CommitAsync();
                isSuccess = true;
            }
            else
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public async Task<bool> AccountChangeRoleChangeOperation(List<EntityRoleModel> entityRoleModels)
        {
            try
            {
                var isSuccess = true;
                foreach (var item in entityRoleModels)
                {
                    var activities = await _unitOfWork.ContactActivities
                        .GetContactActivityByActivityDateAsync(item.EntityId, item.EffectiveDate);
                    if (activities.Any())
                    {
                        item.HaveHistoricRecords = true;
                    }
                    else
                    {
                        item.HaveHistoricRecords = false;
                    }
                    item.Status = (int)Status.InActive;

                    await UnassignEntityRole(item);
                }

                return isSuccess;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAssignment(EntityRoleModel model)
        {
            Entityrole entityRole = await _unitOfWork.EntityRoles
                .GetEntityRoleByIdAsync(model.EntityRoleId);

            if (entityRole != null)
            {
                entityRole.IsDeleted = (int)Status.Active;
                _unitOfWork.EntityRoles.Update(entityRole);
                await _unitOfWork.CommitAsync();

                await DeleteRoleHistory(entityRole);
                await DeleteRoleContactActivities(entityRole);

                return true;
            }
            return false;
        }

        public async Task<bool> DeleteRoleHistory(Entityrole model)
        {
            if (model.EntityId != 0)
            {
                var entityHistories = await _unitOfWork.EntityRoleHistories
                    .GetEntityRoleHistoryAsync(model.EntityId ?? 0, model.CompanyId ?? 0, model.ContactRoleId ?? 0);
                if (entityHistories.Any())
                {
                    foreach (var entityRoleHistory in entityHistories)
                    {
                        entityRoleHistory.IsDeleted = (int)Status.Active;
                        _unitOfWork.EntityRoleHistories.Update(entityRoleHistory);
                    }
                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteRoleContactActivities(Entityrole model)
        {
            if (model.EntityId != 0)
            {
                var contactActivities = await _unitOfWork.ContactActivities
                    .GetRoleAssignmentActivities(model.EntityId ?? 0, model.CompanyId ?? 0, model.ContactRoleId ?? 0);
                if (contactActivities.Any())
                {
                    foreach (var contactactivity in contactActivities)
                    {
                        contactactivity.IsDeleted = (int)Status.Active;
                        _unitOfWork.ContactActivities.Update(contactactivity);
                    }
                    await _unitOfWork.CommitAsync();
                    return true;
                }
            }
            return false;
        }
        public async Task<List<SelectListModel>> GetContactSelectList(int accountId)
        {
            var contacts = await _unitOfWork.Persons.GetCompanyPersonsByCompanyIdAsync(accountId);
            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var contact in contacts.OrderBy(x => x.FirstName).ThenBy(x => x.LastName))
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = contact.EntityId.ToString();
                selectListItem.name = $"{contact.FirstName} {contact.LastName}";
                selectList.Add(selectListItem);
            }
            return selectList;
        }
    }
}
