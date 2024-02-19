using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;
using AuthorizeNetCore.RefundModels;


namespace Max.Services.Interfaces
{
    public interface IAuthNetService
    {
        Task<AuthNetPaymentResponseModel> ProcessAcceptPayment(AuthNetSecureDataModel model);
        Task<AuthNetPaymentProfileResponseModel> ProcessPaymentProfile(AuthNetSecureDataModel model);
        Task<PaymentProfileModel> GetPaymentProfile(AuthNetPaymentProfileRequestModel model);
        Task<int> ProcessCreditCardRefund(CreditCardRefundModel model);
        Task<AuthNetVoidModel> ProcessCreditCardVoid(CreditCardVoidModel model);
        Task<AuthNetPaymentResponseModel> ChargePaymentProfile(AuthNetChargePaymentProfileRequestModel model);
        Task<PaymentProfileModel> DeletePaymentProfile(AuthNetPaymentProfileRequestModel model);
        
    }
}
