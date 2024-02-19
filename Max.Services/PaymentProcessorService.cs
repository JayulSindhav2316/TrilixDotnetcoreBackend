using System.Threading.Tasks;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using AuthorizeNetCore.Models;
using Max.Core;

namespace Max.Services
{
    public class PaymentProcessorService : IPaymentProcessorService
    {

        private readonly IUnitOfWork _unitOfWork;
        public PaymentProcessorService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Paymentprocessor> GetPaymentProcessorByOrganizationId(int id)
        {
            return await _unitOfWork.PaymentProcessors
                .GetPaymentProcessorByOrganizationIdAsync(id);
        }

        public async Task<MerchantConfigModel> GetPaymentProcessorInfoByOrganizationId(int id)
        {
            var config = new MerchantConfigModel();
            var processor =  await _unitOfWork.PaymentProcessors.GetPaymentProcessorByOrganizationIdAsync(id);
            if(processor != null)
            {
                if (processor.TransactionMode == (int)PaymentTransactionMode.Live)
                {
                    config.AccepJSURL = processor.LiveAcceptJsurl;
                }
                else
                {
                    config.AccepJSURL = processor.TestAcceptJsurl;
                }
                config.LoginId = processor.LoginId;
                config.TransactionKey = processor.ApiKey;
            }
            return config;
        }

    }
}
