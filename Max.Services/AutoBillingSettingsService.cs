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
    public class AutoBillingSettingsService : IAutoBillingSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AutoBillingSettingsService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<Autobillingsetting> CreateAutoBillingSetting(AutoBillingSettingModel autoBillingSettingModel)
        {
            Autobillingsetting autobillingsetting = new Autobillingsetting();

            autobillingsetting.EnableAutomatedBillingForMembership = autoBillingSettingModel.EnableAutomatedBillingForMembership;
            //autoBillingSettingModel.AutomatedCoordinatorForMembership = autoBillingSettingModel.AutomatedCoordinatorForMembership;
            autobillingsetting.IsPauseOrEnableForMembership = autoBillingSettingModel.IsPauseOrEnableForMembership;
            autobillingsetting.EmailForNotification = autoBillingSettingModel.EmailForNotification;
            autobillingsetting.SmsforNotification = autoBillingSettingModel.SmsforNotification;

            await _unitOfWork.AutoBillingSettings.AddAsync(autobillingsetting);
            await _unitOfWork.CommitAsync();
            return autobillingsetting;
        }
        public async Task<bool> UpdateAutoBillingSetting(AutoBillingSettingModel autoBillingSettingModel)
        {
            Autobillingsetting autobillingsetting = await _unitOfWork.AutoBillingSettings.SingleOrDefaultAsync(a => a.AutoBillingsettingsId > 0);

            if (autobillingsetting != null)
            {
                autoBillingSettingModel.EnableAutomatedBillingForMembership = autoBillingSettingModel.EnableAutomatedBillingForMembership;
                autoBillingSettingModel.AutomatedCoordinatorForMembership = autoBillingSettingModel.AutomatedCoordinatorForMembership;
                autoBillingSettingModel.IsPauseOrEnableForMembership = autoBillingSettingModel.IsPauseOrEnableForMembership;

                _unitOfWork.AutoBillingSettings.Update(autobillingsetting);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<Autobillingsetting> GetAutoBillingSetting()
        {
            return await _unitOfWork.AutoBillingSettings.SingleOrDefaultAsync(a => a.AutoBillingsettingsId > 0);
        }
        public async Task<AutoBillingConfiguration> GetAutoBillingConfiguration()
        {
            var settings = await GetAutoBillingSetting();
            var dates = await _unitOfWork.AutoBillingProcessingDates.SingleOrDefaultAsync(a => a.AutoBillingProcessingDatesId > 0);

            AutoBillingConfiguration configuration = new AutoBillingConfiguration();
            configuration.AutoBillingSettingId = settings.AutoBillingsettingsId;
            configuration.AutoBillingProcessingDateId = dates.AutoBillingProcessingDatesId;
            configuration.AutoBillingEnabled = settings.EnableAutomatedBillingForMembership == 1 ? true : false;
            configuration.NotificationEmails = settings.EmailForNotification;
            configuration.NotificationSMSNumbers = settings.SmsforNotification;
            configuration.AutoBillingStatus = settings.IsPauseOrEnableForMembership == 1 ? true : false;
            configuration.ThroughDate = dates.ThroughDate;
            configuration.EffectiveDate = dates.EffectiveDate;
            
            return configuration;
        }

        public async Task<bool> UpdateAutoBillingConfiguration(AutoBillingConfiguration model)
        {
            var settings = await _unitOfWork.AutoBillingSettings.GetByIdAsync(model.AutoBillingSettingId);

            var dates = await _unitOfWork.AutoBillingProcessingDates.GetAutoBillingProcessingDatesByABPDIdAsync(model.AutoBillingProcessingDateId);

            settings.IsPauseOrEnableForMembership = model.AutoBillingStatus? 1: 0;
            settings.EnableAutomatedBillingForMembership = model.AutoBillingEnabled ? 1 : 0;
            settings.EmailForNotification = model.NotificationEmails;
            settings.SmsforNotification = model.NotificationSMSNumbers;
            dates.ThroughDate = model.ThroughDate;
            dates.EffectiveDate = model.EffectiveDate;

            try
            {
                _unitOfWork.AutoBillingSettings.Update(settings);
                _unitOfWork.AutoBillingProcessingDates.Update(dates);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
