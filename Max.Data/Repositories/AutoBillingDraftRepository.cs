using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core.Models;
using Max.Core;

namespace Max.Data.Repositories
{
    public class AutoBillingDraftRepository : Repository<Autobillingdraft>, IAutoBillingDraftRepository
    {
        public AutoBillingDraftRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Autobillingdraft> GetAutobillingDraftByIdAsync(int autoBillingDraftId)
        {
            return await membermaxContext.Autobillingdrafts.SingleOrDefaultAsync(a => a.AutoBillingDraftId == autoBillingDraftId);
        }
        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsAsync()
        {
            return await membermaxContext.Autobillingdrafts.ToListAsync();
        }
        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByPersonIdAsync(int personId)
        {
            //return await membermaxContext.Autobillingdrafts.Where(a => a.PersonId == personId).ToListAsync();
            return await membermaxContext.Autobillingdrafts.ToListAsync();
        }
       
        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByProcessStatusAsync(int processStatus)
        {
            return await membermaxContext.Autobillingdrafts.Where(a => a.IsProcessed == processStatus).ToListAsync();
        }
        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByBillingDocumentIdAsync(int billingDocumentId)
        {
            return await membermaxContext.Autobillingdrafts
                .Include(p => p.Entity)
                .ThenInclude(p => p.People)
                .Include(p => p.Entity)
                .ThenInclude(p => p.Companies)
                .Include(p => p.Invoice)
                    .ThenInclude(p => p.Entity)
                        .ThenInclude(p=>p.People)
                .Include(p => p.Invoice)
                    .ThenInclude(p => p.Entity)
                        .ThenInclude(p => p.Companies)
                //.Include(m => m.Membership)
                //    .ThenInclude(x => x.MembershipType)
                //        .ThenInclude(t => t.CategoryNavigation)
                .Include(p => p.BillingDocument)
                .Where(p => p.BillingDocumentId == billingDocumentId).ToListAsync();
        }

        public async Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsSummaryByCategoryIdAsync(int billingDocumentId)
        {
           return await membermaxContext.Autobillingdrafts
                //.Include(p => p.Person)
                //.Include(b => b.BillingDocument)
                //.Include(c => c.Person.Memberships)
                //    .ThenInclude(x => x.MembershipType)
                //        .ThenInclude(t => t.CategoryNavigation)
                .Where(a => a.BillingDocumentId == billingDocumentId)
                .ToListAsync();
              
        }

        public async Task<IEnumerable<Autobillingdraft>> GetLastAutoBillingDraftPaymentsAsync(int billingDocumentId)
        {
            return await membermaxContext.Autobillingdrafts
                .Include( x => x.Autobillingpayments)
                    .ThenInclude(x => x.PaymentTransaction)
                .Where(a => a.BillingDocumentId == billingDocumentId)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
