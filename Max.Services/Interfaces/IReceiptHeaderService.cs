using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IReceiptHeaderService
    {
        Task<Receiptheader> CreateReceipt(ReceiptHeaderModel receiptHeaderModel);
        Task<bool> UpdateReceipt(ReceiptHeaderModel receiptHeaderModel);
        Task<Receiptheader> GetReceiptById(int receiptId);
        Task<IEnumerable<Receiptheader>> GetAllReceipts();
        Task<IEnumerable<Receiptheader>> GetReceiptsByDateRange(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Receiptheader>> GetReceiptsByOrganizationId(int organizationId);
        Task<IEnumerable<Receiptheader>> GetReceiptsByStaffId(int staffId);
        Task<Receiptheader> GetReceiptDetailById(int receiptId);
        Task<ReceiptModel> GetReceiptDetailByCartId(int cartId);
        Task<ReceiptModel> GetReceiptDetailByReceiptId(int receiptId);
        Task<Receiptheader> CreateDratfReceipt(InvoiceModel invoice, Autobillingdraft draft);
        Task<LineChartModel> GetDailySalesByMonth();
        Task<ReceiptModel> GetReceiptModel(int receiptId,int? cartId);
    }
}
