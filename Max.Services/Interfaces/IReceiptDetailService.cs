using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IReceiptDetailService
    {
        Task<Receiptdetail> CreateReceipt(ReceiptDetailModel receiptDetailModel);
        Task<bool> UpdateReceipt(ReceiptDetailModel receiptDetailModel);
        Task<Receiptdetail> GetReceiptDetailsById(int receiptDetailid);
        Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByReceiptId(int receiptId);
        Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceId(int invoiceId);
        Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceDetailId(int invoiceDetailId);
        Task<bool> RefundPayment(RefundRequestModel model);
    }
}
