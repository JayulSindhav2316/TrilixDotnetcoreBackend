using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Services
{
    public class AutoBillingNotificationService : IAutoBillingNotificationService
    { 
        private readonly IUnitOfWork _unitOfWork;
        public AutoBillingNotificationService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<Autobillingnotification> CreateAutoBillingNotification(AutoBillingNotificationModel autoBillingNotificationModel)
        {
            Autobillingnotification autobillingnotification = new Autobillingnotification();

            autobillingnotification.AbpdId = autoBillingNotificationModel.AbpdId;
            autobillingnotification.BillingType = autoBillingNotificationModel.BillingType;
            autobillingnotification.InvoiceType = autoBillingNotificationModel.InvoiceType;
            autobillingnotification.NotificationType = autoBillingNotificationModel.NotificationType;
            autobillingnotification.NotificationSentDate = autoBillingNotificationModel.NotificationSentDate;


            await _unitOfWork.AutoBillingNotifications.AddAsync(autobillingnotification);
            await _unitOfWork.CommitAsync();
            return autobillingnotification;
        }
        public async Task<bool> UpdateAutoBillingNotification(AutoBillingNotificationModel autoBillingNotificationModel)
        {
            Autobillingnotification autobillingnotification = await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationByIdAsync(autoBillingNotificationModel.AutoBillingNotificationId);

            if (autobillingnotification != null)
            {
                autobillingnotification.AbpdId = autoBillingNotificationModel.AbpdId;
                autobillingnotification.BillingType = autoBillingNotificationModel.BillingType;
                autobillingnotification.InvoiceType = autoBillingNotificationModel.InvoiceType;
                autobillingnotification.NotificationType = autoBillingNotificationModel.NotificationType;
                autobillingnotification.NotificationSentDate = autoBillingNotificationModel.NotificationSentDate;

                _unitOfWork.AutoBillingNotifications.Update(autobillingnotification);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<Autobillingnotification> GetAutoBillingNotificationById(int autoBillingNotificationId)
        {
            return await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationByIdAsync(autoBillingNotificationId);
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAllAutoBillingNotifications() 
        {
            return await _unitOfWork.AutoBillingNotifications.GetAllAutoBillingNotificationsAsync();
        }
        public async Task<Autobillingnotification> GetAutoBillingNotificationByABPDId(int abpdId) 
        {
            return await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationByABPDIdAsync(abpdId);
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByBillingType(string billingType)
        {
            return await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationsByBillingTypeAsync(billingType);
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByInvoiceType(string invoiceType)
        {
            return await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationsByInvoiceTypeAsync(invoiceType);
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationType(string notificationType)
        {
            return await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationsByNotificationTypeAsync(notificationType);
        }
        public async Task<IEnumerable<Autobillingnotification>> GetAutoBillingNotificationsByNotificationDate(DateTime notificationDate)
        {
            return await _unitOfWork.AutoBillingNotifications.GetAutoBillingNotificationsByNotificationDateAsync(notificationDate);
        }
    }
}
 