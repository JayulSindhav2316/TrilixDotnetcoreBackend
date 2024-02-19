using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Data.Repositories
{
    public class PaymentProfileRepository : Repository<Paymentprofile>, IPaymentProfileRepository
    {
        public PaymentProfileRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Paymentprofile>> GetAllPaymentProfilesAsync()
        {
            return await membermaxContext.Paymentprofiles
                .ToListAsync();
        }

        public async Task<Paymentprofile> GetPaymentProfileByIdAsync(int id)
        {
            return await membermaxContext.Paymentprofiles
                .SingleOrDefaultAsync(m => m.PaymentProfileId == id);
        }
        public async Task<Paymentprofile> GetPreferredPaymentProfileByEntityIdAsync(int id)
        {
            return await membermaxContext.Paymentprofiles
                    .Where(m => m.EntityId == id && m.PreferredPaymentMethod == (int)Status.Active && m.Status!=0).FirstOrDefaultAsync(); ;
        }

        public async Task<Paymentprofile> GetAutoBillingPaymentProfileByEntityIdAsync(int id)
        {
            return await membermaxContext.Paymentprofiles
                    .Where(m => m.EntityId == id && m.UseForAutobilling == (int)Status.Active).FirstOrDefaultAsync(); ;
        }

        public async Task<IEnumerable<Paymentprofile>> GetPaymentProfileByEntityIdAsync(int id)
        {
            return await membermaxContext.Paymentprofiles
                    .Where(m => m.EntityId == id).ToListAsync(); ;
        }

        public async Task<IEnumerable<Paymentprofile>> GetActivePaymentProfileByEntityIdAsync(int id)
        {
            return await membermaxContext.Paymentprofiles
                    .Where(m => m.EntityId == id && m.Status == (int)Status.Active).ToListAsync(); ;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}