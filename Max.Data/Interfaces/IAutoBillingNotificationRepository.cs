using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IAutoBillingNotificationRepository : IRepository<Autobillingnotification>
    {
        Task<Autobillingnotification> GetAutoBillingNotificationByIdAsync(int autoBillingNotificationId);
        Task<IEnumerable<Autobillingnotification>> GetAllAutoBillingNotificationsAsync();
        Task<Autobillingnotification> GetAutoBillingNotificationByABPDIdAsync(int abpdId);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByBillingTypeAsync(string billingType);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByInvoiceTypeAsync(string invoiceType);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationTypeAsync(string notificationType);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationDateAsync(DateTime notificationDate);
    }
}
