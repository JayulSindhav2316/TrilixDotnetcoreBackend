using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Max.Core;
using SolrNet.Utils;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class EntityRoleRepository : Repository<Entityrole>, IEntityRoleRepository
    {
        public EntityRoleRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Entityrole>> GetAllEntityRolesAsync()
        {
            return await membermaxContext.Entityroles
                .ToListAsync();
        }

        public async Task<Entityrole> GeActiveEntityRoleByIdAsync(int id)
        {
            return await membermaxContext.Entityroles
                //.AsNoTracking()
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                   .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
                .Include(x => x.Company)
                .SingleOrDefaultAsync(x => x.EntityRoleId == id
                 && x.IsDeleted != (int)Status.Active  
                 && x.Status == (int)Status.Active);
        }

        public async Task<Entityrole> GetEntityRoleByIdAsync(int id)
        {
            return await membermaxContext.Entityroles
                .AsNoTracking()
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                   .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
                .Include(x => x.Company)
                .SingleOrDefaultAsync(x => x.EntityRoleId == id);
        }

        public async Task<IEnumerable<Entityrole>> GetAllEntityRolesByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Company)
                .Where(x => x.EntityId == entityId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Entityrole>> GetAllEntityRolesByCompanyIdAsync(int companyId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Where(x => x.CompanyId == companyId)
                .OrderBy(x => x.ContactRole.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entityrole>> GetActiveEntityRolesByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                     .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                      .ThenInclude(x => x.People)
                .Include(x => x.Company)
                .Where(x => x.EntityId == entityId
                     && x.IsDeleted != (int)Status.Active
                    && x.Status == (int)Status.Active)
                .ToListAsync();
        }
        public async Task<IEnumerable<Entityrole>> GetEntityRolesByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                     .ThenInclude(x => x.People)
                .Include(x => x.Company)
                .Where(x => x.EntityId == entityId && x.IsDeleted != (int)Status.Active)
                .ToListAsync();
        }
        public async Task<List<Entityrole>> GetActiveEntityRolesByCompanyIdAsync(int companyId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Phones)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Emails)
                .Include(x => x.Company)
                .Where(x => x.CompanyId == companyId
                      && x.IsDeleted != (int)Status.Active
                      && x.Status == (int)Status.Active)
                .OrderBy(x => x.EntityId)
                .ToListAsync();
        }
        public async Task<List<Entityrole>> GetEntityByRoleAndCompanyIdAsync(int roleId, int companyId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Phones)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Emails)
                .Include(x => x.Company)
                .Where(x => x.CompanyId == companyId && x.ContactRoleId == roleId
                   && x.IsDeleted != (int)Status.Active)
                .OrderBy(x => x.EntityId)
                .ToListAsync();
        }
        public async Task<List<Entityrole>> GetContactsByFirstAndLastNameAsync(string firstName, string lastName, int companyId)
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

            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Phones)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Emails)
                .Where(x => x.CompanyId == companyId)
                .OrderBy(x => x.EntityId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Entityrole>> GetAllEntityRolesByEntityIdContactRoleIdAndCompanyIdAsync(int entityId, int contactRoleId, int companyId)
        {
            return await membermaxContext.Entityroles
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                     .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                      .ThenInclude(x => x.People)
                .Include(x => x.Company)
                .Include(x => x.ContactRole)
                .Where(x => x.EntityId == entityId && x.ContactRoleId == contactRoleId && x.CompanyId == companyId && x.Status == (int)Status.Active)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
