using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Core;
using static Max.Core.Constants;
using AutoMapper;
using System.Linq;
using Max.Core.Helpers;
using iTextSharp.text.rtf.field;

namespace Max.Services
{
    public class ReceiptHeaderService : IReceiptHeaderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReceiptHeaderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<Receiptheader> CreateReceipt(ReceiptHeaderModel receiptHeaderModel)
        {
            Receiptheader receiptheader = new Receiptheader();
            receiptheader.Date = DateTime.UtcNow;
            receiptheader.StaffId = receiptHeaderModel.StaffId;
            receiptheader.PaymentMode = receiptHeaderModel.PaymentMode;
            receiptheader.CheckNo = receiptHeaderModel.CheckNo;
            receiptheader.Status = receiptHeaderModel.Status;
            receiptheader.OrganizationId = receiptHeaderModel.OrganizationId;
            receiptHeaderModel.PaymentTransactionId = receiptHeaderModel.PaymentTransactionId;

            await _unitOfWork.ReceiptHeaders.AddAsync(receiptheader);
            await _unitOfWork.CommitAsync();
            return receiptheader;
        }

        public async Task<Receiptheader> CreateDratfReceipt(InvoiceModel invoice, Autobillingdraft draft)
        {
            //Add Receipt to Cart
            var receipt = new Receiptheader();
            int receiptStatus = (int)ReceiptStatus.Created;
            receipt.Date = DateTime.Now;
            receipt.CheckNo = String.Empty;
            receipt.PaymentMode = PaymentType.CREDITCARD;
            receipt.StaffId = 9; //TODO:AKS -> Change it
            receipt.OrganizationId = 2;
            receipt.Status = receiptStatus;

            // Add item details to receipt
            foreach (var item in invoice.InvoiceDetails)
            {
                var receiptDetail = new Receiptdetail();
                receiptDetail.InvoiceDetailId = item.InvoiceDetailId;
                receiptDetail.Description = item.Description;
                receiptDetail.ItemType = (int)ShoppingCartItemType.Membership;//TODO AKS-> Change
                receiptDetail.Rate = item.Price;
                receiptDetail.Quantity = item.Quantity;
                receiptDetail.Amount = item.Amount;
                receiptDetail.Status = receiptStatus;
                receiptDetail.EntityId = invoice.EntityId;
                receipt.Receiptdetails.Add(receiptDetail);
            }
            try
            {
                await _unitOfWork.ReceiptHeaders.AddAsync(receipt);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to add Items to the draft.");
            }
            //Add Receipt to Draft
            //draft.ReceiptId = receipt.Receiptid;
            draft.IsProcessed = -1;  //TODO AKS - ADD it in constants
            _unitOfWork.AutoBillingDrafts.Update(draft);
            await _unitOfWork.CommitAsync();

            return receipt;
        }

