using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IAutoBillingSettingsService
    {
        Task<Autobillingsetting> CreateAutoBillingSetting(AutoBillingSettingModel autoBillingSettingModel);
        Task<bool> UpdateAutoBillingSetting(AutoBillingSettingModel autoBillingSettingModel);
        Task<Autobillingsetting> GetAutoBillingSetting();
        Task<AutoBillingConfiguration> GetAutoBillingConfiguration();
        Task<bool> UpdateAutoBillingConfiguration(AutoBillingConfiguration model);
    }
}
 