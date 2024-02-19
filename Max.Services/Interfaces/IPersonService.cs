using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllPersons();
        Task<IEnumerable<PersonModel>> GetAllPersonsByPhoneNumber(string phone, string exceptMemberIds, int exceptedPersonsGroupId = 0);
        Task<IEnumerable<PersonModel>> GetAllPersonsByEmail(string  email, string exceptMemberIds, int exceptedPersonsGroupId = 0);
        Task<IEnumerable<PersonModel>> GetAllPersonsByFirstAndLastName(string firstName, string lastName, string exceptMemberIds, int exceptedPersonsGroupId = 0);
        Task<IEnumerable<PersonModel>> GetAllPersonsByNameTitle(string text, string exceptMemberIds, int exceptedPersonsGroupId = 0);
        Task<IEnumerable<PersonModel>> GetAllPersonsByPersonIds(string ids);
        Task<PersonModel> GetPersonById(int id);
        Task<AddressModel> GetPrimaryAddressByPersonId(int id);
        Task<Person> CreatePerson(PersonModel person);
        Task<bool> UpdatePerson(PersonModel person);
        Task<bool> UpdatePersonComapny(PersonModel person);
        Task<bool> DeletePerson(int personId);
        Task<bool> IsUniqueueEmail(string emailAddress);
        //Task<bool> UpdatePersonWebLoginPasword(WebLoginModel model);
        Task<IEnumerable<PersonModel>> GetAllPersonsByMembershipId(int membershipId);
        Task<IEnumerable<PersonModel>> GetPeopleByCompanyIdAsync(int companyId);
        Task<IEnumerable<SelectListModel>> GetPeopleListByCompanyIdAsync(int companyId);
        Task<SearchModel> GetAllPersonsByQuickSearch(string quickSearch);
        Task<IEnumerable<SelectListModel>> GetAllPeopleList();
        Task<bool> UpdateSociablePerson(EntitySociableModel entitySociableModel);
        Task<IEnumerable<PersonModel>> GetAllPersonsByName(string name, int exceptedPersonsGroupId = 0);
        Task<bool> IsUniqueueMemberPortalAccount(MemberPortalVerificationModel model);
        Task<List<SearchPersonModel>> GetAllPersonsOrCompanyByNameAndEmail(string value, int entityId, string type);
        Task<PersonModel> GetLastAddedPerson();
    }
}