using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
namespace Max.Services
{
    public class InvoiceDetailService : IInvoiceDetailService
    {

        private readonly IUnitOfWork _unitOfWork;
        public InvoiceDetailService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetails()
        {
            return await _unitOfWork.InvoiceDetails
                .GetAllInvoiceDetailsAsync();
        }

        public async Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsByInvoiceId(int invoiceId)
        {
            return await _unitOfWork.InvoiceDetails
                .GetAllInvoiceDetailsByInvoiceIdAsync(invoiceId);
        }

        public async Task<Invoicedetail> GetInvoiceDetailById(int id)
        {
            return await _unitOfWork.InvoiceDetails
                .GetInvoiceDetailByIdAsync(id);
        }

       
        public async Task<Invoicedetail> CreateInvoiceDetail(InvoiceDetailModel model)
        {
            Invoicedetail invoiceDetail = new Invoicedetail();
            var isValid = ValidInvoiceDetail(model);
            if (isValid)
            {
                //Map Model Data
                invoiceDetail.InvoiceId = model.InvoiceId;
                invoiceDetail.Description = model.Description;
                invoiceDetail.Amount = model.Amount;
                invoiceDetail.Quantity = model.Quantity;
                invoiceDetail.Taxable = model.Taxable;
                invoiceDetail.GlAccount = model.GlAccount;
                invoiceDetail.Status = model.Status;

                await _unitOfWork.InvoiceDetails.AddAsync(invoiceDetail);
                await _unitOfWork.CommitAsync();
            }
            return invoiceDetail;
        }

        private bool ValidInvoiceDetail(InvoiceDetailModel model)
        {
            //Validate  Name
            if (model.InvoiceId == 0)
            {
                throw new InvalidOperationException($"Invoice Id not defined.");
            }

            if (model.GlAccount.IsNullOrEmpty())
            {
                throw new NullReferenceException($"GlAccount is requirede.");
            }

            return true;
        }
    }
}
