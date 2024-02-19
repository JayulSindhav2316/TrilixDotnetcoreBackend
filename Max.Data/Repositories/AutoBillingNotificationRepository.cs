using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class AutoBillingNotificationRepository : Repository<Autobillingnotification>, IAutoBillingNotificationRepository
    {
        public AutoBillingNotificationRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Autobillingnotification> GetAutoBillingNotificationByIdAsync(int autoBillingNotificationId)
        {
            return await membermaxContext.Autobillingnotifications.Where(a => a.AutoBillingNotificationId == autoBillingNotificationId).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAllAutoBillingNotificationsAsync()
        {
            return await membermaxContext.Autobillingnotifications.ToListAsync();
        }
        public async Task<Autobillingnotification> GetAutoBillingNotificationByABPDIdAsync(int abpdId)
        {
            return await membermaxContext.Autobillingnotifications.Where(a => a.AbpdId == abpdId).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByBillingTypeAsync(string billingType)
        {
            return await membermaxContext.Autobillingnotifications.Where(a => a.BillingType == billingType).ToListAsync();
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByInvoiceTypeAsync(string invoiceType)
        {
            return await membermaxContext.Autobillingnotifications.Where(a => a.InvoiceType == invoiceType).ToListAsync();
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationTypeAsync(string notificationType)
        {
            return await membermaxContext.Autobillingnotifications.Where(a => a.NotificationType == notificationType).ToListAsync();
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationDateAsync(DateTime notificationDate)
        {
            return await membermaxContext.Autobillingnotifications.Where(a => a.NotificationSentDate == notificationDate).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
