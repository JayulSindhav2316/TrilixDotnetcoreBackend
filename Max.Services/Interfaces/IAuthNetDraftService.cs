using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;

namespace Max.Services.Interfaces
{
    public interface IAuthNetDraftService
    {
        Task<AuthNetPaymentResponseModel> ChargePaymentProfile(AuthNetChargePaymentProfileRequestModel model);

    }
}
