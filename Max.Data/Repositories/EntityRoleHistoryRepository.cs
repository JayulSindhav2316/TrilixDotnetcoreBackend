using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Max.Core;
using System.ComponentModel.Design;

namespace Max.Data.Repositories
{
    public class EntityRoleHistoryRepository : Repository<Entityrolehistory>, IEntityRoleHistoryRepository
    {
        public EntityRoleHistoryRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryAsync()
        {
            return await membermaxContext.Entityrolehistories
                .ToListAsync();
        }

        public async Task<Entityrolehistory> GetEntityRoleHistoryByIdAsync(int id)
        {
            return await membermaxContext.Entityrolehistories
                .SingleOrDefaultAsync(x => x.EntityRoleHistoryId == id);
        }

        public async Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Entityrolehistories
                .Include(x => x.Company)
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Entityroles)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
                .Where(x => x.EntityId == entityId && x.Status == (int)Status.Active
                             && x.IsDeleted != (int)Status.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entityrolehistory>> GetEntityRoleHistoryByTypeAsync(int entityId,int companyId,string type)
        {
            return await membermaxContext.Entityrolehistories
                .Include(x => x.Company)
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Entityroles)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
                .Where(x => x.EntityId == entityId 
                         && x.CompanyId==companyId 
                         && x.ActivityType==type 
                         && x.Status == (int)Status.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entityrolehistory>> GetEntityRoleHistoryAsync(int entityId, int companyId, int roleId)
        {
            return await membermaxContext.Entityrolehistories
                .Include(x => x.Company)
                .Include(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Entityroles)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Companies)
                .Where(x => x.EntityId == entityId
                         && x.CompanyId == companyId
                         && x.ContactRoleId == roleId
                         && x.Status == (int)Status.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryByCompanyIdAsync(int companyId)
        {
            return await membermaxContext.Entityrolehistories
                .Where(x => x.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<bool> DeleteHistoryByEntityIdContactRoleId(int entityId, int contactRoleId)
        {
            var data = await membermaxContext.Entityrolehistories
                 .Where(x => x.EntityId == entityId && x.ContactRoleId == contactRoleId)
                 .ToListAsync();
            membermaxContext.Entityrolehistories.RemoveRange(data);
            return true;
        }

        public async Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryByRoleIdAsync(int roleId)
        {
            return await membermaxContext.Entityrolehistories
                .Where(x => x.ContactRoleId == roleId)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
