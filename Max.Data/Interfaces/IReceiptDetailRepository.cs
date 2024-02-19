using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IReceiptDetailRepository : IRepository<Receiptdetail>
    {
        Task<Receiptdetail> GetReceiptDetailsByIdAsync(int receiptDetailid);
        Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByReceiptIdAsync(int receiptId);
        Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceIdAsync(int invoiceId);
        Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceDetailIdAsync(int invoiceDetailId);
    }
}