        public async Task<bool> UpdateReceipt(ReceiptHeaderModel receiptHeaderModel)
        {
            Receiptheader receiptheader = await _unitOfWork.ReceiptHeaders.GetReceiptByIdAsync(receiptHeaderModel.Receiptid);

            if (receiptheader != null)
            {
                //receiptheader.Date = DateTime.UtcNow;
                //receiptheader.StaffId = receiptHeaderModel.StaffId;
                //receiptheader.PaymentMode = receiptHeaderModel.PaymentMode;
                //receiptheader.CheckNo = receiptHeaderModel.CheckNo;
                receiptheader.Status = receiptHeaderModel.Status;
                //receiptheader.OrganizationId = receiptHeaderModel.OrganizationId;
                receiptheader.Notes = receiptHeaderModel.Notes;

                _unitOfWork.ReceiptHeaders.Update(receiptheader);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<Receiptheader> GetReceiptById(int receiptId)
        {
            return await _unitOfWork.ReceiptHeaders.GetReceiptByIdAsync(receiptId);
        }

        public async Task<Receiptheader> GetReceiptDetailById(int receiptId)
        {
            return await _unitOfWork.ReceiptHeaders.GetReceiptDetailById(receiptId);
        }

        public async Task<ReceiptModel> GetReceiptDetailByReceiptId(int receiptId)
        {
            ReceiptModel receiptModel = new ReceiptModel();
            var cart = await _unitOfWork.ShoppingCarts.SingleOrDefaultAsync(a => a.ReceiptId == receiptId);
            if (cart != null)
            {
                receiptModel = await GetReceiptModel(receiptId, cart.ShoppingCartId);
                if (cart.UseCreditBalance > 0)
                {
                    receiptModel.CreditUsed = cart.CreditBalance;
                    receiptModel.AmountExceptCreditUsed = receiptModel.TotalAmount - cart.CreditBalance;
                }
                else
                {
                    receiptModel.AmountExceptCreditUsed = receiptModel.TotalAmount;
                }
            }
            else
            {
                receiptModel = await GetReceiptModel(receiptId);
            }
            return receiptModel;
        }
        public async Task<ReceiptModel> GetReceiptDetailByCartId(int cartId)
        {
            ReceiptModel receiptModel = new ReceiptModel();
            var cart = await _unitOfWork.ShoppingCarts.GetByIdAsync(cartId);
            if (cart != null)
            {
                receiptModel = await GetReceiptModel(cart.ReceiptId ?? 0, cartId);

                if (cart.UseCreditBalance > 0)
                {
                    receiptModel.CreditUsed = cart.CreditBalance;
                    receiptModel.AmountExceptCreditUsed = receiptModel.TotalAmount - cart.CreditBalance;
                }
                else
                {
                    receiptModel.AmountExceptCreditUsed = receiptModel.TotalAmount;
                }

            }
            return receiptModel;
        }

        public async Task<LineChartModel> GetDailySalesByMonth()
        {
            var chartData = await _unitOfWork.ReceiptHeaders.GetDailySalesByMonth();

            //Break the data in 12 Data Sets :1 for each month
            //The group name format is month|day
            LineChartModel lineChart = new LineChartModel();

            for (int month = 1; month <= 12; month++)
            {
                var monthData = chartData.Where(x => x.Month == month).Select(x => new { Day = x.Day, SaleAmount = x.SaleAmount }).ToList();
                if (monthData != null)
                {

                    List<int> data = new List<int>();
                    List<string> color = new List<string>();
                    List<string> labels = new List<string>();
                    LineChartDataset lineChartDataSet = new LineChartDataset();
                    List<string> months = new List<string>(12);
                    for (int i = 1; i <= 31; i++)
                    {
                        var dailyChart = monthData.Where(x => x.Day == i).FirstOrDefault();
                        if (dailyChart != null)
                        {

                            labels.Add(i.ToString());
                            data.Add(Convert.ToInt32(dailyChart.SaleAmount));
                        }
                        else
                        {
                            labels.Add(i.ToString());
                            data.Add(0);
                        }

                    }
                    lineChart.Labels = labels;
                    lineChartDataSet = new LineChartDataset { Label = month.GetAbbreviatedMonthName(), Data = data, Fill = true, BackgroundColor = GraphHelper.GetRandomColor(), BorderColor = GraphHelper.GetRandomColor() };
                    lineChart.Datasets.Add(lineChartDataSet);

                }

            }

            return lineChart;
        }


        public async Task<IEnumerable<Receiptheader>> GetAllReceipts()
        {
            return await _unitOfWork.ReceiptHeaders.GetAllReceiptsAsync();
        }
        public async Task<IEnumerable<Receiptheader>> GetReceiptsByDateRange(DateTime fromDate, DateTime toDate)
        {
            return await _unitOfWork.ReceiptHeaders.GetReceiptsByDateRangeAsync(fromDate, toDate);
        }
        public async Task<IEnumerable<Receiptheader>> GetReceiptsByOrganizationId(int organizationId)
        {
            return await _unitOfWork.ReceiptHeaders.GetReceiptsByOrganizationIdAsync(organizationId);
        }
        public async Task<IEnumerable<Receiptheader>> GetReceiptsByStaffId(int staffId)
        {
            return await _unitOfWork.ReceiptHeaders.GetReceiptsByStaffIdAsync(staffId);
        }
        public async Task<ReceiptModel> GetReceiptModel(int receiptId, int? cartId = 0)
        {
            ReceiptModel receiptModel = new ReceiptModel();
            var receipt = await _unitOfWork.ReceiptHeaders.GetReceiptDetailById(receiptId);
            receiptModel = _mapper.Map<ReceiptModel>(receipt);
            var shoppingcartdetails = await _unitOfWork.ShoppingCartDetails.GetShoppingCartDetailsByCartIdAsync(cartId ?? 0);


            var receciptDetails = await _unitOfWork.ReceiptDetails.GetReceiptDetailsByReceiptIdAsync(receipt.Receiptid);
            receciptDetails = receciptDetails.Where(x => x.Status == (int)ReceiptStatus.Active);
            //var invoices = receciptDetails.Select(x => x.InvoiceDetail.InvoiceId).ToList().Distinct();
            receiptModel.PaymentMode = receipt.PaymentMode;
            int invoiceId = 0;
            var lineItem = new ReceiptLineItem();
            if (shoppingcartdetails.Count() > 0)
            {
                var shoppingcartdetailsIdList = shoppingcartdetails?.Select(x => x.ItemId).ToList();
                if (shoppingcartdetailsIdList.Count() > 0)
                {
                    receciptDetails = receciptDetails.OrderBy(x => shoppingcartdetailsIdList?.IndexOf(x.InvoiceDetailId)).ToList();
                }
            }

            foreach (var item in receciptDetails)
            {
                var invoice = await _unitOfWork.Invoices
                .GetInvoicePrintDetailsByIdAsync(item.InvoiceDetail.InvoiceId);
                if (invoiceId == 0 || invoiceId != item.InvoiceDetail.InvoiceId)
                {
                    lineItem = new ReceiptLineItem();

                    if (receiptModel.BillableEntity == null)
                    {
                        receiptModel.BillableEntity = _mapper.Map<EntityModel>(invoice.BillableEntity);
                    }

                    if (receiptModel.EntityId == null)
                    {
                        receiptModel.EntityId = invoice.Entity?.EntityId;
                    }
                    //Get billing Address

                    BillingAddressModel billingAddress = new BillingAddressModel();

                    var entity = await _unitOfWork.Entities
                        .GetByIdAsync(invoice.BillableEntity.EntityId);

                    if (entity.PersonId != null)
                    {
                        var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);

                        var personModel = _mapper.Map<PersonModel>(person);
                        var primaryAddress = personModel.Addresses.GetPrimaryAddress();

                        billingAddress.BillToEmail = personModel.Emails.GetPrimaryEmail();
                        billingAddress.BillToName = $"{personModel.Prefix} {personModel.FirstName} {personModel.LastName}";
                        billingAddress.StreetAddress = primaryAddress.StreetAddress;
                        billingAddress.City = primaryAddress.City;
                        billingAddress.State = primaryAddress.State;
                        billingAddress.Zip = primaryAddress.Zip.FormatZip();
                    }
                    else
                    {
                        var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);
                        
                        billingAddress.BillToName = company.CompanyName;
                        var companyModel = _mapper.Map<CompanyModel>(company);
                        var primaryEMail = companyModel.Emails.GetPrimaryEmail();
                        billingAddress.BillToEmail = primaryEMail;
                        var primaryAddress = companyModel.Addresses.GetPrimaryAddress();
                        if (primaryAddress != null)
                        {
                            billingAddress.StreetAddress = $"{primaryAddress.Address1} {primaryAddress.Address2} {primaryAddress.Address3}";
                            billingAddress.City = primaryAddress.City;
                            billingAddress.State = primaryAddress.State;
                            billingAddress.Zip = primaryAddress.Zip.FormatZip();
                        }
                        else
                        {
                            var address = companyModel.Addresses.FirstOrDefault();
                            if (address != null)
                            {
                                billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                                billingAddress.City = address.City;
                                billingAddress.State = address.State;
                                billingAddress.Zip = address.Zip.FormatZip();
                            }
                        }
                    }
                    receiptModel.BillingAddress = billingAddress;
                    if (item.ItemType == (int)InvoiceItemType.Membership) //&& lineItem.MembershipCategory != null)
                    {
                        lineItem.ItemGroupDescription = ShoppingCartItemType.Membership.ToString();
                        if (invoice.Membership != null)
                        {
                            lineItem.MembershipCategory = invoice.Membership.MembershipType.CategoryNavigation.Name + System.Environment.NewLine;
                            lineItem.MembershipPeriod = $"Starts :{invoice.Membership.StartDate.ToString("MM/dd/yyyy")} Ends: {invoice.Membership.EndDate.ToString("MM/dd/yyyy")}{System.Environment.NewLine}";
                            lineItem.MembershipName = invoice.Membership.MembershipType.Name + System.Environment.NewLine;
                            lineItem.Quantity = string.Empty;
                            lineItem.Rate = string.Empty;
                            lineItem.Tax = string.Empty;
                            lineItem.Description = string.Empty;
                            lineItem.Amount = string.Empty;
                            lineItem.Discount = string.Empty;
                            receiptModel.LineItems.Add(lineItem);
                        }
                    }

                    if (item.ItemType == (int)InvoiceItemType.Event)
                    {
                        lineItem.ItemGroupDescription = EnumUtil.GetDescription(((InvoiceItemType)item.ItemType));
                        if (invoice.Event != null)
                        {
                            lineItem.EventName = invoice.Event.Name;
                            lineItem.EventType = invoice.Event.EventTypeId == 1 ? "In-Person" : invoice.Event.EventTypeId == 2 ? "Virtual" : "Pre Recorded";
                            lineItem.MembershipCategory = string.Empty;
                            lineItem.MembershipPeriod = string.Empty;
                            lineItem.MembershipName = string.Empty;
                            lineItem.Quantity = string.Empty;
                            lineItem.Rate = string.Empty;
                            lineItem.Tax = string.Empty;
                            lineItem.Description = string.Empty;
                            lineItem.Amount = string.Empty;
                            lineItem.Discount = string.Empty;
                            receiptModel.LineItems.Add(lineItem);
                        }
                    }


                    invoiceId = item.InvoiceDetail.InvoiceId;

                    #region code to check if any item has zero amount in InvoiceDetails
                    ////commented the code for ticket LBOLT-1847
                    //foreach (var subItem in invoice.Invoicedetails)
                    //{
                    //    if (subItem.Status == (int)InvoiceStatus.FullyPaid)
                    //    {
                    //        if (!receciptDetails.Select(x => x.InvoiceDetail.InvoiceDetailId)
                    //                        .Contains(subItem.InvoiceDetailId))
                    //        {
                    //            lineItem = new ReceiptLineItem();
                    //            if (subItem.ItemType == (int)InvoiceItemType.Membership)
                    //            {
                    //                lineItem.ItemGroupDescription = ShoppingCartItemType.Membership.ToString();
                    //            }
                    //            else
                    //            {
                    //                lineItem.ItemGroupDescription = EnumUtil.GetDescription(((InvoiceItemType)subItem.ItemType));
                    //            }
                    //            lineItem.MembershipCategory = string.Empty;
                    //            lineItem.MembershipPeriod = string.Empty;
                    //            lineItem.MembershipName = string.Empty;
                    //            lineItem.Quantity = subItem.Quantity.ToString();
                    //            lineItem.Rate = (subItem.Price).ToCurrency();
                    //            lineItem.Tax = (subItem.TaxRate ?? 0).ToCurrency();
                    //            lineItem.Description = subItem.Description;
                    //            lineItem.Amount = subItem.Amount.ToCurrency();
                    //            lineItem.Discount = subItem.Discount.ToCurrency();
                    //            receiptModel.LineItems.Add(lineItem);
                    //        }
                    //    }
                    //}
                    #endregion
                }
                lineItem = new ReceiptLineItem();
                if (item.ItemType == (int)InvoiceItemType.Membership)
                {
                    lineItem.ItemGroupDescription = ShoppingCartItemType.Membership.ToString();
                }
                else
                {
                    lineItem.ItemGroupDescription = EnumUtil.GetDescription(((InvoiceItemType)item.ItemType));
                }
                lineItem.MembershipCategory = string.Empty;
                lineItem.MembershipPeriod = string.Empty;
                lineItem.MembershipName = string.Empty;
                lineItem.Quantity = item.Quantity.ToString();
                lineItem.Rate = (item.Rate ?? 0).ToCurrency();
                lineItem.Tax = (item.Tax ?? 0).ToCurrency();
                lineItem.Description = item.Description;
                lineItem.Amount = item.Amount.ToCurrency();
                lineItem.Discount = item.Discount.ToCurrency();
                receiptModel.LineItems.Add(lineItem);

            }
            receiptModel.TotalDiscount = receciptDetails.Where(x => x.Status == (int)Status.Active).Sum(x => x.Discount);
            receiptModel.TotalAmount = receciptDetails.Where(x => x.Status == (int)Status.Active).Sum(x => x.Amount);
            receiptModel.TotalDueAmount = receiptModel.TotalAmount;
            return receiptModel;
        }

    }
}
