using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Data.Interfaces
{
    public interface IReceiptHeaderRepository: IRepository<Receiptheader>
    {
        Task<Receiptheader> GetReceiptByIdAsync(int receiptid);
        Task<IEnumerable<Receiptheader>> GetAllReceiptsAsync();
        Task<IEnumerable<Receiptheader>> GetReceiptsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Receiptheader>> GetReceiptsByOrganizationIdAsync(int OrganizationId);
        Task<IEnumerable<Receiptheader>> GetReceiptsByStaffIdAsync(int staffId);
        Task<Receiptheader> GetReceiptDetailById(int id);
        Task<List<Receiptheader>> GetReceiptDetailsById(int[] receiptids);
        Task<Receiptheader> GetReceiptItemDetailById(int id);
        Task<IEnumerable<MonthDayGroupModel>> GetDailySalesByMonth();
        Task<IEnumerable<Receiptheader>> GetReceiptsByPromoCodeIdAsync(int promoCodeId);
    }
}
