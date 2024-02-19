using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IEmailService
    {
        void Send();

        Task<bool> SendHtmlInvoice (BatchEmailNotificationModel model);
        Task<bool> SendAutoBillingNotification(AutoBillingEmailNotificationModel model);
        Task<bool> SendMultiFactorNotification(string organizationName, string emailAddress, string code);
        Task<bool> SendHtmlReceipt(EmailMessageModel model);
        Task<bool> SendBatchInvoiceNotification(BatchEmailNotificationModel model);
        Task<bool> SendPasswordResetNotification(ResetPasswordModel model);
        string GetInvoiceNotificationEmailBody(BatchEmailNotificationModel model);
        Task<bool> SendAccountVerificationEmailBody(MemberAccountEmailModel model);
    }

}
