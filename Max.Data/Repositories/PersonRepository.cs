using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System.Numerics;
using Max.Core.Helpers;
using System;
using System.Xml.Linq;
using System.ComponentModel.Design;

namespace Max.Data.Repositories
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await membermaxContext.People
                .ToListAsync();
        }

        public async Task<Person> GetPersonProfileByIdAsync(int id)
        {
            var person = await membermaxContext.People
                            .Include(a => a.Addresses).DefaultIfEmpty()
                            .Include(p => p.Phones).DefaultIfEmpty()
                            .Include(e => e.Emails).DefaultIfEmpty()
                            .Include(c => c.Company).DefaultIfEmpty()
                            .AsNoTracking()
                            .SingleOrDefaultAsync(m => m.PersonId == id);
            return person;
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            return await membermaxContext.People
                .Include(x => x.Entity)
                .Include(x => x.Entity.Entityroles)
                .Include(a => a.Entity.Paymenttransactions)
                .Include(a => a.Entity.Memberships)
                .Include(a => a.Entity.Documentobjectaccesshistories)
                .Include(a => a.Entity.InvoiceEntities)
                .Include(a => a.Addresses).DefaultIfEmpty()
                .Include(p => p.Phones).DefaultIfEmpty()
                .Include(e => e.Emails).DefaultIfEmpty()
                .Include(c => c.Company).DefaultIfEmpty()
                .Include(c => c.Company).DefaultIfEmpty()
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.PersonId == id);
        }

        public async Task<Person> GetPersonDetailByIdAsync(int id)
        {
            return await membermaxContext.People
                .Where(m => m.PersonId == id)
                .Include(a => a.Addresses)
                .Include(p => p.Phones)
                .Include(e => e.Emails)
                .Include(c => c.Company)
                .Include(x => x.Entity)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonByPhoneNumberAsync(string phoneNumber)
        {
            return await membermaxContext.People
                .Where(c => c.Phones.Any(p => p.PhoneNumber.StartsWith(phoneNumber)))
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                        .ThenInclude(x => x.MembershipType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonsByEmaillAsync(string email)
        {
            return await membermaxContext.People
               .Where(c => c.Emails.Any(p => p.EmailAddress.StartsWith(email)))
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                 .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                        .ThenInclude(x => x.MembershipType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonsByFirstAndLastNamelAsync(string firstName, string lastName)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            if ((!firstName.IsNullOrEmpty()) && lastName.IsNullOrEmpty())
            {
                predicate = predicate.And(x => x.FirstName.StartsWith(firstName));
            }
            else if ((firstName.IsNullOrEmpty()) && (!lastName.IsNullOrEmpty()))
            {
                predicate = predicate.And(x => x.LastName.StartsWith(lastName));
            }
            else if ((!firstName.IsNullOrEmpty()) && (!lastName.IsNullOrEmpty()))
            {
                predicate = predicate.And(x => x.FirstName.StartsWith(firstName));
                predicate = predicate.And(x => x.LastName.StartsWith(lastName));
            }


            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                        .ThenInclude(x => x.MembershipType)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonsByNameTitleAsync(string text)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            if (!text.IsNullOrEmpty())
            {
                var searchList = text.Trim().Split(' ').ToList();
                if (searchList.Count > 1)
                {
                    var firstName = searchList.FirstOrDefault();
                    var lastName = searchList.LastOrDefault();
                    predicate = predicate.And(x => x.FirstName.StartsWith(firstName)
                                  || x.FirstName.Contains(firstName));
                    predicate = predicate.And(x => x.LastName.StartsWith(lastName)
                                    || x.LastName.Contains(lastName));
                }
                else
                {
                    predicate = predicate.And(x => x.FirstName.StartsWith(text)
                                    || x.FirstName.Contains(text));
                    predicate = predicate.Or(x => x.LastName.StartsWith(text)
                                    || x.LastName.Contains(text));
                    predicate = predicate.Or(x => x.CasualName.StartsWith(text)
                                    || x.CasualName.Contains(text));
                    predicate = predicate.Or(x => x.Title.StartsWith(text)
                                    || x.Title.Contains(text));
                }
            }


            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                        .ThenInclude(x => x.MembershipType)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Person>> GetCompanyPersonsByNameAsync(string name, int companyId)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();
            predicate = predicate.And(x => x.CompanyId == companyId);

            if (!name.IsNullOrEmpty())
            {
                var searchList = name.Trim().Split(' ').ToList();
                if (searchList.Count > 1)
                {
                    var firstName = searchList.FirstOrDefault();
                    var lastName = searchList.LastOrDefault();
                    predicate = predicate.And(x => (x.FirstName.StartsWith(firstName)
                                  || x.FirstName.Contains(firstName)));
                    predicate = predicate.And(x => (x.LastName.StartsWith(lastName)
                                    || x.LastName.Contains(lastName)));
                }
                else
                {
                    predicate = predicate.And(x => (x.FirstName.StartsWith(name)
                                    || x.FirstName.Contains(name))
                                    || (x.LastName.StartsWith(name) || x.LastName.Contains(name)));
                }
            }

            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Entityroles.Where(x => x.Status == (int)Status.Active))
                        .ThenInclude(x => x.ContactRole)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetCompanyPersonsByFirstAndLastNameAsync(string firstName, string lastName, int companyId)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            predicate = predicate.And(x => x.CompanyId == companyId);
            if ((!firstName.IsNullOrEmpty()) && lastName.IsNullOrEmpty())
            {
                predicate = predicate.And(x => x.FirstName.StartsWith(firstName));
            }
            else if ((firstName.IsNullOrEmpty()) && (!lastName.IsNullOrEmpty()))
            {
                predicate = predicate.And(x => x.LastName.StartsWith(lastName));
            }
            else if ((!firstName.IsNullOrEmpty()) && (!lastName.IsNullOrEmpty()))
            {
                predicate = predicate.And(x => x.FirstName.StartsWith(firstName));
                predicate = predicate.And(x => x.LastName.StartsWith(lastName));
            }


            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Entityroles.Where(x => x.Status == (int)Status.Active))
                        .ThenInclude(x => x.ContactRole)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetCompanyPersonsByCompanyIdAsync(int companyId)
        {

            return await membermaxContext.People
                .Where(x => x.CompanyId == companyId)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Entityroles)
                        .ThenInclude(x => x.ContactRole)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetActiveCompanyPersonsByCompanyIdAsync(int companyId)
        {

            return await membermaxContext.People
                .Where(x => x.CompanyId == companyId && !x.Entity.Entityroles.Any(er => er.IsDeleted == (int)Status.Active))
                .Include(x => x.Company)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Entityroles.Where(x => x.Status == (int)Status.Active))
                        .ThenInclude(x => x.ContactRole)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonByPersonIdsAsync(int[] personIds)
        {
            return await membermaxContext.People
               .Where(x => personIds.Contains(x.PersonId))
               .Include(i => i.Phones)
               .Include(e => e.Emails)
               .Include(a => a.Addresses)
                .Include(x => x.Entity)
               .ThenInclude(m => m.Membershipconnections)
               .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
               .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetAllPersonsByMembershipIdAsync(int membershipId)
        {
            return await membermaxContext.People
                 .Include(x => x.Entity)
                    .ThenInclude(x => x.Membershipconnections)
               .Include(i => i.Phones)
               .Include(e => e.Emails)
               .Include(a => a.Addresses)
               .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
               .Where(x => x.Entity.Memberships.Any(x => x.MembershipId == membershipId))
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<Person> GetEmailsByPersonId(int personId)
        {
            return await membermaxContext.People
                .Include(x => x.Emails)
                .Where(x => x.PersonId == personId)
                .FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<Person>> GetPersonsByQuickSearchAsync(string searchParameter)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            if ((!searchParameter.IsNullOrEmpty()))
            {
                var searchList = searchParameter.Trim().Split(' ').ToList();
                if (searchParameter.Contains("@"))
                {
                    predicate = predicate.And(x => x.Emails.Any(p => p.EmailAddress.StartsWith(searchParameter)));
                }
                else if (searchParameter.IsPhoneNumber())
                {
                    string cleanPhoneNumber = searchParameter.GetCleanPhoneNumber();
                    if (cleanPhoneNumber.Length > 0)
                    {
                        predicate = predicate.And(x => x.Phones.Any(p => p.PhoneNumber.StartsWith(cleanPhoneNumber)));
                    }
                }
                else if (searchList.Count > 1 && !searchParameter.IsPhoneNumber())
                {
                    var firstName = searchList.FirstOrDefault();
                    var lastName = searchList.LastOrDefault();
                    predicate = predicate.And(x => x.FirstName.StartsWith(firstName)
                                  || x.FirstName.Contains(firstName));
                    predicate = predicate.And(x => x.LastName.StartsWith(lastName)
                                    || x.LastName.Contains(lastName));
                }
                else
                {
                    predicate = predicate.And(x => x.FirstName.StartsWith(searchParameter) || x.LastName.StartsWith(searchParameter) || x.Addresses.Any(p => p.Address1.StartsWith(searchParameter)));
                }

            }
            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .Include(a => a.Addresses)
                .Include(x => x.Entity)
                    .ThenInclude(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                        .ThenInclude(x => x.MembershipType)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .AsNoTracking()
                .Take(100)
                .ToListAsync();
        }

        public async Task<Person> GetPersonByEntityIdAsync(int entityid)
        {
            return await membermaxContext.People
                .Where(m => m.EntityId == entityid)
                .Include(a => a.Addresses)
                .Include(p => p.Phones)
                .Include(e => e.Emails)
                .Include(c => c.Company)
                .Include(x => x.Entity)
                .AsNoTracking()
                .SingleOrDefaultAsync();
        }

        public async Task<Person> GetPersonByEmailIdAsync(string email)
        {
            return await membermaxContext.People
               .Where(c => c.Emails.Any(p => p.EmailAddress.StartsWith(email)))
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                 .Include(x => x.Entity)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Person>> GetPeopleByCompanyIdAsync(int companyId)
        {
            return await membermaxContext.People
               .Where(c => c.CompanyId == companyId)
                .Include(x => x.Entity)
                .Include(x => x.Company)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<IEnumerable<Person>> GetPersonsByFirstORLastNameAsync(string name)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            if ((!name.IsNullOrEmpty()))
            {
                predicate = predicate.And(x => x.FirstName.StartsWith(name) || x.LastName.StartsWith(name));
            }
            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .AsNoTracking()
                .Take(100)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetUniqPersonDetailByFirstNameLastNameAndPhoneNumberAsync(string firstName, string lastName, string phoneNumber)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            var cleanPhoneNumber = phoneNumber.GetCleanPhoneNumber();

            predicate = predicate.And(x => x.FirstName == firstName && x.LastName == lastName);
            predicate = predicate.And(x => x.Phones.Any(x => x.PhoneNumber == cleanPhoneNumber));
            return await membermaxContext.People
                .Where(predicate)
                .Include(i => i.Phones)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPeopleWithNoAccount()
        {
            return await membermaxContext.People
              .Where(c => c.CompanyId == null)
               .Include(x => x.Entity)
               .Include(x => x.Company)
               .AsNoTracking()
               .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetPersonByFirstORLastNameAsync(string value)
        {
            return await membermaxContext.People
                .Where(x => x.FirstName.StartsWith(value) || x.LastName.StartsWith(value))
                .Where(x => x.Status == 1)
                .Include(x => x.Entity)
                .Include(i => i.Phones)
                .Include(e => e.Emails)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Person> GetLastAddedPersonAsync()
        {
            return await membermaxContext.People
                 .OrderByDescending(x => x.PersonId).FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }

}
