using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core.Helpers;
using Max.Core;

namespace Max.Data.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await membermaxContext.Companies
                .Where(c => c.EntityId != null)
                .ToListAsync();
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await membermaxContext.Companies
                .Include(x => x.Entity.Paymenttransactions)
                .Include(x => x.Entity.Documentobjectaccesshistories)
                .Include(x => x.Entity.Memberships)
                .Include(x => x.Entity.InvoiceEntities)
                .Include(x => x.Entity.RelationEntities)
                .Include(x => x.Entity.People)
                .Include(a => a.Addresses).DefaultIfEmpty()
                .Include(a => a.Emails).DefaultIfEmpty()
                .Include(a => a.Phones).DefaultIfEmpty()
                .SingleOrDefaultAsync(m => m.CompanyId == id && m.EntityId != null);
        }
        public async Task<Company> GetCompanyProfileByIdAsync(int id)
        {
            return await membermaxContext.Companies
                .SingleOrDefaultAsync(m => m.CompanyId == id && m.EntityId != null);
        }

        public async Task<IEnumerable<Company>> GetCompaniesByName(string name)
        {
            return await membermaxContext.Companies
               .Include(x => x.Addresses).DefaultIfEmpty()
               .Include(x => x.Phones).DefaultIfEmpty()
               .Include(x => x.Emails).DefaultIfEmpty()
               .Where(m => m.CompanyName.ToUpper().Contains(name.ToUpper()) && m.EntityId != null)
               .ToListAsync();
        }
        public async Task<IEnumerable<Company>> GetCompaniesByQuickSearchAsync(string searchParameter)
        {
            var predicate = PredicateBuilder.True<Company>();
            if ((!searchParameter.IsNullOrEmpty()))
            {
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
                else
                {
                    predicate = predicate.And(x => x.CompanyName.ToUpper().Contains(searchParameter.ToUpper()) || x.Addresses.Any(p => p.Address1.Contains(searchParameter)));
                }

            }
            return await membermaxContext.Companies
               .Include(x => x.Addresses).DefaultIfEmpty()
               .Include(x => x.Phones).DefaultIfEmpty()
               .Include(x => x.Emails).DefaultIfEmpty()
               .Where(predicate)
               .AsNoTracking()
               .Take(100)
               .ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetCompanyByBillableContactId(int id)
        {
            return await membermaxContext.Companies
                .Where(m => m.BillableContactId == id && m.EntityId != null).ToListAsync();
        }

        public async Task<Company> GetCompanyByEmailIdAsync(string email)
        {
            return await membermaxContext.Companies
               .Where(c => c.Emails.Any(p => p.EmailAddress.StartsWith(email)))
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        public async Task<Company> GetLastAddedCompanyAsync()
        {
            return await membermaxContext.Companies
                 .AsNoTracking()
                 .OrderByDescending(x => x.CompanyId)
                 .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
