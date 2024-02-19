
using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Max.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;
        private readonly ITenantService _tenantService;
        private readonly IRelationService _relationService;
        private readonly ISociableService _sociableService;
        private readonly ICompanyService _companyService;
        private readonly ILogger<PersonService> _logger;
        private readonly IEntityService _entityService;
        private readonly IEntityRoleService _entityRoleService;
        private readonly IContactActivityService _contactActivityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PersonService(IEntityService entityService, ICompanyService companyService,
            IUnitOfWork unitOfWork, IMapper mapper, IInvoiceService invoiceService,
            IRelationService relationService, ISociableService sociableService,
            ITenantService tenantService, IEntityRoleService entityRoleService, IContactActivityService contactActivityService,
            ILogger<PersonService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._invoiceService = invoiceService;
            this._relationService = relationService;
            this._sociableService = sociableService;
            this._tenantService = tenantService;
            this._contactActivityService = contactActivityService;
            _companyService = companyService;
            _entityService = entityService;
            _entityRoleService = entityRoleService;
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<Person> CreatePerson(PersonModel personModel)
        {
            Person person = new Person();

            //Get organization: TODO : needs to add in session of the user
            //In case of multipl organization uI should have organization field

            var organization = await _unitOfWork.Organizations.GetAllOrganizationsAsync();

            if (organization != null)
            {
                personModel.OrganizationId = organization.FirstOrDefault().OrganizationId;
            }
            else
            {
                personModel.OrganizationId = 0;
            }

            int? socialCompanyId = null;
            if (personModel.CompanyId == null && personModel.CompanyName != "")
            {
                var checkCompany = await _companyService.GetCompaniesByName(personModel.CompanyName, string.Empty);
                var exist = checkCompany.Where(x => x.CompanyName == personModel.CompanyName).ToList();
                if (exist.Count() != 0)
                {
                    personModel.CompanyId = exist.FirstOrDefault().CompanyId;
                }
                else
                {
                    var companyModel = new CompanyModel();
                    companyModel.CompanyName = personModel.CompanyName;
                    companyModel.OrganizationId = personModel.OrganizationId ?? 1;
                    var comapny = await _companyService.CreateCompany(companyModel);
                    personModel.CompanyId = comapny.CompanyId;
                }
                if (personModel.CompanyId != null)
                {
                    var entityDetails = await _unitOfWork.Entities.GetEntityByCompanyIdAsync(Convert.ToInt32(personModel.CompanyId));
                    if (entityDetails != null)
                    {
                        socialCompanyId = entityDetails.SociableUserId;
                    }
                }
            }

            var isValidPerson = await ValidPerson(personModel);
            if (isValidPerson)
            {
                person.Prefix = personModel.Prefix;
                person.FirstName = personModel.FirstName;
                person.LastName = personModel.LastName;
                person.MiddleName = personModel.MiddleName;
                person.CasualName = personModel.CasualName;
                person.Suffix = personModel.Suffix;
                person.OrganizationId = personModel.OrganizationId;
                person.Gender = personModel.Gender;
                person.PreferredContact = personModel.PreferredContact;
                person.DateOfBirth = personModel.DateOfBirth;
                person.CompanyId = personModel.CompanyId == 0 ? null : personModel.CompanyId;
                person.FacebookName = personModel.FacebookName;
                person.InstagramName = personModel.InstagramName;
                person.LinkedinName = personModel.LinkedinName;
                person.SkypeName = personModel.SkypeName;
                person.Salutation = personModel.Salutation;
                person.Status = personModel.Status;
                person.Website = personModel.Website;
                person.Title = personModel.Title;
                person.Designation = personModel.Designation;
                person.MemberId = personModel.MemberId;
                //Add  Emails

                //Check if primary email is unique 
                var existingEmail = await _unitOfWork.Emails.GetPrimaryEmailByEmailAddressAsync(personModel.Emails.Where(x => x.IsPrimary == (int)Status.Active).Select(x => x.EmailAddress).FirstOrDefault());

                if (existingEmail != null)
                {
                    throw new Exception("Please use a unique Email Address for Primary Email.");
                }
                foreach (var item in personModel.Emails)
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
                        person.Emails.Add(email);
                    }
                }

                //Add  Phone Numbers
                foreach (var item in personModel.Phones)
                {
                    Phone phone = new Phone();
                    phone.PhoneType = item.PhoneType;
                    phone.PhoneNumber = item.PhoneNumber.GetCleanPhoneNumber();
                    phone.IsPrimary = item.IsPrimary;
                    person.Phones.Add(phone);
                }

                //Add  Addresses
                foreach (var item in personModel.Addresses)
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

                    person.Addresses.Add(address);
                }

                //Add an entity

                var primaryEmail = personModel.Emails.GetPrimaryEmail();
                string webLogin = string.Empty;

                var entity = new Entity();
                entity.Name = $"{person.Prefix} {person.FirstName} {person.LastName}";
                entity.OrganizationId = person.OrganizationId;

                PasswordHash hash = new PasswordHash(Guid.NewGuid().ToString());

                entity.WebPasswordSalt = hash.Salt;
                entity.WebPassword = hash.Password;

                person.Entity = entity;
                try
                {
                    await _unitOfWork.Persons.AddAsync(person);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Create Person:  AddPerson failed First Name: {person.FirstName} Last Name:{person.LastName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                    throw ex;
                }

                var userName = await _unitOfWork.Entities.GetEntityByUserNameAsync($"{person.FirstName}{person.LastName}");
                if (userName == null)
                {
                    webLogin = $"{person.FirstName}{person.LastName}";
                }
                else
                {
                    webLogin = $"{person.FirstName}{person.LastName}{person.PersonId}";
                }
                entity.WebLoginName = webLogin;
                //Update entity
                try
                {
                    entity.PersonId = person.PersonId;
                    _unitOfWork.Entities.Update(entity);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Create Person: Failed to update entity for Name: {entity.Name} {entity.WebLoginName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                    throw ex;
                }
                var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(person.OrganizationId ?? 0);

                if (configuration.DisplayRoles != null && configuration.DisplayRoles == (int)Status.Active)
                {
                    if (personModel.CompanyId != null && personModel.CompanyId > 0)
                    {
                        ContactActivityInputModel model = new ContactActivityInputModel();
                        model.EntityId = person.EntityId ?? 0;
                        //create activity for account assigned
                        model.AccountId = Convert.ToInt32(personModel.CompanyId);
                        await _contactActivityService.CreateAccountChangeContactActivity(model, true);
                    }
                }

                var newEntityRoles = personModel.EntityRoles.ToList();
                foreach (var item in newEntityRoles)
                {
                    var checkEntityRoleExist = await _unitOfWork.EntityRoles.GetAllEntityRolesByEntityIdContactRoleIdAndCompanyIdAsync(personModel.EntityId, item.ContactRoleId, Convert.ToInt32(personModel.CompanyId));
                    if (checkEntityRoleExist.Count() == 0)
                    {
                        var entityRole = new EntityRoleModel();
                        entityRole.EntityId = person.EntityId ?? 0;
                        entityRole.ContactRoleId = item.ContactRoleId;
                        entityRole.EffectiveDate = item.EffectiveDate;
                        entityRole.EndDate = item.EndDate;
                        entityRole.Status = item.Status;
                        entityRole.CompanyId = personModel.CompanyId ?? 0;
                        await _entityRoleService.CreateEntityRole(entityRole);
                    }
                }

                //Check if Relation is defined
                if (personModel.AddRelationToContact > 0)
                {
                    RelationModel relation = new RelationModel();
                    relation.EntityId = personModel.AddRelationToContact;
                    relation.RelatedEntityId = entity.EntityId;
                    relation.StartDate = DateTime.Now;
                    relation.RelationshipId = personModel.RelationshipId;
                    relation.Status = (int)Status.Active;
                    relation.Notes = string.Empty;
                    await this._relationService.CreateRelation(relation);
                }
                //check if add to billable contact
                if (personModel.AddBillableToContact > 0)
                {
                    await _entityService.AddBillableContact(personModel.AddBillableToContact, person.PersonId);
                }

                //Create sociable Profile
                var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                {
                    try
                    {
                        //create company profile if it is not exist in social
                        if (socialCompanyId == null)
                        {
                            if (personModel.CompanyId != null)
                            {
                                var companyDetails = await _unitOfWork.Companies.GetByIdAsync(Convert.ToInt32(personModel.CompanyId));
                                if (companyDetails != null)
                                {
                                    companyDetails.Entity.RelationEntities = new List<Relation>();
                                    var companyModel = _mapper.Map<CompanyModel>(companyDetails);
                                    int socialCompany = await _sociableService.CreatePerson(null, companyModel, personModel.OrganizationId ?? 0);
                                    if(socialCompany > 0)
                                    {
                                        var companyEntityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(companyDetails.EntityId));
                                        companyEntityDetails.SociableUserId = socialCompany;
                                        socialCompanyId = socialCompany;
                                        var userInfo = await _sociableService.GetUserById(Convert.ToInt32(socialCompanyId), personModel.OrganizationId ?? 0);
                                        dynamic profile = JObject.Parse(userInfo);
                                        var profileId = profile.profile_profiles[0].target_id;
                                        if(profileId > 0)
                                        {
                                            var result = await _sociableService.UpdatePersonProfile(null, companyModel, (int)profileId, personModel.OrganizationId ?? 0);
                                        }
                                        companyEntityDetails.SociableProfileId = profileId;
                                        _unitOfWork.Entities.Update(companyEntityDetails);
                                        await _unitOfWork.CommitAsync();
                                    }
                                }
                            }
                        }
                       
                        person = await _unitOfWork.Persons.GetPersonDetailByIdAsync(person.PersonId);
                        personModel = _mapper.Map<PersonModel>(person);
                        personModel.SocialCompanyId = Convert.ToInt32(socialCompanyId);
                        int sociableUserId = await _sociableService.CreatePerson(personModel, null, personModel.OrganizationId ?? 0);

                        if (sociableUserId > 0)
                        {
                            entity.SociableUserId = sociableUserId;
                            //Get ProfileId and update Profile

                            var userInfo = await _sociableService.GetUserById(sociableUserId, personModel.OrganizationId ?? 0);

                            dynamic profile = JObject.Parse(userInfo);
                            var profileId = profile.profile_profiles[0].target_id;
                            if (profileId > 0)
                            {

                                var result = await _sociableService.UpdatePersonProfile(personModel, null, (int)profileId, personModel.OrganizationId ?? 0);
                            }
                            entity.SociableProfileId = profileId;
                            _unitOfWork.Entities.Update(entity);
                            await _unitOfWork.CommitAsync();
                        }
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError($"Create Person: Failed to create sociable profile: {entity.Name} {entity.WebLoginName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                    }
                }
            }
            return person;
        }
        public async Task<bool> DeletePerson(int id)
        {
            var Person = await _unitOfWork.Persons.GetPersonByIdAsync(id);
            if (Person != null)
            {
                if (Person.Entity != null)
                {
                    var memberGroup = await _unitOfWork.GroupMembers.GetGroupsByEntityIdAsync((int)Person.EntityId);

                    var billableContacts = await _unitOfWork.Companies.GetCompanyByBillableContactId(Person.PersonId);

                    if (Person.Entity.Memberships != null && Person.Entity.Memberships.Count > 0)
                    {
                        return false;
                    }
                    else if (Person.Entity.Documentobjectaccesshistories != null && Person.Entity.Documentobjectaccesshistories.Count > 0)
                    {
                        return false;
                    }
                    else if (Person.Entity.Paymenttransactions != null && Person.Entity.Paymenttransactions.Count > 0)
                    {
                        return false;
                    }
                    else if (Person.Entity.InvoiceEntities != null && Person.Entity.InvoiceEntities.Count > 0)
                    {
                        return false;
                    }
                    else if (Person.Entity.Entityroles != null && Person.Entity.Entityroles.Where(x => x.Status == (int)Status.Active).Count() > 0)
                    {
                        return false;
                    }
                    else if (memberGroup != null && memberGroup.Count() > 0)
                    {
                        return false;
                    }
                    else if (billableContacts != null && billableContacts.Count() > 0)
                    {
                        return false;
                    }

                    _unitOfWork.Persons.Remove(Person);
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
        public async Task<IEnumerable<Person>> GetAllPersons()
        {
            return await _unitOfWork.Persons
                .GetAllPersonsAsync();
        }

        private async Task<IEnumerable<Person>> FilterPersonByGroupId(IEnumerable<Person> persons, int exceptedPersonsGroupId = 0)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetAllGroupMembersByGroupIdAsync(exceptedPersonsGroupId);
            var existedPersonsId = groupMembers.Where(f => f.EntityId != null).Select(f => f.Entity.PersonId).ToArray();
            if (existedPersonsId != null)
            {
                persons = persons.Where(f => !existedPersonsId.Contains(f.PersonId)).ToList();
            }
            return persons;
        }

        private async Task<IEnumerable<Person>> FilterPersonByPersonIds(IEnumerable<Person> persons, string exceptedPersonsId)
        {
            var Ids = await GetExceptedMemberIds(exceptedPersonsId);
            if (Ids != null && Ids.Count() > 0)
            {
                persons = persons.Where(f => !Ids.Contains((int)f.EntityId)).ToList();
            }
            return persons;
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

        public async Task<IEnumerable<PersonModel>> GetAllPersonsByPhoneNumber(string phoneNumber, string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            var persons = await _unitOfWork.Persons.GetPersonByPhoneNumberAsync(phoneNumber.GetCleanPhoneNumber());
            if (exceptedPersonsGroupId > 0)
                persons = await FilterPersonByGroupId(persons, exceptedPersonsGroupId);

            if (!string.IsNullOrEmpty(exceptMemberIds))
                persons = await FilterPersonByPersonIds(persons, exceptMemberIds);

            List<PersonModel> model = new List<PersonModel>();

            if (phoneNumber.IsNullOrEmpty()) return model;

            model = _mapper.Map<List<PersonModel>>(persons);

            foreach (var item in model)
            {
                //Map additional fields

                var primaryAddress = item.Addresses.GetPrimaryAddress();
                item.StreetAddress = primaryAddress.StreetAddress;
                item.City = primaryAddress.City;
                item.State = primaryAddress.State;
                item.Zip = primaryAddress.Zip.FormatZip();

                item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
            }


            return model.OrderBy(x => x.PrimaryPhone);
        }
        public async Task<IEnumerable<PersonModel>> GetAllPersonsByEmail(string email, string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            var persons = await _unitOfWork.Persons.GetPersonsByEmaillAsync(email);
            if (exceptedPersonsGroupId > 0)
                persons = await FilterPersonByGroupId(persons, exceptedPersonsGroupId);
            if (!string.IsNullOrEmpty(exceptMemberIds))
                persons = await FilterPersonByPersonIds(persons, exceptMemberIds);

            List<PersonModel> model = new List<PersonModel>();

            if (email.IsNullOrEmpty()) return model;

            model = _mapper.Map<List<PersonModel>>(persons);

            foreach (var item in model)
            {
                //Map additional fields

                var primaryAddress = item.Addresses.GetPrimaryAddress();
                item.StreetAddress = primaryAddress.StreetAddress;
                item.City = primaryAddress.City;
                item.State = primaryAddress.State;
                item.Zip = primaryAddress.Zip.FormatZip();

                item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
            }

            return model.OrderBy(x => x.PrimaryEmail);
        }
        public async Task<IEnumerable<PersonModel>> GetAllPersonsByFirstAndLastName(string firstName, string lastName, string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            try
            {
                var persons = await _unitOfWork.Persons.GetPersonsByFirstAndLastNamelAsync(firstName, lastName);
                if (exceptedPersonsGroupId > 0)
                    persons = await FilterPersonByGroupId(persons, exceptedPersonsGroupId);
                if (!string.IsNullOrEmpty(exceptMemberIds))
                    persons = await FilterPersonByPersonIds(persons, exceptMemberIds);

                List<PersonModel> model = new List<PersonModel>();

                if (firstName.IsNullOrEmpty() && lastName.IsNullOrEmpty()) return model;

                model = _mapper.Map<List<PersonModel>>(persons);

                foreach (var item in model)
                {
                    //Map additional fields

                    var primaryAddress = item.Addresses.GetPrimaryAddress();
                    if (primaryAddress != null)
                    {
                        item.StreetAddress = primaryAddress.StreetAddress;
                        item.City = primaryAddress.City;
                        item.State = primaryAddress.State;
                        item.Zip = primaryAddress.Zip.FormatZip();
                    }
                    else
                    {
                        item.StreetAddress = string.Empty;
                        item.City = string.Empty;
                        item.State = string.Empty;
                        item.Zip = string.Empty;
                    }

                    item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                    item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
                }

                return model.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Search Person:  failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                throw ex;
            }
        }
        public async Task<IEnumerable<PersonModel>> GetAllPersonsByNameTitle(string text, string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            try
            {
                var persons = await _unitOfWork.Persons.GetPersonsByNameTitleAsync(text);
                if (exceptedPersonsGroupId > 0)
                    persons = await FilterPersonByGroupId(persons, exceptedPersonsGroupId);
                if (!string.IsNullOrEmpty(exceptMemberIds))
                    persons = await FilterPersonByPersonIds(persons, exceptMemberIds);

                List<PersonModel> model = new List<PersonModel>();

                if (text.IsNullOrEmpty()) return model;

                model = _mapper.Map<List<PersonModel>>(persons);

                foreach (var item in model)
                {
                    //Map additional fields

                    var primaryAddress = item.Addresses.GetPrimaryAddress();
                    if (primaryAddress != null)
                    {
                        item.StreetAddress = primaryAddress.StreetAddress;
                        item.City = primaryAddress.City;
                        item.State = primaryAddress.State;
                        item.Zip = primaryAddress.Zip.FormatZip();
                    }
                    else
                    {
                        item.StreetAddress = string.Empty;
                        item.City = string.Empty;
                        item.State = string.Empty;
                        item.Zip = string.Empty;
                    }

                    item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                    item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
                }

                return model.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Search Person:  failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                throw ex;
            }
        }

        public async Task<IEnumerable<PersonModel>> GetAllPersonsByName(string name, int exceptedPersonsGroupId = 0)
        {
            var persons = await _unitOfWork.Persons.GetPersonsByFirstORLastNameAsync(name);
            if (exceptedPersonsGroupId > 0)
                persons = await FilterPersonByGroupId(persons, exceptedPersonsGroupId);
            List<PersonModel> model = new List<PersonModel>();

            if (name.IsNullOrEmpty()) return model;

            model = _mapper.Map<List<PersonModel>>(persons);

            foreach (var item in model)
            {
                var primaryAddress = item.Addresses.GetPrimaryAddress();
                item.StreetAddress = primaryAddress.StreetAddress;
                item.City = primaryAddress.City;
                item.State = primaryAddress.State;
                item.Zip = primaryAddress.Zip.FormatZip();

                item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
            }

            return model.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        }

        public async Task<IEnumerable<PersonModel>> GetAllPersonsByPersonIds(string ids)
        {
            int[] personIds = ids.Split(',').Select(int.Parse).ToArray();
            var persons = await _unitOfWork.Persons.GetPersonByPersonIdsAsync(personIds);

            List<PersonModel> model = new List<PersonModel>();

            model = _mapper.Map<List<PersonModel>>(persons);

            foreach (var item in model)
            {
                //Map additional fields

                var primaryAddress = item.Addresses.GetPrimaryAddress();
                item.StreetAddress = primaryAddress.StreetAddress;
                item.City = primaryAddress.City;
                item.State = primaryAddress.State;
                item.Zip = primaryAddress.Zip.FormatZip();

                item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
            }

            return model.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        }
        public async Task<PersonModel> GetPersonById(int id)
        {
            var person = await _unitOfWork.Persons.GetPersonByIdAsync(id);

            PersonModel model = new PersonModel();

            if (person != null)
            {
                //Get EntityRoles
                var entityRoles = person.Entity.Entityroles;
                model = _mapper.Map<PersonModel>(person);
                model.EntityRoles = _mapper.Map<List<EntityRoleModel>>(entityRoles);
                //Map additional fields
                var primaryAddress = model.Addresses.GetPrimaryAddress();
                model.StreetAddress = primaryAddress.StreetAddress;
                model.City = primaryAddress.City;
                model.State = primaryAddress.State;
                model.Zip = primaryAddress.Zip.FormatZip();

                model.PrimaryEmail = model.Emails.GetPrimaryEmail();
                model.PrimaryPhone = model.Phones.GetPrimaryPhoneNumber();
            }

            return model;
        }

        public async Task<AddressModel> GetPrimaryAddressByPersonId(int personid)
        {
            var addresses = await _unitOfWork.Addresses.GetAddressByPersonIdAsync(personid);

            var addressModel = _mapper.Map<List<AddressModel>>(addresses);

            return addressModel.GetPrimaryAddress();
        }
        public async Task<bool> UpdatePerson(PersonModel PersonModel)
        {
            if (PersonModel.PersonId <= 0) return false;
            int? socialCompanyId = null;
            var isValidPerson = await ValidPerson(PersonModel);
            bool updateEntity = false;

            if (isValidPerson)
            {
                try
                {
                    var person = await _unitOfWork.Persons.GetPersonDetailByIdAsync(PersonModel.PersonId);

                    //get configuration info
                    var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(person.OrganizationId ?? 0);

                    //person = _mapper.Map<Person>(PersonModel);

                    if (person != null)
                    {
                        if (person.FirstName != PersonModel.FirstName || person.LastName != PersonModel.LastName)
                        {
                            updateEntity = true;
                        }

                        person.Prefix = PersonModel.Prefix;
                        person.FirstName = PersonModel.FirstName;
                        person.LastName = PersonModel.LastName;
                        person.MiddleName = PersonModel.MiddleName;
                        person.CasualName = PersonModel.CasualName;
                        person.Suffix = PersonModel.Suffix;
                        person.Gender = PersonModel.Gender;
                        person.PreferredContact = PersonModel.PreferredContact;
                        person.DateOfBirth = PersonModel.DateOfBirth;
                        person.Title = PersonModel.Title;
                        person.FacebookName = PersonModel.FacebookName;
                        person.InstagramName = PersonModel.InstagramName;
                        person.LinkedinName = PersonModel.LinkedinName;
                        person.SkypeName = PersonModel.SkypeName;
                        person.TwitterName = PersonModel.TwitterName;
                        person.Salutation = PersonModel.Salutation;
                        person.Website = PersonModel.Website;
                        person.Status = PersonModel.Status;
                        person.Designation = PersonModel.Designation;
                        person.MemberId = PersonModel.MemberId;

                        //Add / Update Emails
                        var emails = person.Emails.ToList();
                        //Check if primary email is unique 
                        var validEmail = await ValidatePrimaryEmail(PersonModel.Emails.Where(x => x.IsPrimary == (int)Status.Active).Select(x => x.EmailAddress).FirstOrDefault(), person.PersonId);

                        if (!validEmail)
                        {
                            throw new Exception("Please use a unique Email Address for Primary Email.");
                        }
                        foreach (var item in emails)
                        {
                            if (PersonModel.Emails.Any(x => x.EmailId == item.EmailId))
                            {
                                EmailModel emailModel = PersonModel.Emails.Where(x => x.EmailId == item.EmailId).FirstOrDefault();
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
                                    _logger.LogError($"Update Person: Update email failed: First Name: {person.FirstName} Last Name:{person.LastName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                                    throw new Exception($"Failed to update person email");
                                }
                                continue;

                            }

                            _unitOfWork.Emails.Remove(item);
                            person.Emails.Remove(item);
                        }

                        //Add  Emails
                        foreach (var item in PersonModel.Emails.Where(x => x.EmailId == 0).ToList())
                        {

                            Email email = new Email();
                            email.EmailAddressType = item.EmailAddressType;
                            email.EmailAddress = item.EmailAddress;
                            email.IsPrimary = item.IsPrimary;
                            try
                            {
                                person.Emails.Add(email);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Update Person: Update email failed: First Name: {person.FirstName} Last Name:{person.LastName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException.Message}");
                                throw new Exception($"Failed to update person email");
                            }
                        }

                        //Add / Update Phones
                        var phones = person.Phones.ToList();

                        foreach (var item in phones)
                        {
                            if (PersonModel.Phones.Any(x => x.PhoneId == item.PhoneId))
                            {
                                PhoneModel phone = PersonModel.Phones.Where(x => x.PhoneId == item.PhoneId).FirstOrDefault();
                                item.PhoneType = phone.PhoneType;
                                item.PhoneNumber = phone.PhoneNumber.GetCleanPhoneNumber();
                                item.IsPrimary = phone.IsPrimary;

                                _unitOfWork.Phones.Update(item);
                                continue;

                            }

                            _unitOfWork.Phones.Remove(item);
                            person.Phones.Remove(item);
                        }

                        //Add  Phones
                        foreach (var item in PersonModel.Phones.Where(x => x.PhoneId == 0).ToList())
                        {

                            Phone phone = new Phone();
                            phone.PhoneType = item.PhoneType;
                            phone.PhoneNumber = item.PhoneNumber.GetCleanPhoneNumber();
                            phone.IsPrimary = item.IsPrimary;
                            person.Phones.Add(phone);
                        }

                        //Add / Update Address
                        var addresses = person.Addresses.ToList();

                        foreach (var item in addresses)
                        {
                            if (PersonModel.Addresses.Any(x => x.AddressId == item.AddressId))
                            {
                                AddressModel address = PersonModel.Addresses.Where(x => x.AddressId == item.AddressId).FirstOrDefault();
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
                            person.Addresses.Remove(item);
                        }

                        //Add  Addresses
                        foreach (var item in PersonModel.Addresses.Where(x => x.AddressId == 0).ToList())
                        {

                            Address address = _mapper.Map<Address>(item);
                            person.Addresses.Add(address);
                        }


                    }
                    try
                    {
                        //create company
                        if (PersonModel.CompanyId == null)
                        {
                            if (!PersonModel.CompanyName.IsNullOrEmpty())
                            {
                                var checkCompany = await _companyService.GetCompaniesByName(PersonModel.CompanyName, string.Empty);
                                var exist = checkCompany.Where(x => x.CompanyName == PersonModel.CompanyName).ToList();
                                if (exist.Count() != 0)
                                {
                                    PersonModel.CompanyId = exist.FirstOrDefault().CompanyId;
                                }
                                else
                                {
                                    var companyModel = new CompanyModel();
                                    companyModel.CompanyName = PersonModel.CompanyName;
                                    companyModel.OrganizationId = PersonModel.OrganizationId ?? 1;
                                    var comapny = await _companyService.CreateCompany(companyModel);
                                    PersonModel.CompanyId = comapny.CompanyId;
                                }
                                if (PersonModel.CompanyId != null)
                                {
                                    var entityDetails = await _unitOfWork.Entities.GetEntityByCompanyIdAsync(Convert.ToInt32(PersonModel.CompanyId));
                                    if (entityDetails != null)
                                    {
                                        socialCompanyId = entityDetails.SociableUserId;
                                    }
                                }
                            }
                        }

                        var oldCompanyId = person.CompanyId;
                        person.CompanyId = PersonModel.CompanyId == 0 ? null : PersonModel.CompanyId;
                        _unitOfWork.Persons.Update(person);

                        if (configuration.DisplayRoles != null && configuration.DisplayRoles == (int)Status.Active)
                        {
                            if (PersonModel.CompanyId != oldCompanyId)
                            {
                                ContactActivityInputModel model = new ContactActivityInputModel();
                                model.EntityId = PersonModel.EntityId;
                                //create activity for account assigned
                                model.AccountId = Convert.ToInt32(PersonModel.CompanyId);
                                await _contactActivityService.CreateAccountChangeContactActivity(model, true);
                                //create activity for account unassigned
                                model.AccountId = Convert.ToInt32(oldCompanyId);
                                await _contactActivityService.CreateAccountChangeContactActivity(model, false);

                                var entityRoleList = await _unitOfWork.EntityRoles.GetActiveEntityRolesByEntityIdAsync(model.EntityId);
                                foreach (var entityRole in entityRoleList)
                                {
                                    entityRole.CompanyId = PersonModel.CompanyId;
                                    _unitOfWork.EntityRoles.Update(entityRole);
                                }
                                await _unitOfWork.CommitAsync();
                            }
                        }

                        //Add update EntityRoles
                        //var currentEntityRoles = await _unitOfWork.EntityRoles.GetAllEntityRolesByEntityIdAsync(PersonModel.EntityId);
                        var newEntityRoles = PersonModel.EntityRoles.ToList();
                        foreach (var item in newEntityRoles)
                        {
                            var checkEntityRoleExist = await _unitOfWork.EntityRoles.GetAllEntityRolesByEntityIdContactRoleIdAndCompanyIdAsync(PersonModel.EntityId, item.ContactRoleId, Convert.ToInt32(PersonModel.CompanyId));
                            if (checkEntityRoleExist.Count() == 0)
                            {
                                var entityRole = new EntityRoleModel();
                                entityRole.EntityId = PersonModel.EntityId;
                                entityRole.ContactRoleId = item.ContactRoleId;
                                entityRole.EffectiveDate = item.EffectiveDate;
                                entityRole.EndDate = item.EndDate;
                                entityRole.Status = item.Status;
                                entityRole.CompanyId = PersonModel.CompanyId ?? 0;
                                await _entityRoleService.CreateEntityRole(entityRole);
                            }
                        }

                        var entity = await _unitOfWork.Entities.GetByIdAsync(PersonModel.EntityId);
                        if (updateEntity)
                        {
                            if (entity != null)
                            {
                                entity.Name = $"{person.Prefix} {person.FirstName} {person.LastName}";
                                _unitOfWork.Entities.Update(entity);
                            }
                        }

                        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                        Match match = regex.Match(entity.WebLoginName);
                        if (string.IsNullOrEmpty(entity.WebLoginName) || match.Success)
                        {
                            string webLogin = string.Empty;
                            var userName = await _unitOfWork.Entities.GetEntityByUserNameAsync($"{person.FirstName}{person.LastName}");
                            if (userName == null)
                            {
                                webLogin = $"{person.FirstName}{person.LastName}";
                            }
                            else
                            {
                                webLogin = $"{person.FirstName}{person.LastName}{person.PersonId}";
                            }
                            entity.WebLoginName = webLogin;
                            if (string.IsNullOrEmpty(entity.WebPassword) || string.IsNullOrEmpty(entity.WebPasswordSalt))
                            {
                                PasswordHash hash = new PasswordHash(Guid.NewGuid().ToString());
                                entity.WebPasswordSalt = hash.Salt;
                                entity.WebPassword = hash.Password;
                            }
                            _unitOfWork.Entities.Update(entity);
                        }

                        await _unitOfWork.CommitAsync();

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Update Person:  UpdatePerson failed First Name: {person.FirstName} Last Name:{person.LastName} failed with error {ex.Message} {ex.StackTrace} {ex.InnerException?.Message}");
                        throw ex;
                    }

                    //Update Sociable if enabled
                    var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
                    if (configuration.SociableSyncEnabled == (int)Status.Active && staff != null)
                    {
                        try
                        {
                            //create company profile if it is not exist in social
                            if (socialCompanyId == null)
                            {
                                if (PersonModel.CompanyId != null)
                                {
                                    var companyDetails = await _unitOfWork.Companies.GetCompanyByIdAsync(Convert.ToInt32(PersonModel.CompanyId));
                                    if (companyDetails != null)
                                    {
                                        companyDetails.Entity.RelationEntities = new List<Relation>();
                                        var companyModel = _mapper.Map<CompanyModel>(companyDetails);
                                        int socialCompany = await _sociableService.CreatePerson(null, companyModel, PersonModel.OrganizationId ?? 0);
                                        if (socialCompany > 0)
                                        {
                                            var companyEntityDetails = await _unitOfWork.Entities.GetByIdAsync(Convert.ToInt32(companyDetails.EntityId));
                                            companyEntityDetails.SociableUserId = socialCompany;
                                            socialCompanyId = socialCompany;
                                            var userInfo = await _sociableService.GetUserById(Convert.ToInt32(socialCompanyId), PersonModel.OrganizationId ?? 0);
                                            dynamic profile = JObject.Parse(userInfo);
                                            var profileId = profile.profile_profiles[0].target_id;
                                            if (profileId > 0)
                                            {
                                                var result = await _sociableService.UpdatePersonProfile(null, companyModel, (int)profileId, PersonModel.OrganizationId ?? 0);
                                            }
                                            companyEntityDetails.SociableProfileId = profileId;
                                            _unitOfWork.Entities.Update(companyEntityDetails);
                                            await _unitOfWork.CommitAsync();
                                        }
                                    }
                                }
                            }

                            person = await _unitOfWork.Persons.GetPersonDetailByIdAsync(person.PersonId);
                            var personModel = _mapper.Map<PersonModel>(person);
                            int? socialUserId = null;
                            var entity = await _unitOfWork.Entities.GetByIdAsync(PersonModel.EntityId);
                            socialUserId = entity.SociableUserId;
                            if(socialUserId != null || socialUserId > 0)
                            {
                                var checkUserInfoExist = await _sociableService.GetUserById(Convert.ToInt32(entity.SociableUserId), personModel.OrganizationId ?? 0);
                                dynamic uInfo = JObject.Parse(checkUserInfoExist);
                                if (uInfo.uid == null)
                                {
                                    socialUserId = null;
                                }
                            }
                            if (socialUserId == null || socialUserId <= 0)
                            {
                                int sociableUserId = await _sociableService.CreatePerson(personModel, null, personModel.OrganizationId ?? 0);
                                if (sociableUserId > 0)
                                {
                                    entity.SociableUserId = sociableUserId;
                                    var userInfo = await _sociableService.GetUserById(sociableUserId, personModel.OrganizationId ?? 0);
                                    dynamic profile = JObject.Parse(userInfo);
                                    var profileId = profile.profile_profiles[0].target_id;
                                    entity.SociableProfileId = profileId;
                                    _unitOfWork.Entities.Update(entity);
                                    await _unitOfWork.CommitAsync();
                                    socialUserId = entity.SociableUserId;
                                }
                            }

                            if (socialUserId > 0)
                            {
                                //Update user primary email
                                var primaryEmail = personModel.Emails.GetPrimaryEmail();
                                string webPassword = string.Empty;

                                await _sociableService.UpdatePerson(entity.SociableUserId ?? 0, entity.WebLoginName, webPassword, primaryEmail, personModel.OrganizationId ?? 0, false, false, false);

                                if (entity.SociableProfileId > 0)
                                {

                                    var result = await _sociableService.UpdatePersonProfile(personModel, null, entity.SociableProfileId ?? 0, personModel.OrganizationId ?? 0);

                                    if (result <= 0)
                                    {
                                        throw new Exception($"Failed to update Sociable Profile:");
                                    }
                                }
                            }
                            return true;
                        }

                        catch (Exception ex)
                        {
                            _logger.LogError($"Update Person: Update sociable profile failed: First Name: {person.FirstName} Last Name:{person.LastName} failed with error {ex.Message} {ex.StackTrace}");
                            throw new Exception($"Failed to update Sociable Profile");
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Update Person:  failed with error {ex.Message} {ex.StackTrace} {ex.InnerException?.Message}");
                    throw new Exception(ex.Message);
                }
            }
            return false;
        }


        public async Task<IEnumerable<PersonModel>> GetAllPersonsByMembershipId(int membershipId)
        {
            var persons = await _unitOfWork.Persons.GetAllPersonsByMembershipIdAsync(membershipId);

            List<PersonModel> model = new List<PersonModel>();

            model = _mapper.Map<List<PersonModel>>(persons);

            foreach (var item in model)
            {
                //Map additional fields

                var primaryAddress = item.Addresses.GetPrimaryAddress();
                item.StreetAddress = primaryAddress.StreetAddress;
                item.City = primaryAddress.City;
                item.State = primaryAddress.State;
                item.Zip = primaryAddress.Zip.FormatZip();

                item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
            }
            return model.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        }

        public async Task<SearchModel> GetAllPersonsByQuickSearch(string quickSearch)
        {
            var searchModel = new SearchModel();
            List<PersonModel> model = new List<PersonModel>();
            List<CompanyModel> companyModel = new List<CompanyModel>();

            if (quickSearch.IsNullOrEmpty()) return searchModel;

            //Check if it is a invoice Serach

            if (quickSearch.isNumeric() && !quickSearch.IsPhoneNumber())
            {
                int invoiceId = int.Parse(quickSearch);
                var invoiceList = await _unitOfWork.Invoices.GetAllAsync();
                if (invoiceList.Any())
                {
                    searchModel.IsInvoiceModuleInUse = true;
                    var invoice = invoiceList.Where(f => f.InvoiceId == invoiceId).FirstOrDefault();
                    if (invoice != null)
                    {
                        var entity = await _unitOfWork.Entities.GetByIdAsync(invoice.BillableEntityId ?? 0);
                        if (entity.PersonId != null)
                        {
                            var person = await _unitOfWork.Persons.GetByIdAsync(entity.PersonId ?? 0);

                            if (person != null)
                            {
                                model.Add(_mapper.Map<PersonModel>(person));
                                searchModel.Data = model;
                                return searchModel;
                            }
                        }
                        else
                        {
                            var company = await _unitOfWork.Companies.GetByIdAsync(entity.CompanyId ?? 0);
                            if (company != null)
                            {
                                companyModel.Add(_mapper.Map<CompanyModel>(company));
                                searchModel.Data = companyModel;
                                return searchModel;
                            }
                        }
                    }
                    else
                    {
                        return searchModel;
                    }
                }
            }

            var persons = await _unitOfWork.Persons.GetPersonsByQuickSearchAsync(quickSearch);
            var companies = await _companyService.GetCompaniesByQuickSearchAsync(quickSearch);

            model = _mapper.Map<List<PersonModel>>(persons);

            foreach (var item in model)
            {
                //Map additional fields

                var primaryAddress = item.Addresses.GetPrimaryAddress();
                item.StreetAddress = primaryAddress.StreetAddress;
                item.City = primaryAddress.City;
                item.State = primaryAddress.State;
                item.Zip = primaryAddress.Zip.FormatZip();

                item.PrimaryEmail = item.Emails.GetPrimaryEmail();
                item.PrimaryPhone = item.Phones.GetPrimaryPhoneNumber();
            }
            var dict = new Dictionary<string, object>();
            dict.Add("person", model);
            dict.Add("company", companies);
            searchModel.Data = dict;//model.OrderBy(x => x.PrimaryEmail);
            return searchModel;
        }

        public async Task<IEnumerable<PersonModel>> GetPeopleByCompanyIdAsync(int companyId)
        {
            var persons = await _unitOfWork.Persons.GetPeopleByCompanyIdAsync(companyId);

            List<PersonModel> model = new List<PersonModel>();

            model = _mapper.Map<List<PersonModel>>(persons);
            return model.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
        }

        public async Task<IEnumerable<SelectListModel>> GetPeopleListByCompanyIdAsync(int companyId)
        {
            List<Person> people = new List<Person>();

            if (companyId != 0)
            {
                var persons = await _unitOfWork.Persons.GetPeopleByCompanyIdAsync(companyId);
                if (persons.Any())
                {
                    people = persons.ToList();
                }
            }
            else if (companyId == 0)
            {
                var persons = await _unitOfWork.Persons.GetPeopleWithNoAccount();
                if (persons.Any())
                {
                    people = persons.ToList();
                }
            }

            List<SelectListModel> peopleList = new List<SelectListModel>();

            foreach (var person in people)
            {
                SelectListModel item = new SelectListModel();
                item.name = string.Format("{0} {1}", person.FirstName, person.LastName);
                item.code = person.EntityId.ToString();
                peopleList.Add(item);
            }
            return peopleList;
        }

        public async Task<IEnumerable<SelectListModel>> GetAllPeopleList()
        {
            var people = await _unitOfWork.Persons
                 .GetAllPersonsAsync();
            List<SelectListModel> peopleList = people.Select(person => new SelectListModel
            {
                name = $"{person.FirstName} {person.LastName}",
                code = person.EntityId.ToString()
            }).ToList();
            return peopleList;

        }
        public async Task<bool> UpdateSociablePerson(EntitySociableModel entitySociableModel)
        {
            if (entitySociableModel.EntityId <= 0)
            {
                return false;
            }

            var entity = await _unitOfWork.Entities.GetByIdAsync(entitySociableModel.EntityId);
            var person = await _unitOfWork.Persons.GetPersonDetailByIdAsync(entity.PersonId ?? 0);
            var personModel = _mapper.Map<PersonModel>(person);

            var isValidPerson = await ValidPerson(personModel);
            bool updateEntity = false;

            if (person != null)
            {
                if (isValidPerson)
                {
                    if (person.FirstName != entitySociableModel.FirstName || person.LastName != entitySociableModel.LastName)
                    {
                        updateEntity = true;
                    }

                    person.Prefix = entitySociableModel.Prefix;
                    person.FirstName = entitySociableModel.FirstName;
                    person.LastName = entitySociableModel.LastName;
                    person.MiddleName = entitySociableModel.MiddleName;
                    person.CasualName = entitySociableModel.CasualName;
                    person.Suffix = entitySociableModel.Suffix;
                    person.Gender = entitySociableModel.Gender;
                    person.PreferredContact = entitySociableModel.PreferredContact;
                    person.DateOfBirth = entitySociableModel.DateOfBirth;
                    person.Title = entitySociableModel.Title;

                    //Update Primary Email
                    foreach (var item in person.Emails.Where(x => x.IsPrimary == 1))
                    {
                        item.EmailAddress = entitySociableModel.Email;
                        _unitOfWork.Emails.Update(item);
                        continue;
                    }

                    //Update Primary Phone
                    foreach (var item in person.Phones.Where(x => x.IsPrimary == 1))
                    {
                        item.PhoneNumber = entitySociableModel.Phone.GetCleanPhoneNumber();
                        _unitOfWork.Phones.Update(item);
                        continue;

                    }

                    //Update Primary Address
                    foreach (var item in person.Addresses.Where(x => x.IsPrimary == 1))
                    {
                        item.Address1 = entitySociableModel.StreetAddress;
                        item.City = entitySociableModel.City;
                        item.State = entitySociableModel.State;
                        item.Zip = entitySociableModel.Zip.Replace("_", "").Length == 6 ? entitySociableModel.Zip.Replace("_", "").Replace("-", "") : entitySociableModel.Zip;
                        _unitOfWork.Addresses.Update(item);
                        continue;
                    }

                    try
                    {
                        _unitOfWork.Persons.Update(person);

                        if (updateEntity)
                        {
                            if (entity != null)
                            {
                                entity.Name = $"{entitySociableModel.FirstName} {entitySociableModel.LastName}";
                                _unitOfWork.Entities.Update(entity);
                            }
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

        public async Task<bool> IsUniqueueEmail(string email)
        {
            if (email.IsNullOrEmpty()) return false;

            var persons = await _unitOfWork.Persons.GetPersonsByEmaillAsync(email);

            if (persons.Count() == 0) return true;

            return false;
        }

        public async Task<bool> IsUniqueueMemberPortalAccount(MemberPortalVerificationModel model)
        {
            var persons = await _unitOfWork.Persons.GetUniqPersonDetailByFirstNameLastNameAndPhoneNumberAsync(model.FirstName, model.LastName, model.PhoneNumber);
            if (persons.Count() == 0) return true;

            //Validate DateOfBirth

            try
            {
                DateTime dateOfBirth = DateTime.Parse(model.BirthDate);
                if (persons.Count() > 0)
                {
                    if (persons.Any(x => (x.DateOfBirth ?? DateTime.MinValue).Date == dateOfBirth.Date))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                             ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return false;
        }

        public async Task<bool> UpdatePersonComapny(PersonModel person)
        {
            bool companyUpdated = false;
            var personDetails = await _unitOfWork.Persons.GetByIdAsync(person.PersonId);
            var oldCompanyId = personDetails.CompanyId;
            if (personDetails != null)
            {
                personDetails.CompanyId = person.CompanyId;
                _unitOfWork.Persons.Update(personDetails);
                await _unitOfWork.CommitAsync();
                companyUpdated = true;
            }
            //get configuration info
            var configuration = await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(person.OrganizationId ?? 0);
            if (configuration.DisplayRoles != null && configuration.DisplayRoles == (int)Status.Active)
            {
                if (person.CompanyId != oldCompanyId)
                {
                    ContactActivityInputModel model = new ContactActivityInputModel();
                    model.EntityId = person.EntityId;
                    //create activity for account assigned
                    model.AccountId = Convert.ToInt32(person.CompanyId);
                    await _contactActivityService.CreateAccountChangeContactActivity(model, true);
                    //create activity for account unassigned
                    model.AccountId = Convert.ToInt32(oldCompanyId);
                    await _contactActivityService.CreateAccountChangeContactActivity(model, false);
                }
            }

            return companyUpdated;
        }

        private async Task<bool> ValidatePrimaryEmail(string primaryEmail, int personId)
        {
            var existingEmail = await _unitOfWork.Emails.GetPrimaryEmailByEmailAddressAsync(primaryEmail);

            if (existingEmail != null)
            {
                if (existingEmail.PersonId != personId)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<List<SearchPersonModel>> GetAllPersonsOrCompanyByNameAndEmail(string value, int entityId, string type)
        {
            List<SearchPersonModel> searchPersonModels = new List<SearchPersonModel>();
            var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
            if (staff != null)
            {
                var persons = await GetPersons(value, entityId);
                var companies = await GetCompanies(value, entityId);
                if (type == "people" && persons.Any())
                {
                    searchPersonModels.AddRange(persons);
                }
                else if (type == "company" && companies.Any())
                {
                    searchPersonModels.AddRange(companies);
                }
            }
            return searchPersonModels;
        }

        private async Task<List<SearchPersonModel>> GetPersons(string value, int entityId)
        {
            List<SearchPersonModel> searchPersonModels = new List<SearchPersonModel>();
            var users = await _unitOfWork.Persons.GetPersonByFirstORLastNameAsync(value);
            var existingRelationPersons = await _relationService.GetRelationsByEntityId(entityId);
            if (existingRelationPersons.Any())
            {
                var existingRelationEntityIds = existingRelationPersons.Select(s => s.RelatedEntityId);
                if (existingRelationEntityIds.Any())
                {
                    users = users.Where(s => !existingRelationEntityIds.Contains(s.EntityId));
                }
            }
            if (entityId != 0)
            {
                users = users.Where(s => s.EntityId != entityId);
            }
            foreach (var item in users)
            {
                SearchPersonModel searchPersonModel = new SearchPersonModel();
                searchPersonModel.PersonId = item.PersonId;
                searchPersonModel.EntityId = item.EntityId != null ? Convert.ToInt32(item.EntityId) : null;
                searchPersonModel.FirstName = item.FirstName;
                searchPersonModel.LastName = item.LastName;
                searchPersonModel.Gender = item.Gender;
                searchPersonModel.DateOfBirth = item.DateOfBirth != null ? item.DateOfBirth.Value.ToString("MM/dd/yyyy") : null;
                var primaryEmail = item.Emails.FirstOrDefault(s => s.IsPrimary == 1);
                if (primaryEmail != null)
                {
                    searchPersonModel.Email = primaryEmail.EmailAddress;
                }
                var primaryPhone = item.Phones.FirstOrDefault(s => s.IsPrimary == 1);
                if (primaryPhone != null)
                {
                    searchPersonModel.Phone = primaryPhone.PhoneNumber.FormatPhoneNumber();
                }
                searchPersonModels.Add(searchPersonModel);
            }
            return searchPersonModels;
        }

        private async Task<List<SearchPersonModel>> GetCompanies(string value, int entityId)
        {
            List<SearchPersonModel> searchPersonModels = new List<SearchPersonModel>();
            var companies = await _unitOfWork.Companies.GetCompaniesByName(value);
            var existingRelationPersons = await _relationService.GetRelationsByEntityId(entityId);
            if (existingRelationPersons.Any())
            {
                var existingRelationEntityIds = existingRelationPersons.Select(s => s.RelatedEntityId);
                if (existingRelationEntityIds.Any())
                {
                    companies = companies.Where(s => !existingRelationEntityIds.Contains(s.EntityId) && s.EntityId != entityId);
                }
            }
            if (entityId != 0)
            {
                companies = companies.Where(s => s.EntityId != entityId);
            }
            foreach (var item in companies)
            {
                SearchPersonModel searchPersonModel = new SearchPersonModel();
                searchPersonModel.compnyId = item.CompanyId;
                searchPersonModel.EntityId = item.EntityId != null ? Convert.ToInt32(item.EntityId) : null;
                searchPersonModel.FirstName = item.CompanyName;
                var primaryEmail = item.Emails.FirstOrDefault(s => s.IsPrimary == 1);
                if (primaryEmail != null)
                {
                    searchPersonModel.Email = primaryEmail.EmailAddress;
                }
                var primaryPhone = item.Phones.FirstOrDefault(s => s.IsPrimary == 1);
                if (primaryPhone != null)
                {
                    searchPersonModel.Phone = primaryPhone.PhoneNumber.FormatPhoneNumber();
                }
                searchPersonModels.Add(searchPersonModel);
            }
            return searchPersonModels;
        }

        public async Task<PersonModel> GetLastAddedPerson()
        {
            var person = await _unitOfWork.Persons.GetLastAddedPersonAsync();
            var personModel = _mapper.Map<PersonModel>(person);
            return personModel;
        }

        private Task<bool> ValidPerson(PersonModel PersonModel)
        {
            if (PersonModel.LastName == null)
            {
                throw new NullReferenceException($"Person Last Name can not be NULL.");
            }

            if (PersonModel.FirstName == null)
            {
                throw new NullReferenceException($"Person First Name can not be NULL.");
            }

            //Validate  Name
            if (PersonModel.FirstName.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Person First Name can not be empty.");
            }

            if (PersonModel.LastName.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Person Last Name can not be empty.");
            }

            return Task.FromResult(true);
        }
    }
}