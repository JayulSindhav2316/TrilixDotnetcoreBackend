using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;

namespace Max.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoices();
        Task<InvoiceModel> GetInvoiceById(int id);
        Task<InvoiceModel> CreateInvoice(InvoiceModel invoiceModel);
        Task<InvoiceModel> CreateItemInvoice(GeneralInvoiceModel model);
        Task<Invoice> UpdateItemInvoice(GeneralInvoiceModel model);
        Task<InvoiceModel> CreateNewMembershipInvoice(MembershipSessionModel model);
        Task<InvoiceModel> CreateMembershipBillingInvoice(int membershipId, string invoiceType, DateTime nextBilldate, int cycleId = 0);
        Task<IEnumerable<InvoicePaymentModel>> GetAllInvoicesByEntityId(int id, string sortOrder, string paymentStatus);
        Task<IEnumerable<InvoicePaymentModel>> GetInvoicesBySearchCondition(int Id, string serachBy, string itemDescription, DateTime start, DateTime end);
        Task<InvoiceModel> GetInvoiceDetailsByInvoiceId(int id);
        Task<Invoice> UpdateInvoice(InvoiceModel invoiceModel);
        Task<bool> DeleteInvoice(int invoiceId);
        Task<bool> DeletePendingInvoices(string invoiceType);
        Task<IEnumerable<MembershipDuesModel>> GetMembershipInvoiceDues();
        Task<IEnumerable<ReceivablesReportMembershipDueModel>> GetAllOutstandingReceivables();
        decimal GetBalanceByEntityId(int id);
        Task<IEnumerable<InvoicePaymentModel>> GetInvoicePaymentsByEntityId(int id);
        Task<List<InvoicePaymentModel>> GetInvoicePaymentsByInvoiceId(int invoiceId);
        Task<IEnumerable<InvoicePaymentModel>> GetInvoicesByMultipleInvoiceIds(int[] invoiceIds);
        Task<IEnumerable<InvoicePaymentModel>> GetInvoicesWithBalanceByEntityId(int entityId);
        Task<bool> SendHtmlInvoice(EmailMessageModel model);
        Task<Invoicedetail> GetInvoiceDetail(int id);
    }
}
