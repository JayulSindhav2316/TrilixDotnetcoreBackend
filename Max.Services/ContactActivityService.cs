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

namespace Max.Services
{
    public class ContactActivityService : IContactActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEntityService _entityService;
        private readonly IEntityRoleService _entityRoleService;

        public ContactActivityService(IUnitOfWork unitOfWork, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IEntityService entityService, IEntityRoleService entityRoleService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _entityService = entityService;
            this._entityRoleService = entityRoleService;
        }
        public async Task<Contactactivity> CreateContactActivity(ContactActivityInputModel model)
        {
            Contactactivity contactActivity = new Contactactivity();
            if (model != null)
            {
                contactActivity = _mapper.Map<Contactactivity>(model);
                contactActivity.Status = (int)Status.Active;
                contactActivity.AccountId = contactActivity.AccountId > 0 ? contactActivity.AccountId : null;
                await _unitOfWork.ContactActivities.AddAsync(contactActivity);
                await _unitOfWork.CommitAsync();

                if (model.InteractionContactDetails.Count > 0)
                {
                    for (int i = 0; i < model.InteractionContactDetails.Count; i++)
                    {
                        if (model.InteractionContactDetails[i].ContactRoleList.Count > 0)
                        {
                            for (int j = 0; j < model.InteractionContactDetails[i].ContactRoleList.Count; j++)
                            {
                                Contactactivityinteraction activityInteraction = new Contactactivityinteraction();
                                activityInteraction.InteractionAccountId = model.InteractionContactDetails[i].AccountId > 0 ? model.AccountId : null;
                                activityInteraction.InteractionEntityId = model.InteractionContactDetails[i].EntityId > 0 ? model.InteractionContactDetails[i].EntityId : null;
                                activityInteraction.InteractionRoleId = model.InteractionContactDetails[i].ContactRoleList[j] > 0 ? model.InteractionContactDetails[i].ContactRoleList[j] : null;
                                activityInteraction.ContactActivityId = contactActivity.ContactActivityId;
                                await _unitOfWork.ContactActivityInteractions.AddAsync(activityInteraction);
                                await _unitOfWork.CommitAsync();
                            }
                        }
                        else
                        {
                            Contactactivityinteraction activityInteraction = new Contactactivityinteraction();
                            activityInteraction.InteractionAccountId = model.InteractionContactDetails[i].AccountId > 0 ? model.AccountId : null;
                            activityInteraction.InteractionEntityId = model.InteractionContactDetails[i].EntityId;
                            activityInteraction.InteractionRoleId = null;
                            activityInteraction.ContactActivityId = contactActivity.ContactActivityId;
                            await _unitOfWork.ContactActivityInteractions.AddAsync(activityInteraction);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                }
                //if (model.InteractionAccountList.Count > 0)
                //{
                //    for (int i = 0; i < model.InteractionAccountList.Count; i++)
                //    {
                //        Contactactivityinteraction activityInteraction = new Contactactivityinteraction();
                //        if (model.InteractionAccountList[i] == 0)
                //        {
                //            activityInteraction.InteractionAccountId = null;
                //        }
                //        else
                //        {
                //            activityInteraction.InteractionAccountId = model.InteractionAccountList[i];
                //        }
                //        activityInteraction.InteractionEntityId = model.InteractionEntityList.Count >= i ? model.InteractionEntityList[i] : null;
                //        activityInteraction.ContactActivityId = contactActivity.ContactActivityId;
                //        await _unitOfWork.ContactActivityInteractions.AddAsync(activityInteraction);
                //        await _unitOfWork.CommitAsync();

                //    }
                //}
            }
            return contactActivity;
        }

        public async Task<bool> UpdateContactActivity(ContactActivityInputModel model)
        {
            Contactactivity activity = await _unitOfWork.ContactActivities.GetContactActivityByIdAsync(model.ContactActivityId);

            if (activity != null)
            {

                activity.AccountId = activity.AccountId > 0 ? activity.AccountId : null;
                activity.EntityId = model.EntityId;
                activity.ActivityDate = model.ActivityDate;
                activity.InteractionType = model.InteractionType;
                activity.ActivityConnection = model.ActivityConnection;
                activity.Subject = model.Subject;
                activity.Description = model.Description;
                activity.StaffUserId = model.StaffUserId;
                activity.ModificationDate = model.ModificationDate;

                _unitOfWork.ContactActivities.Update(activity);
                //_unitOfWork.ContactActivityInteractions.RemoveRange(activity.Contactactivityinteractions);
                await _unitOfWork.CommitAsync();

                #region commented code for future reference
                //if (model.InteractionContactDetails.Count > 0)
                //{
                //    for (int i = 0; i < model.InteractionContactDetails.Count; i++)
                //    {
                //        if (model.InteractionContactDetails[i].ContactRoleList.Count > 0)
                //        {
                //            for (int j = 0; j < model.InteractionContactDetails[i].ContactRoleList.Count; j++)
                //            {
                //                Contactactivityinteraction activityInteraction = new Contactactivityinteraction();
                //                activityInteraction.InteractionAccountId = model.InteractionContactDetails[i].AccountId > 0 ? model.AccountId : null;
                //                activityInteraction.InteractionEntityId = model.InteractionContactDetails[i].EntityId > 0 ? model.InteractionContactDetails[i].EntityId : null;
                //                activityInteraction.InteractionRoleId = model.InteractionContactDetails[i].ContactRoleList[j] > 0 ? model.InteractionContactDetails[i].ContactRoleList[j] : null;
                //                activityInteraction.ContactActivityId = activity.ContactActivityId;
                //                await _unitOfWork.ContactActivityInteractions.AddAsync(activityInteraction);
                //                await _unitOfWork.CommitAsync();
                //            }
                //        }
                //        else
                //        {
                //            Contactactivityinteraction activityInteraction = new Contactactivityinteraction();
                //            activityInteraction.InteractionAccountId = model.InteractionContactDetails[i].AccountId > 0 ? model.AccountId : null;
                //            activityInteraction.InteractionEntityId = model.InteractionContactDetails[i].EntityId;
                //            activityInteraction.InteractionRoleId = null;
                //            activityInteraction.ContactActivityId = activity.ContactActivityId;
                //            await _unitOfWork.ContactActivityInteractions.AddAsync(activityInteraction);
                //            await _unitOfWork.CommitAsync();
                //        }
                //    }
                //}
                //if (model.InteractionAccountList.Count > 0)
                //{
                //    for (int i = 0; i < model.InteractionAccountList.Count; i++)
                //    {
                //        var accountId = model.InteractionAccountList[i];
                //        var contactId = model.InteractionEntityList.Count >= i ? model.InteractionEntityList[i] : 0;
                //        var activityInteraction = new Contactactivityinteraction();
                //        if (accountId == 0)
                //        {
                //            activityInteraction.InteractionAccountId = null;
                //        }
                //        else
                //        {
                //            activityInteraction.InteractionAccountId = accountId;
                //        }
                //        activityInteraction.InteractionEntityId = contactId;
                //        activityInteraction.ContactActivityId = activity.ContactActivityId;
                //        _unitOfWork.ContactActivityInteractions.Update(activityInteraction);
                //        await _unitOfWork.CommitAsync();

                //    }
                //}
                #endregion

                return true;

            }
            return false;
        }

        public async Task<List<ContactActivityModel>> GetAllContactActivities()
        {
            List<ContactActivityModel> contactActivities = new List<ContactActivityModel>();
            var activities = await _unitOfWork.ContactActivities.GetAllContactActivitiesAsync();
            foreach (var activity in activities)
            {
                ContactActivityModel activityModel = new ContactActivityModel();

                activityModel = _mapper.Map<ContactActivityModel>(activity);
                activityModel.Entity.Person = _mapper.Map<PersonModel>(activity.Entity.People?.FirstOrDefault());
                activityModel.Entity.Company = _mapper.Map<CompanyModel>(activity.Entity.Companies?.FirstOrDefault());
                activityModel.ContactActivityInteractions = _mapper.Map<List<ContactActivityInteractionModel>>(activity.Contactactivityinteractions);
                if (activity.Contactactivityinteractions.Count > 0)
                {
                    activityModel.ContactActivityInteractions = new List<ContactActivityInteractionModel>();
                    foreach (var activityInteraction in activity.Contactactivityinteractions)
                    {
                        ContactActivityInteractionModel activityInteractionModel = new ContactActivityInteractionModel();
                        activityInteractionModel = _mapper.Map<ContactActivityInteractionModel>(activityInteraction);
                        activityInteractionModel.InteractionEntity.Person = _mapper.Map<PersonModel>(activityInteraction.InteractionEntity?.People?.FirstOrDefault());
                        activityModel.ContactActivityInteractions.Add(activityInteractionModel);
                    }
                }
                contactActivities.Add(activityModel);
            }
            return contactActivities;
        }

        public async Task<ContactActivityModel> GetContactActivityById(int id)
        {
            var activity = await _unitOfWork.ContactActivities.GetByIdAsync(id);
            return _mapper.Map<ContactActivityModel>(activity);
        }

        public async Task<IEnumerable<ContactActivityOutputModel>> GetContactActivityBySearchCondition(int entityId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId)
        {
            List<ContactActivityOutputModel> contactActivities = new List<ContactActivityOutputModel>();
            IEnumerable<Contactactivity> activities = new List<Contactactivity>();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(entityId);
            if (entity.PersonId > 0)
            {
                activities = await _unitOfWork.ContactActivities.GetContactActivityBySearchConditionAsync(entityId, fromDate, toDate, interactionType, interactionEntityId);

            }
            else if (entity.CompanyId > 0)
            {
                activities = await _unitOfWork.ContactActivities.GetAccountActivityBySearchConditionAsync(entity.CompanyId ?? 0, fromDate, toDate, interactionType, interactionEntityId);

            }
            foreach (var activity in activities)
            {
                ContactActivityOutputModel activityModel = new ContactActivityOutputModel();

                activityModel = _mapper.Map<ContactActivityOutputModel>(activity);
                activityModel.Entity.Person = _mapper.Map<PersonModel>(activity.Entity.People?.FirstOrDefault());
                activityModel.Entity.Company = _mapper.Map<CompanyModel>(activity.Entity.Companies?.FirstOrDefault());
                activityModel.ContactActivityInteractions = _mapper.Map<List<ContactActivityInteractionOutputModel>>(activity.Contactactivityinteractions);
                if (activity.Contactactivityinteractions.Count > 0)
                {
                    activityModel.ContactActivityInteractions = new List<ContactActivityInteractionOutputModel>();
                    var activityInteractions = activity.Contactactivityinteractions
                        .GroupBy(d => new { d.InteractionEntityId, d.InteractionAccountId, d.ContactActivityId, d.InteractionEntity })
                        .Select(x => new ContactActivityInteractionOutputModel
                        {
                            ContactActivityId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.ContactActivityId : 0,
                            InteractionAccountId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.InteractionAccountId : 0,
                            InteractionEntityId = x.Key.InteractionEntityId.HasValue ? (int)x.Key.InteractionEntityId : 0,
                            InteractionEntity = x.Key.InteractionEntity != null ? new EntityModel { Person = _mapper.Map<PersonModel>(x.Key.InteractionEntity.People.FirstOrDefault()) } : null,
                            InteractionRoles = x.Select(a => a.InteractionRoleId.HasValue ? a.InteractionRoleId : null)
                             .Where(a => a != null)
                             .Select(m => (int)m)
                             .ToList()
                        }).ToList();
                    activityModel.ContactActivityInteractions = activityInteractions;
                }

                contactActivities.Add(activityModel);
            }
            return contactActivities;
        }

        public async Task<IEnumerable<ContactActivityOutputModel>> GetRoleActivityBySearchCondition(int entityId, DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId, int roleId)
        {
            List<ContactActivityOutputModel> contactActivities = new List<ContactActivityOutputModel>();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(entityId);
            if (entity != null && entity.CompanyId != null)
            {
                var roleContacts = await _entityRoleService.GetContactsByRoleAndCompanyId(roleId, Convert.ToInt32(entity.CompanyId));
                var roleHistoryEntities = await _unitOfWork.EntityRoleHistories.GetAllEntityRoleHistoryByRoleIdAsync(roleId);
                List<int> selectedRoleContactsEntity = new List<int>();
                List<int> selectedRoleHistoryEntity = new List<int>();
                List<Contactactivity> contactActivity = new List<Contactactivity>();
                if (roleContacts.Any() || roleHistoryEntities.Any())
                {
                    if (roleContacts.Any())
                    {
                        selectedRoleContactsEntity.AddRange(roleContacts.Where(s => s.Status == (int)Status.Active).Select(s => s.EntityId).ToList());
                    }
                    if (roleHistoryEntities.Any())
                    {
                        selectedRoleHistoryEntity.AddRange(roleHistoryEntities.Select(s => Convert.ToInt32(s.EntityId)).ToList());
                    }
                    var roleDetails = await _unitOfWork.ContactRoles.GetByIdAsync(roleId);
                    var activities = await _unitOfWork.ContactActivities.GetEntityRoleActivityBySearchConditionAsync(Convert.ToInt32(entity.CompanyId), fromDate, toDate, interactionType, interactionEntityId, selectedRoleContactsEntity, selectedRoleHistoryEntity, roleDetails?.Name, roleId);
                    if (activities.Any())
                    {
                        contactActivity = activities.ToList();
                    }
                }
                foreach (var activity in contactActivity)
                {
                    ContactActivityOutputModel activityModel = new ContactActivityOutputModel();

                    activityModel = _mapper.Map<ContactActivityOutputModel>(activity);
                    activityModel.Entity.Person = _mapper.Map<PersonModel>(activity.Entity.People?.FirstOrDefault());
                    activityModel.Entity.Company = _mapper.Map<CompanyModel>(activity.Entity.Companies?.FirstOrDefault());
                    activityModel.ContactActivityInteractions = _mapper.Map<List<ContactActivityInteractionOutputModel>>(activity.Contactactivityinteractions);
                    if (activity.Contactactivityinteractions.Count > 0)
                    {
                        activityModel.ContactActivityInteractions = new List<ContactActivityInteractionOutputModel>();
                        var activityInteractions = activity.Contactactivityinteractions
                            .GroupBy(d => new { d.InteractionEntityId, d.InteractionAccountId, d.ContactActivityId, d.InteractionEntity })
                            .Select(x => new ContactActivityInteractionOutputModel
                            {
                                ContactActivityId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.ContactActivityId : 0,
                                InteractionAccountId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.InteractionAccountId : 0,
                                InteractionEntityId = x.Key.InteractionEntityId.HasValue ? (int)x.Key.InteractionEntityId : 0,
                                InteractionEntity = x.Key.InteractionEntity != null ? new EntityModel { Person = _mapper.Map<PersonModel>(x.Key.InteractionEntity.People.FirstOrDefault()) } : null,
                                InteractionRoles = x.Select(a => a.InteractionRoleId.HasValue ? a.InteractionRoleId : null)
                                 .Where(a => a != null)
                                 .Select(m => (int)m)
                                 .ToList()
                            }).ToList();
                        activityModel.ContactActivityInteractions = activityInteractions;
                    }

                    contactActivities.Add(activityModel);
                }
            }
            return contactActivities;
        }

        public async Task<List<ContactActivityOutputModel>> GetContactActivityByEntityId(int id, int? recordsCount = 0)
        {
            List<ContactActivityOutputModel> contactActivities = new List<ContactActivityOutputModel>();
            IEnumerable<Contactactivity> activities = new List<Contactactivity>();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(id);
            if (entity.PersonId > 0)
            {
                activities = await _unitOfWork.ContactActivities.GetContactActivityByEntityIdAsync(id);
                if (recordsCount.HasValue && recordsCount > 0)
                {
                    activities = activities.Take(recordsCount.Value).ToList();
                }
            }
            else if (entity.CompanyId > 0)
            {
                activities = await _unitOfWork.ContactActivities.GetContactActivitiesByAccountIdAsync(entity.CompanyId ?? 0);
                if (recordsCount.HasValue && recordsCount > 0)
                {
                    activities = activities.Take(recordsCount.Value).ToList();
                }
            }
            foreach (var activity in activities)
            {
                ContactActivityOutputModel activityModel = new ContactActivityOutputModel();

                activityModel = _mapper.Map<ContactActivityOutputModel>(activity);
                activityModel.Entity.Person = _mapper.Map<PersonModel>(activity.Entity.People?.FirstOrDefault());
                activityModel.Entity.Company = _mapper.Map<CompanyModel>(activity.Entity.Companies?.FirstOrDefault());
                activityModel.ContactActivityInteractions = _mapper.Map<List<ContactActivityInteractionOutputModel>>(activity.Contactactivityinteractions);
                if (activity.Contactactivityinteractions.Count > 0)
                {
                    activityModel.ContactActivityInteractions = new List<ContactActivityInteractionOutputModel>();
                    var activityInteractions = activity.Contactactivityinteractions
                        .GroupBy(d => new { d.InteractionEntityId, d.InteractionAccountId, d.ContactActivityId, d.InteractionEntity })
                        .Select(x => new ContactActivityInteractionOutputModel
                        {
                            ContactActivityId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.ContactActivityId : 0,
                            InteractionAccountId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.InteractionAccountId : 0,
                            InteractionEntityId = x.Key.InteractionEntityId.HasValue ? (int)x.Key.InteractionEntityId : 0,
                            InteractionEntity = x.Key.InteractionEntity != null ? new EntityModel { Person = _mapper.Map<PersonModel>(x.Key.InteractionEntity.People.FirstOrDefault()) } : null,
                            InteractionRoles = x.Select(a => a.InteractionRoleId.HasValue ? a.InteractionRoleId : null)
                             .Where(a => a != null)
                             .Select(m => (int)m)
                             .ToList()
                        }).ToList();
                    activityModel.ContactActivityInteractions = activityInteractions;
                    //foreach (var activityInteraction in activity.Contactactivityinteractions)
                    //{
                    //    ContactActivityInteractionOutputModel activityInteractionModel = new ContactActivityInteractionOutputModel();
                    //    activityInteractionModel = _mapper.Map<ContactActivityInteractionOutputModel>(activityInteraction);
                    //    activityInteractionModel.InteractionEntity.Person = _mapper.Map<PersonModel>(activityInteraction.InteractionEntity?.People?.FirstOrDefault());
                    //    activityModel.ContactActivityInteractions.Add(activityInteractionModel);
                    //}
                }
                contactActivities.Add(activityModel);
            }
            return contactActivities;
        }

        public async Task<List<ContactActivityOutputModel>> GetRoleActivityByEntityId(int id, int roleId)
        {
            List<ContactActivityOutputModel> contactActivities = new List<ContactActivityOutputModel>();
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(id);
            if (entity != null && entity.CompanyId != null)
            {
                var roleContacts = await _entityRoleService.GetContactsByRoleAndCompanyId(roleId, Convert.ToInt32(entity.CompanyId));
                var roleHistoryEntities = await _unitOfWork.EntityRoleHistories.GetAllEntityRoleHistoryByRoleIdAsync(roleId);
                List<int> selectedRoleContactsEntity = new List<int>();
                List<int> selectedRoleHistoryEntity = new List<int>();
                List<Contactactivity> contactActivity = new List<Contactactivity>();
                if (roleContacts.Any() || roleHistoryEntities.Any())
                {
                    if (roleContacts.Any())
                    {
                        selectedRoleContactsEntity.AddRange(roleContacts.Select(s => s.EntityId).ToList());
                    }
                    if (roleHistoryEntities.Any())
                    {
                        selectedRoleHistoryEntity.AddRange(roleHistoryEntities.Select(s => Convert.ToInt32(s.EntityId)).ToList());
                    }
                    var roleDetails = await _unitOfWork.ContactRoles.GetByIdAsync(roleId);
                    var activities = await _unitOfWork.ContactActivities.GetEntityRoleActivityByEntityIdAsync(Convert.ToInt32(entity.CompanyId), selectedRoleContactsEntity, selectedRoleHistoryEntity, roleDetails?.Name, roleId);
                    contactActivity = activities.ToList();
                }
                foreach (var activity in contactActivity)
                {
                    ContactActivityOutputModel activityModel = new ContactActivityOutputModel();

                    activityModel = _mapper.Map<ContactActivityOutputModel>(activity);
                    activityModel.Entity.Person = _mapper.Map<PersonModel>(activity.Entity.People?.FirstOrDefault());
                    activityModel.Entity.Company = _mapper.Map<CompanyModel>(activity.Entity.Companies?.FirstOrDefault());
                    activityModel.ContactActivityInteractions = _mapper.Map<List<ContactActivityInteractionOutputModel>>(activity.Contactactivityinteractions);
                    if (activity.Contactactivityinteractions.Count > 0)
                    {
                        activityModel.ContactActivityInteractions = new List<ContactActivityInteractionOutputModel>();
                        var activityInteractions = activity.Contactactivityinteractions
                            .GroupBy(d => new { d.InteractionEntityId, d.InteractionAccountId, d.ContactActivityId, d.InteractionEntity })
                            .Select(x => new ContactActivityInteractionOutputModel
                            {
                                ContactActivityId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.ContactActivityId : 0,
                                InteractionAccountId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.InteractionAccountId : 0,
                                InteractionEntityId = x.Key.InteractionEntityId.HasValue ? (int)x.Key.InteractionEntityId : 0,
                                InteractionEntity = x.Key.InteractionEntity != null ? new EntityModel { Person = _mapper.Map<PersonModel>(x.Key.InteractionEntity.People.FirstOrDefault()) } : null,
                                InteractionRoles = x.Select(a => a.InteractionRoleId.HasValue ? a.InteractionRoleId : null)
                                 .Where(a => a != null)
                                 .Select(m => (int)m)
                                 .ToList()
                            }).ToList();
                        activityModel.ContactActivityInteractions = activityInteractions;
                    }

                    contactActivities.Add(activityModel);
                }
            }
            return contactActivities;
        }

        public async Task<List<ContactActivityOutputModel>> GetContactActivityByAccountIdAndEntityId(int accountId, int entityId)
        {
            var activityList = new List<ContactActivityOutputModel>();
            var activities = await _unitOfWork.ContactActivities.GetContactActivityByAccountAndEntityIdAsync(accountId, entityId);
            var activitiesModel = _mapper.Map<List<ContactActivityOutputModel>>(activities);
            foreach (var activity in activities)
            {
                ContactActivityOutputModel activityModel = new ContactActivityOutputModel();

                activityModel = _mapper.Map<ContactActivityOutputModel>(activity);
                activityModel.Entity.Person = _mapper.Map<PersonModel>(activity.Entity.People?.FirstOrDefault());
                activityModel.Entity.Company = _mapper.Map<CompanyModel>(activity.Entity.Companies?.FirstOrDefault());
                activityModel.ContactActivityInteractions = _mapper.Map<List<ContactActivityInteractionOutputModel>>(activity.Contactactivityinteractions);
                if (activity.Contactactivityinteractions.Count > 0)
                {
                    activityModel.ContactActivityInteractions = new List<ContactActivityInteractionOutputModel>();
                    var activityInteractions = activity.Contactactivityinteractions
                        .GroupBy(d => new { d.InteractionEntityId, d.InteractionAccountId, d.ContactActivityId, d.InteractionEntity })
                        .Select(x => new ContactActivityInteractionOutputModel
                        {
                            ContactActivityId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.ContactActivityId : 0,
                            InteractionAccountId = x.Key.InteractionAccountId.HasValue ? (int)x.Key.InteractionAccountId : 0,
                            InteractionEntityId = x.Key.InteractionEntityId.HasValue ? (int)x.Key.InteractionEntityId : 0,
                            InteractionEntity = x.Key.InteractionEntity != null ? new EntityModel { Person = _mapper.Map<PersonModel>(x.Key.InteractionEntity.People.FirstOrDefault()) } : null,
                            InteractionRoles = x.Select(a => a.InteractionRoleId.HasValue ? a.InteractionRoleId : null)
                             .Where(a => a != null)
                             .Select(m => (int)m)
                             .ToList()
                        }).ToList();
                    activityModel.ContactActivityInteractions = activityInteractions;
                }

                activityList.Add(activityModel);
            }
            return activityList;
        }
        public async Task<IEnumerable<ContactActivityModel>> GetContactActivityByActivityDate(int entityId, DateTime activityDate)
        {
            var activities = await _unitOfWork.ContactActivities.GetContactActivityByActivityDateAsync(entityId, activityDate);
            var contactActivities = _mapper.Map<List<ContactActivityModel>>(activities);
            return contactActivities;
        }

        public async Task<IEnumerable<ContactActivityModel>> GetContactActivityByRoleIdAndActivityDateAsync(int entityId,int roleId, DateTime activityDate)
        {
            var activities = await _unitOfWork.ContactActivities.GetContactActivityByRoleIdAndActivityDateAsync(entityId,roleId, activityDate);
            var contactActivities = _mapper.Map<List<ContactActivityModel>>(activities);
            return contactActivities;
        }

        public async Task<bool> DeleteContactActivity(int id)
        {
            var contactActivity = await _unitOfWork.ContactActivities.GetContactActivityByIdAsync(id);
            if (contactActivity != null)
            {
                var activityInteractions = new List<Contactactivityinteraction>();
                foreach (var item in contactActivity.Contactactivityinteractions)
                {
                    item.IsDeleted = (int)Status.Active;
                    activityInteractions.Add(item);
                }
                contactActivity.Contactactivityinteractions = activityInteractions;
                contactActivity.IsDeleted = (int)Status.Active;
                _unitOfWork.ContactActivities.Update(contactActivity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> CreateAccountChangeContactActivity(ContactActivityInputModel model, bool isAccountAssigned)
        {
            Contactactivity contactActivity = new Contactactivity();
            contactActivity.EntityId = model.EntityId;
            contactActivity.AccountId = model.AccountId;
            contactActivity.ActivityConnection = (int)ContactActivityConnectionType.RoleContact;
            contactActivity.ActivityDate = DateTime.UtcNow;
            contactActivity.InteractionType = (int)ContactActivityInteractionType.AccountChange;
            contactActivity.StaffUserId = null;
            contactActivity.Status = (int)Status.Active;
            var entityDetails = await _unitOfWork.Entities.GetByIdAsync(model.EntityId);
            if (entityDetails != null && model.AccountId > 0)
            {
                if (entityDetails.PersonId != null)
                {
                    var personDetails = await _unitOfWork.Persons.GetByIdAsync(Convert.ToInt32(entityDetails.PersonId));
                    var accountDetails = await _unitOfWork.Companies.GetByIdAsync(model.AccountId);
                    if (personDetails != null)
                    {
                        contactActivity.Subject = $"Account Change for {personDetails.FirstName} {personDetails.LastName}";
                        if (isAccountAssigned)
                        {
                            contactActivity.Description = $"{personDetails.FirstName} {personDetails.LastName} assigned to {accountDetails.CompanyName} Account";
                        }
                        else
                        {
                            contactActivity.Description = $"{personDetails.FirstName} {personDetails.LastName} unassigned from {accountDetails.CompanyName} Account";

                        }
                        await _unitOfWork.ContactActivities.AddAsync(contactActivity);
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
