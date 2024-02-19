using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Max.Services.Interfaces
{
    public interface INotificationService
    {
        MessageResource SendAutoBillingSMSNotification(string message, string smsNumber);
        Task<bool> SendAutoBillingNotification(AutoBillingEmailNotificationModel model);
        MessageResource SendMultiFactorSMSNotification(string organizationName,string message, string smsNumber);
    }

}
