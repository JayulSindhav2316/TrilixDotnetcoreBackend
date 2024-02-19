using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IAutoBillingNotificationService
    {
        Task<Autobillingnotification> CreateAutoBillingNotification(AutoBillingNotificationModel autoBillingNotificationModel);
        Task<bool> UpdateAutoBillingNotification(AutoBillingNotificationModel autoBillingNotificationModel);
        Task<Autobillingnotification> GetAutoBillingNotificationById(int autoBillingNotificationId);
        Task<IEnumerable<Autobillingnotification>> GetAllAutoBillingNotifications();
        Task<Autobillingnotification> GetAutoBillingNotificationByABPDId(int abpdId);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByBillingType(string billingType);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByInvoiceType(string invoiceType);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationType(string notificationType);
        Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationDate(DateTime notificationDate);
    }
}
