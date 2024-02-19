using Max.Core;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class BillingCycleNotificationRepository : Repository<BillingCycleNotification>, IBillingCycleNotificationRepository
    {
        public BillingCycleNotificationRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<BillingCycleNotification> GetNotificationByBillingCycleIdAsync(int id)
        {
            var billingJobNotification = await membermaxContext.BillingCycleNotifications
                               .Where(x => x.BillingCycleId == id)
                               .SingleOrDefaultAsync();

            return billingJobNotification;
        }

        public async Task<IEnumerable<BillingCycleNotification>> GetAllUnReadBillingNotificationAsync()
        {
            var billingJobNotifications = await membermaxContext.BillingCycleNotifications
                               .Where(x => x.IsRead == (int)Status.InActive).ToListAsync();

            return billingJobNotifications;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
