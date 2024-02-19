
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Max.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAutoBillingNotificationService _autoBillingNotificationService;
        private readonly IAutoBillingSettingsService _autoBillingSettingsService;
        private readonly IEmailService _emailService;
        private readonly ITenantService _tenantService;
        private readonly ILogger<NotificationService> _logger;
        public NotificationService(IOptions<AppSettings> appSettings, 
                                    IUnitOfWork unitOfWork, 
                                    IDocumentService documentService, 
                                    IAutoBillingNotificationService autoBillingNotificationService,
                                    IAutoBillingSettingsService autoBillingSettingsService,
                                    IEmailService emailService,
                                    ITenantService tenantService,
                                    ILogger<NotificationService> logger)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _autoBillingNotificationService = autoBillingNotificationService;
            _autoBillingSettingsService = autoBillingSettingsService;
            _emailService = emailService;
            _tenantService = tenantService;
            _logger = logger;
        }

        public MessageResource SendAutoBillingSMSNotification(string message, string smsNumber)
        {
            string accountSid = "ACa660151372d853b2b51910cc1bc81295";
            string authToken = "1caf61a03ab8cbc7926fbe841fcc95cb";

            TwilioClient.Init(accountSid, authToken);

            var response = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber("+14252303178"),
                to: new Twilio.Types.PhoneNumber(smsNumber)
            );
            return response;
        }
        public MessageResource SendMultiFactorSMSNotification(string organizationName,string message, string smsNumber)
        {
            _logger.LogInformation("Entering SendMultiFactorSMSNotification  SMS Number -> " + smsNumber);
            var tenantConfig =  _tenantService.GetTenantByOrganizationName(organizationName).GetAwaiter().GetResult();
            string accountSid = tenantConfig.TwilioAccountSid;
            string authToken = tenantConfig.TwilioAuthToken;
            string fromPhoneNumber = tenantConfig.TwilioSenderPhoneNumber;
            try {
                TwilioClient.Init(accountSid, authToken);

                var response = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
                    to: new Twilio.Types.PhoneNumber("+" + smsNumber)
                );
                _logger.LogInformation("Exiting SendMultiFactorSMSNotification  SMS Number -> " + smsNumber);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendMultiFactorSMSNotification Error:{ex.Message} {ex.StackTrace}");
            }
            return null;
        }
        public async Task<bool> SendAutoBillingNotification(AutoBillingEmailNotificationModel model)
        {
            var settings = await _autoBillingSettingsService.GetAutoBillingConfiguration();

            model.EmailAddresses = settings.NotificationEmails.Split(",");
            model.ProcessDate = settings.EffectiveDate.ToString("MM/dd/yyyy");
            model.ThroughDate = settings.ThroughDate.ToString("MM/dd/yyyy");
            model.BillingType = "Membership"; //TODO:AKS should come from configuration

            try
            {
                await _emailService.SendAutoBillingNotification(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Send SMS Notfication
            if(settings.NotificationSMSNumbers.Length > 0)
            {
                var message = $"AutoBilling Service: Processed: ${model.TotalDue.ToString()} Approved: ${model.Approved.ToString()}";
                var smsNumbers = settings.NotificationSMSNumbers.Split(",");
                for (int i = 0; i < smsNumbers.Length; i++)
                {
                    SendAutoBillingSMSNotification(message, smsNumbers[i]);
                }
            }
            return true;
        }
    }
}
