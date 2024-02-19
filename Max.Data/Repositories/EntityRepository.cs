using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class EntityRepository : Repository<Entity>, IEntityRepository
    {
        public EntityRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Entity>> GetAllEntitiesAsync()
        {
            return await membermaxContext.Entities
                .ToListAsync();
        }

        public async Task<Entity> GetEntityByIdAsync(int id)
        {
            return await membermaxContext.Entities
                .Include(x => x.People)
                .Include(x => x.Companies)
                .Include(x => x.Memberships)
                .Include(x => x.InvoiceEntities)
                .SingleOrDefaultAsync(m => m.EntityId == id);
        }

        public Entity GetEntityById(int id)
        {
            return membermaxContext.Entities
                .Include(x => x.People)
                .Include(x => x.Companies)
                .AsNoTracking()
                .SingleOrDefault(m => m.EntityId == id);
        }

        public async Task<Entity> GetEntityByPersonIdAsync(int id)
        {
            return await membermaxContext.Entities
                .Include(x => x.People)
                .SingleOrDefaultAsync(m => m.PersonId == id);
        }

        public async Task<Entity> GetEntityByCompanyIdAsync(int id)
        {
            return await membermaxContext.Entities
                .SingleOrDefaultAsync(m => m.CompanyId == id);
        }

        public async Task<IEnumerable<Entity>> GetEntitiesByNameAsync(string name)
        {
            return await membermaxContext.Entities
                .Where(x => x.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<Entity> GetEntityDetailsByIdAsync(int id)
        {
            return await membermaxContext.Entities
                .Include(x => x.Organization)
                .Include(x => x.People)
                .Include(i => i.InvoiceEntities)
                   .ThenInclude(i => i.Invoicedetails)
                     .ThenInclude(i => i.Receiptdetails).DefaultIfEmpty()
                 .Include(i => i.InvoiceEntities)
                   .ThenInclude(i => i.Invoicedetails)
                     .ThenInclude(i => i.Receiptdetails)
                        .ThenInclude(i => i.Refunddetails).DefaultIfEmpty()
                 .Include(i => i.InvoiceEntities)
                   .ThenInclude(i => i.Invoicedetails)
                        .ThenInclude(i => i.Writeoffs)
                .Include(c => c.Communications).DefaultIfEmpty()
                .Include(n => n.Notes).DefaultIfEmpty()
                .Include(m => m.Paymentprofiles).DefaultIfEmpty()
                .Include(m => m.Membershipconnections)
                    .ThenInclude(x => x.Membership)
                      .ThenInclude(m => m.MembershipType)
               .Include(c => c.Credittransactions).DefaultIfEmpty()
               .Include(b => b.Groupmembers)
                .ThenInclude(g => g.Group)
               .AsNoTracking()
               .SingleOrDefaultAsync(m => m.EntityId == id);
        }

        public async Task<Entity> GetMembershipDetailByEntityId(int id)
        {

            return await membermaxContext.Entities
                   .Where(p => p.EntityId == id)
                   .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                            .ThenInclude(m => m.MembershipType)
                                .ThenInclude(x => x.PeriodNavigation)
                   .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                            .ThenInclude(x => x.Membershiphistories)
                    .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                            .ThenInclude(x => x.Billingfees)
                                .ThenInclude(x => x.MembershipFee)
                    .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                            .ThenInclude(x => x.MembershipType)
                    .Include(m => m.Membershipconnections)
                        .ThenInclude(m => m.Membership)
                            .ThenInclude(x => x.MembershipType)
                                .ThenInclude(x => x.CategoryNavigation)
                    .Include(m => m.Membershipconnections)
                        .ThenInclude(m => m.Membership)
                            .ThenInclude(m => m.BillableEntity)
                   .Include(m => m.Paymentprofiles)
                   .Include(c => c.Credittransactions).DefaultIfEmpty()
                   .FirstOrDefaultAsync();

        }

        public async Task<Entity> GetMembershipHistoryByEntityId(int id)
        {

            return await membermaxContext.Entities
                   .Where(p => p.EntityId == id)
                   .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                            .ThenInclude(x => x.Membershiphistories).DefaultIfEmpty()
                    .Include(m => m.Memberships)
                         .ThenInclude(x => x.Billingfees).DefaultIfEmpty()
                    .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                            .ThenInclude(x => x.MembershipType)
                                .ThenInclude(x => x.PeriodNavigation)
                    .Include(m => m.Membershipconnections)
                        .ThenInclude(x => x.Membership)
                             .ThenInclude(x => x.MembershipType)
                                 .ThenInclude(x => x.CategoryNavigation)
                    .AsNoTracking()
                   .FirstOrDefaultAsync();

        }
        public async Task<IEnumerable<Entity>> GetEntitiesByIdsAsync(int[] entityIds)
        {
            return await membermaxContext.Entities
               .Where(x => entityIds.Contains(x.EntityId))
               .Include(i => i.People)
               .Include(e => e.Companies)
               .Include(m => m.Membershipconnections)
                    .ThenInclude(x => x.Membership)
               .ToListAsync();
        }

        public async Task<Entity> GetEntityByUserNameAsync(string userName)
        {
            return await membermaxContext.Entities
               .Where(x => x.WebLoginName == userName)
               .Include(i => i.People)
                .ThenInclude(e => e.Emails)
               .Include(e => e.Companies)
               .FirstOrDefaultAsync();
        }

        public async Task<Entity> GetEntityByWebLoginNameAsync(string userName)
        {
            var entityDetailsByWebLoginName = await membermaxContext.Entities.FirstOrDefaultAsync(s => s.WebLoginName == userName);
            if(entityDetailsByWebLoginName!=null)
            {
                return entityDetailsByWebLoginName;
            }
            return null;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }


    }
}
