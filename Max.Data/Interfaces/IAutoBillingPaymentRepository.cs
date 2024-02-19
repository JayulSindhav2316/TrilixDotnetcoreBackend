using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IAutoBillingPaymentRepository : IRepository<Autobillingpayment>
    {
        Task<Autobillingpayment> GetAutobillingPaymentByIdAsync(int autoBillingPaymentId);
        Task<IEnumerable<Autobillingpayment>> GetAutobillingPaymentsAsync();
        Task<Autobillingpayment> GetAutobillingPaymentByPaymentTransactionIdAsync(int paymentTransactionId);
    }
}
