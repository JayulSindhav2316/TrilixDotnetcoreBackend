using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;

namespace Max.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<bool> UpdateTransactionStatus(PaymentTransactionModel model);
    }
}
