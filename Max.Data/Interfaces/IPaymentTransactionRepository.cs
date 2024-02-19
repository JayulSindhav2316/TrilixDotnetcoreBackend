using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;
using System;

namespace Max.Data.Interfaces
{
    public interface IPaymentTransactionRepository : IRepository<Paymenttransaction>
    {
        Task<Paymenttransaction> GetPaymentTransactionByIdAsync(int paymentTransactionId);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByDateAsync(DateTime transactionDate);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByEntityIdAsync(int id);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByReceiptIdAsync(int receiptId);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByTransactionTypeAsync(int transactionType);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByPaymentTypeAsync(string paymentType);
        Task<IEnumerable<Paymenttransaction>> GetCreditCardPaymentsByDateRangeAsync(string cardType, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Paymenttransaction>> GetDepositsByDateRangeAsync(string paymentType, DateTime fromDate, DateTime toDate, string portal);
    }
}
