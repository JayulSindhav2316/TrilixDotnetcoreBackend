using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IPaymentTransactionService
    {
        Task<Paymenttransaction> GetPaymentTransactionById(int paymentTransactionId);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByDate(DateTime transactionDate);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByEntityIdAsync(int id);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByReceiptId(int receiptId);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByTransactionType(int transactionType);
        Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByPaymentType(string paymentType);
        Task<IList<CreditCardReportModel>> GetCreditCardReport(string cardType, string searchType, DateTime fromDate, DateTime toDate);
        Task<IList<DepositReportModel>> GetDepositReport(string paymentType, string searchType, DateTime fromDate, DateTime toDate, int summary, string portal);
        Task<PaymentTransactionModel> CreatePaymentTransaction(PaymentTransactionModel model);
        Task<bool> ProcessCreditPaymentTransaction(int cartId);
        Task<bool> UpdatePaymentTransaction(PaymentTransactionModel model);
    }
}
