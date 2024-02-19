using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IPersonRepository : IRepository<Person>
    {
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task<Person> GetPersonProfileByIdAsync(int id);
        Task<Person> GetPersonByIdAsync(int id);
        Task<Person> GetPersonDetailByIdAsync(int id);
        Task<IEnumerable<Person>> GetPersonByPersonIdsAsync(int[] ids);
        Task<IEnumerable<Person>> GetPersonByPhoneNumberAsync(string phoneNumer);
        Task<IEnumerable<Person>> GetPersonsByEmaillAsync(string email);
        Task<IEnumerable<Person>> GetPersonsByNameTitleAsync(string text);
        Task<IEnumerable<Person>> GetPersonsByFirstAndLastNamelAsync(string firstName, string lastName);
        Task<IEnumerable<Person>> GetAllPersonsByMembershipIdAsync(int membershipId);
        Task<Person> GetEmailsByPersonId(int personId);
        Task<IEnumerable<Person>> GetPersonsByQuickSearchAsync(string quickSearch);
        Task<Person> GetPersonByEntityIdAsync(int entityid);
        Task<Person> GetPersonByEmailIdAsync(string email);
        Task<IEnumerable<Person>> GetPersonsByFirstORLastNameAsync(string name);
        Task<IEnumerable<Person>> GetCompanyPersonsByFirstAndLastNameAsync(string firstName, string lastName, int companyId);
        Task<IEnumerable<Person>> GetCompanyPersonsByCompanyIdAsync(int companyId);
        Task<IEnumerable<Person>> GetActiveCompanyPersonsByCompanyIdAsync(int companyId);
        Task<IEnumerable<Person>> GetPeopleByCompanyIdAsync(int companyId);
        Task<IEnumerable<Person>> GetPeopleWithNoAccount();
        Task<IEnumerable<Person>> GetUniqPersonDetailByFirstNameLastNameAndPhoneNumberAsync(string firstName, string lastName, string phoneNumber);
        Task<IEnumerable<Person>> GetPersonByFirstORLastNameAsync(string value);
        Task<IEnumerable<Person>> GetCompanyPersonsByNameAsync(string name, int companyId);
        Task<Person> GetLastAddedPersonAsync();
    }

}
