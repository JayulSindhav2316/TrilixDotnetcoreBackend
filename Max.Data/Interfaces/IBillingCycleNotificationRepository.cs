using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IBillingCycleNotificationRepository : IRepository<BillingCycleNotification>
    {
        Task<BillingCycleNotification> GetNotificationByBillingCycleIdAsync(int id);
        Task<IEnumerable<BillingCycleNotification>> GetAllUnReadBillingNotificationAsync();
    }
}
