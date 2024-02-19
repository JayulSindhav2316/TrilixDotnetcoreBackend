using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Services
{
    public class ReceiptDetailService : IReceiptDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReceiptDetailService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<Receiptdetail> CreateReceipt(ReceiptDetailModel receiptDetailModel) 
        {
            Receiptdetail receiptDetail = new Receiptdetail();

            receiptDetail.ReceiptHeaderId = receiptDetailModel.ReceiptHeaderId;
            receiptDetail.Quantity = receiptDetailModel.Quantity;
            receiptDetail.Rate = receiptDetailModel.Rate;
            receiptDetail.Amount = receiptDetailModel.Amount;
            receiptDetail.Status = receiptDetailModel.Status;
            receiptDetail.Description = receiptDetailModel.Description;
            receiptDetail.InvoiceDetailId = receiptDetailModel.InvoiceDetailId;
            receiptDetail.Tax = receiptDetailModel.Tax;
            receiptDetail.PastDueInvoiceDetailRef = receiptDetailModel.PastDueInvoiceDetailRef;
            receiptDetail.ItemType = receiptDetailModel.ItemType;

            await _unitOfWork.ReceiptDetails.AddAsync(receiptDetail);
            await _unitOfWork.CommitAsync();
            return receiptDetail;
        }
        public async Task<bool> UpdateReceipt(ReceiptDetailModel receiptDetailModel) 
        {
            Receiptdetail receiptDetail = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByIdAsync(receiptDetailModel.ReceiptDetailId);

            if (receiptDetail != null)
            {
                receiptDetail.ReceiptHeaderId = receiptDetailModel.ReceiptHeaderId;
                receiptDetail.Quantity = receiptDetailModel.Quantity;
                receiptDetail.Rate = receiptDetailModel.Rate;
                receiptDetail.Amount = receiptDetailModel.Amount;
                receiptDetail.Status = receiptDetailModel.Status;
                receiptDetail.Description = receiptDetailModel.Description;
                receiptDetail.InvoiceDetailId = receiptDetailModel.InvoiceDetailId;
                receiptDetail.Tax = receiptDetailModel.Tax;
                receiptDetail.PastDueInvoiceDetailRef = receiptDetailModel.PastDueInvoiceDetailRef;
                receiptDetail.ItemType = receiptDetailModel.ItemType;

                _unitOfWork.ReceiptDetails.Update(receiptDetail);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }
        public async Task<Receiptdetail> GetReceiptDetailsById(int receiptDetailid) 
        {
            return await _unitOfWork.ReceiptDetails.GetReceiptDetailsByIdAsync(receiptDetailid);
        }
        public async Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByReceiptId(int receiptId) 
        {
            return await _unitOfWork.ReceiptDetails.GetReceiptDetailsByReceiptIdAsync(receiptId);
        }
        public async Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceId(int invoiceId)
        {
            return await _unitOfWork.ReceiptDetails.GetReceiptDetailsByInvoiceIdAsync(invoiceId);
        }
        public async Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceDetailId(int invoiceDetailId)
        {
            return await _unitOfWork.ReceiptDetails.GetReceiptDetailsByInvoiceDetailIdAsync(invoiceDetailId);
        }
        public async Task<bool> RefundPayment(RefundRequestModel model)
        {
            Receiptdetail receiptDetail = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByIdAsync(model.ReceiptDetailId);

            return true;
        }
    }
}
