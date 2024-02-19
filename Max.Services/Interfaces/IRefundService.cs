using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IRefundService
    {
        Task<Refunddetail> CreateRefund(RefundRequestModel model);
        Task<RefundResponseModel> ProcessRefund(RefundRequestModel model);
    }
}
