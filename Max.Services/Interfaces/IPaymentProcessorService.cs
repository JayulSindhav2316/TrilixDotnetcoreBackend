using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;
using AuthorizeNetCore.Models;

namespace Max.Services.Interfaces
{
    public interface IPaymentProcessorService
    {
        Task<Paymentprocessor> GetPaymentProcessorByOrganizationId(int organizationId);
        Task<MerchantConfigModel> GetPaymentProcessorInfoByOrganizationId(int organizationId);
        
    }
}
