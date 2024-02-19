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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Max.Services
{
    public class RelationService : IRelationService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISociableService _sociableService;
        public RelationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISociableService sociableService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
            this._sociableService = sociableService;
        }

        public async Task<IEnumerable<Relation>> GetAllRelations()
        {
            return await _unitOfWork.Relations
                .GetAllRelationsAsync();
        }

        public async Task<Relation> GetRelationById(int id)
        {
            return await _unitOfWork.Relations
                .GetRelationByIdAsync(id);
        }

        public async Task<IEnumerable<RelationModel>> GetRelationsByEntityId(int entityId)
        {
            List<RelationModel> relationships = new List<RelationModel>();
            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);
            //Map Relations
            var relations = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(entityId);
            foreach (var item in relations)
            {
                var relation = new RelationModel();

                relation.RelatedEntityId = item.RelatedEntityId;
                relation.RelationId = item.RelationId;
                relation.RelationshipId = item.RelationshipId;
                relation.RelationshipType = item.Relationship.Relation;
                relation.StartDate = item.StartDate;
                relation.EndDate = item.EndDate;
                relation.Notes = item.Notes;
                relation.PrimaryChecked = item.Primary != null ? Convert.ToBoolean(item.Primary) : false;
                relation.BillableChecked = item.Billable != null ? Convert.ToBoolean(item.Billable) : false;
                relation.SocialChecked = item.Social != null ? Convert.ToBoolean(item.Social) : false;
                if (item.RelatedEntity.PersonId != null)
                {
                    var person = await _unitOfWork.Persons.GetByIdAsync(item.RelatedEntity.PersonId ?? 0);

                    relation.FirstName = person.FirstName;
                    relation.LastName = person.LastName;
                    relation.DateOfBirth = person.DateOfBirth.ToString();
                    relation.Gender = person.Gender;
                    relation.IsPeople = true;
                }
                else if (item.RelatedEntity.CompanyId != null)
                {
                    var company = await _unitOfWork.Companies.GetCompanyByIdAsync(item.RelatedEntity.CompanyId ?? 0);

                    relation.FirstName = company.CompanyName;
                    relation.LastName = string.Empty;
                    relation.DateOfBirth = string.Empty;
                    relation.Gender = "";
                    relation.IsPeople = false;
                    relation.CompanyEmail = company.Emails.Count > 0 ? _mapper.Map<List<EmailModel>>(company.Emails)?.GetPrimaryEmail() : null;
                    relation.CompanyPhone = company.Phones.Count > 0 ? _mapper.Map<List<PhoneModel>>(company.Phones)?.GetPrimaryPhoneNumber().FormatPhoneNumber() : null;
                }

                relationships.Add(relation);
            }

            //Map Reverse Relations
            var reverseRelations = await _unitOfWork.Relations.GetAllReverseRelationsByEntityIdAsync(entityId);
            foreach (var item in reverseRelations)
            {
                var relation = new RelationModel();
                string relationshipType = "Unknown";

                relation.RelatedEntityId = item.EntityId;
                relation.RelationId = item.RelationId;
                relation.RelationshipId = item.RelationshipId;
                relation.RelationshipType = relationshipType;
                relation.StartDate = item.StartDate;
                relation.EndDate = item.EndDate;
                relation.Notes = item.Notes;
                relation.PrimaryChecked = item.Primary != null ? Convert.ToBoolean(item.Primary) : false;
                relation.BillableChecked = item.Billable != null ? Convert.ToBoolean(item.Billable) : false;
                relation.SocialChecked = item.Social != null ? Convert.ToBoolean(item.Social) : false;
                if (item.Entity.PersonId != null)
                {
                    var person = await _unitOfWork.Persons.GetByIdAsync(item.Entity.PersonId ?? 0);

                    relation.FirstName = person.FirstName;
                    relation.LastName = person.LastName;
                    relation.DateOfBirth = person.DateOfBirth.ToString();
                    relation.Gender = person.Gender;
                    relation.IsPeople = true;
                    if (relation.Gender == "Female")
                    {
                        relation.RelationshipType = item.Relationship.ReverseRelationFemale;
                    }
                    else
                    {
                        relation.RelationshipType = item.Relationship.ReverseRelationMale;
                    }
                }
                else if (item.Entity.CompanyId != null)
                {

                    var company = await _unitOfWork.Companies.GetCompanyByIdAsync(item.Entity.CompanyId ?? 0);
                    if (company != null)
                    {
                        relation.FirstName = company.CompanyName;
                        relation.LastName = string.Empty;
                        relation.DateOfBirth = string.Empty;
                        relation.Gender = string.Empty;
                        relation.IsPeople = false;
                        //if (entity.PersonId != null)
                        //{
                        //    relation.RelationshipType = item.Relationship.Relation;
                        //}
                        //else
                        //{
                        relation.RelationshipType = item.Relationship.ReverseRelationMale;
                        //}
                        relation.CompanyEmail = company.Emails.Count > 0 ? _mapper.Map<List<EmailModel>>(company.Emails)?.GetPrimaryEmail() : null;
                        relation.CompanyPhone = company.Phones.Count > 0 ? _mapper.Map<List<PhoneModel>>(company.Phones)?.GetPrimaryPhoneNumber().FormatPhoneNumber() : null;
                    }
                }

                relationships.Add(relation);
            }

            return relationships;
        }

        public async Task<IEnumerable<Relation>> GetReverseRelationsByEntityId(int entityId)
        {
            return await _unitOfWork.Relations
                .GetAllReverseRelationsByEntityIdAsync(entityId);
        }
        public async Task<Relation> CreateRelation(RelationModel model)
        {
            Relation relation = new Relation();
            var relations = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(model.EntityId ?? 0);
            var reverseRelations = await _unitOfWork.Relations.GetAllReverseRelationsByEntityIdAsync(model.EntityId ?? 0);
            var companyRelatedEntities = reverseRelations.Where(x => x.Entity.CompanyId != null);
            reverseRelations = reverseRelations.Except(companyRelatedEntities);

            if (relations.Any(x => x.RelatedEntityId == model.RelatedEntityId)
                && reverseRelations.Any(x => x.RelatedEntityId == model.EntityId))
            {
                throw new InvalidOperationException("A relationship already exists.");
            }

            var relationships = await _unitOfWork.Relationships.GetAllAsync();

            if (model.RelationshipId == 0)
            {
                model.RelationshipId = relationships.Where(x => x.Relation == "Unknown").Select(x => x.RelationshipId).FirstOrDefault();
            }

            if (ValidRelation(model))
            {
                relation.EntityId = model.EntityId;
                relation.RelatedEntityId = model.RelatedEntityId;
                relation.EndDate = model.EndDate;
                relation.RelationshipId = model.RelationshipId;
                relation.StartDate = model.StartDate;
                relation.EndDate = model.EndDate;
                relation.Status = model.Status;
                relation.Notes = model.Notes;
                relation.Primary = model.PrimaryChecked;
                relation.Billable = model.BillableChecked;
                relation.Social = model.SocialChecked;

                await _unitOfWork.Relations.AddAsync(relation);
                await _unitOfWork.CommitAsync();
            }

            return relation;
        }


        public async Task<Relation> UpdateRelation(RelationModel model)
        {
            Relation Relation = await _unitOfWork.Relations.GetRelationByIdAsync(model.RelationId);
            if (Relation != null)
            {
                Relation.RelationshipId = model.RelationshipId;
                Relation.Primary = model.PrimaryChecked;
                Relation.Billable = model.BillableChecked;
                Relation.Social = model.SocialChecked;
                _unitOfWork.Relations.Update(Relation);
                await _unitOfWork.CommitAsync();
            }
            return Relation;
        }

        public async Task<bool> DeleteRelation(int RelationId)
        {
            Relation Relation = await _unitOfWork.Relations.GetRelationByIdAsync(RelationId);

            if (Relation != null)
            {
                _unitOfWork.Relations.Remove(Relation);
                await _unitOfWork.CommitAsync();

                if (Relation.EntityId != null)
                {
                    var checkIsPerson = await _unitOfWork.Persons.GetPersonByEntityIdAsync(Convert.ToInt32(Relation.EntityId));
                    if(checkIsPerson != null)
                    {
                        var entityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(checkIsPerson.EntityId));
                        if(entityDetails != null)
                        {
                            if(entityDetails.SociableProfileId!=null && entityDetails.SociableProfileId > 0)
                            {
                                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                                await _sociableService.RemoveCompanyForUser(Convert.ToInt32(entityDetails.SociableProfileId), staff.OrganizationId);
                            }
                        }
                    }
                }

                return true;

            }
            throw new InvalidOperationException($"Relation: {RelationId} not found.");

        }

        public async Task<List<SelectListModel>> GetRelationSelectListByEntityId(int entityId)
        {
            var relations = await _unitOfWork.Relations.GetAllRelationsByEntityIdAsync(entityId);
            var reverseRelations = await _unitOfWork.Relations.GetAllReverseRelationsByEntityIdAsync(entityId);
            List<SelectListModel> typeList = new List<SelectListModel>();
            SelectListModel model = new SelectListModel();
            foreach (var item in relations)
            {
                model = new SelectListModel();
                model.code = item.RelatedEntityId.ToString();
                model.name = item.RelatedEntity.Name;
                typeList.Add(model);
            }
            foreach (var item in reverseRelations)
            {
                model = new SelectListModel();
                model.code = item.EntityId.ToString();
                model.name = item.Entity.Name;
                typeList.Add(model);
            }
            // Add the member himself
            var entity = await _unitOfWork.Entities.GetByIdAsync(entityId);
            model = new SelectListModel();
            model.code = entity.EntityId.ToString();
            model.name = entity.Name;
            typeList.Add(model);

            return typeList.OrderBy(x => x.name).ToList();
        }

        public async Task<bool> AddOrUpdateRelation(List<RelationModel> RelationModel)
        {
            try
            {
                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(Convert.ToInt32(staff.OrganizationId));
                var companyId = RelationModel.FirstOrDefault(s => s.EntityId != null);
                CompanyModel companyModel = new CompanyModel();
                Entity companyEntityDetails = new Entity();
                int? sociableCompanyId = 0;
                if (companyId != null)
                {
                    companyEntityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(companyId.EntityId));
                    var companyDetails = await _unitOfWork.Companies.GetCompanyByIdAsync(Convert.ToInt32(companyEntityDetails.CompanyId));
                    if (companyEntityDetails != null)
                    {
                        sociableCompanyId = companyEntityDetails.SociableUserId != null ? companyEntityDetails.SociableUserId : null;
                        companyModel = _mapper.Map<CompanyModel>(companyDetails);

                        // Company add if not exist
                        if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                        {
                            if (sociableCompanyId == null || sociableCompanyId < 1)
                            {
                                int returnedSociableCompanyId = await _sociableService.CreatePerson(null, companyModel, staff.OrganizationId);
                                sociableCompanyId = returnedSociableCompanyId;
                                companyEntityDetails.SociableUserId = returnedSociableCompanyId;
                            }

                            if (sociableCompanyId > 0)
                            {
                                var companyInfo = await _sociableService.GetUserById(Convert.ToInt32(sociableCompanyId), staff.OrganizationId);
                                dynamic company = JObject.Parse(companyInfo);
                                var companyProfileId = company.profile_profiles[0].target_id;
                                if (companyProfileId > 0)
                                {

                                    var result = await _sociableService.UpdatePersonProfile(null, companyModel, (int)companyProfileId, staff.OrganizationId);
                                }
                                companyEntityDetails.SociableProfileId = companyProfileId;
                                _unitOfWork.Entities.Update(companyEntityDetails);
                                await _unitOfWork.CommitAsync();
                            }
                        }
                    }
                }
                foreach (var item in RelationModel)
                {
                    if (item.RelationId > 0)
                    {
                        await UpdateRelation(item);
                    }
                    else if (item.RelationId == 0)
                    {
                        await CreateRelation(item);
                    }

                    if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                    {
                        // person add/update operation
                        var entityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(item.RelatedEntityId));
                        var personDetails = await _unitOfWork.Persons.GetPersonByEntityIdAsync(Convert.ToInt32(item.RelatedEntityId));
                        if (personDetails != null)
                        {
                            var personModel = _mapper.Map<PersonModel>(personDetails);
                            if (sociableCompanyId != null)
                            {
                                personModel.SocialCompanyId = Convert.ToInt32(sociableCompanyId);
                            }
                            if (item.SocialChecked)
                            {
                                personModel.IsSociablemanager = true;
                            }
                            if (item.BillableChecked)
                            {
                                personModel.IsBillableManager = true;
                            }
                            var sociableUserId = entityDetails.SociableUserId != null ? entityDetails.SociableUserId : null;
                            if (sociableUserId != null || sociableUserId > 0)
                            {
                                var checkUserInfoExist = await _sociableService.GetUserById(Convert.ToInt32(sociableUserId), personModel.OrganizationId ?? 0);
                                dynamic uInfo = JObject.Parse(checkUserInfoExist);
                                if (uInfo.uid == null)
                                {
                                    sociableUserId = null;
                                }
                            }
                            if (sociableUserId == null || sociableUserId <= 0)
                            {
                                int returnedSociableUserId = await _sociableService.CreatePerson(personModel, null,
                                    staff.OrganizationId);
                                sociableUserId = returnedSociableUserId;
                                entityDetails.SociableUserId = returnedSociableUserId;
                                sociableUserId = entityDetails.SociableUserId;
                            }
                            else
                            {
                                await _sociableService.UpdatePerson(Convert.ToInt32(entityDetails.SociableUserId),entityDetails.WebLoginName, "",personModel.Emails.GetPrimaryEmail(), staff.OrganizationId, personModel.IsBillableManager, personModel.IsSociablemanager, true);
                            }

                            if (sociableUserId > 0)
                            {
                                var userInfo = await _sociableService.GetUserById(Convert.ToInt32(sociableUserId), staff.OrganizationId);
                                dynamic profile = JObject.Parse(userInfo);
                                var profileId = profile.profile_profiles[0].target_id;
                                if (profileId > 0)
                                {

                                    var result = await _sociableService.UpdatePersonProfile(personModel, null, (int)profileId, staff.OrganizationId);
                                }

                                if (profileId > 0)
                                {
                                    if (item.PrimaryChecked)
                                    {
                                        companyModel.SociablePrimaryContact = Convert.ToInt32(sociableUserId);
                                    }
                                    if (item.BillableChecked)
                                    {
                                        companyModel.SociableBillableContact = Convert.ToInt32(sociableUserId);
                                    }
                                    if (item.SocialChecked)
                                    {
                                        companyModel.SociableManager.Add(Convert.ToInt32(sociableUserId));
                                    }
                                }

                                entityDetails.SociableProfileId = profileId;
                                _unitOfWork.Entities.Update(entityDetails);
                                await _unitOfWork.CommitAsync();
                            }
                        }
                    }
                }

                // update company with relations
                if(configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                {
                    if (sociableCompanyId > 0)
                    {
                        var companyInfo = await _sociableService.GetUserById(Convert.ToInt32(sociableCompanyId), staff.OrganizationId);
                        dynamic company = JObject.Parse(companyInfo);
                        var companyProfileId = company.profile_profiles[0].target_id;
                        if (companyProfileId > 0)
                        {

                            var result = await _sociableService.UpdatePersonProfile(null, companyModel, (int)companyProfileId, staff.OrganizationId);
                        }
                        companyEntityDetails.SociableProfileId = companyProfileId;
                        _unitOfWork.Entities.Update(companyEntityDetails);
                        await _unitOfWork.CommitAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool ValidRelation(RelationModel model)
        {
            //Validate  Name
            if (model.EntityId == 0)
            {
                throw new InvalidOperationException($"Entity Id not defined.");
            }

            if (model.RelatedEntityId == 0)
            {
                throw new NullReferenceException($"Related  Entity Id  not defined.");
            }

            return true;
        }
    }
}
