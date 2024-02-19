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
using System.Linq;
using static Max.Core.Constants;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Max.Services.Helpers;

namespace Max.Services
{
    public class InvoiceService : IInvoiceService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IEmailService _emailService;
        private readonly ITenantService _tenantService;
        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InvoiceService> logger, IEmailService emailService, ITenantService tenantService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._logger = logger;
            this._emailService = emailService;
            this._tenantService = tenantService;
        }


        public async Task<IEnumerable<Invoice>> GetAllInvoices()
        {
            return await _unitOfWork.Invoices
                .GetAllInvoicesAsync();
        }

        public async Task<IEnumerable<InvoicePaymentModel>> GetAllInvoicesByEntityId(int entityId, string sortOrder, string paymentStatus)
        {
            var invoices = await _unitOfWork.Invoices
                .GetAllInvoicesByEntityIdAsync(entityId);

            List<InvoicePaymentModel> invoiceList = new List<InvoicePaymentModel>();

            if (invoices.Count() > 0)
            {
                foreach (var invoice in invoices)
                {
                    InvoicePaymentModel item = new InvoicePaymentModel();
                    item.EntityId = invoice.EntityId ?? 0;
                    item.InvoiceId = invoice.InvoiceId;
                    item.Description = invoice.BillingType;
                    item.InvoiceType = invoice.InvoiceType;
                    item.Date = invoice.Date;
                    item.DueDate = invoice.DueDate;
                    item.Total = invoice.Invoicedetails.Sum(x => x.Amount);
                    item.Paid = invoice.Invoicedetails.Select(x => x.Receiptdetails.Where(x => x.Status == (int)Status.Active).Sum(x => x.Amount)).Sum();
                    item.WriteOff = invoice.Invoicedetails.Select(x => x.Writeoffs.Sum(x => x.Amount ?? 0)).Sum();
                    item.Balance = item.Total - item.Paid - item.WriteOff;
                    if (paymentStatus.ToUpper() == "PAID" && item.Balance > 0.0M)
                    {
                        continue;
                    }
                    if (paymentStatus.ToUpper() == "BALANCEDUE" && item.Balance == 0.0M)
                    {
                        continue;
                    }

                    //Add invoice details
                    foreach (var invoiceDetail in invoice.Invoicedetails)
                    {
                        var invoiceDetailModel = new InvoiceDetailModel();
                        invoiceDetailModel.InvoiceDetailId = invoiceDetail.InvoiceDetailId;
                        invoiceDetailModel.InvoiceId = invoiceDetail.InvoiceId;
                        invoiceDetailModel.Description = invoiceDetail.Description;
                        invoiceDetailModel.Price = invoiceDetail.Price;
                        invoiceDetailModel.Quantity = invoiceDetail.Quantity;
                        invoiceDetailModel.Discount = invoiceDetail.Discount;
                        invoiceDetailModel.Amount = invoiceDetail.Amount;
                        var paid = invoiceDetail.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created && x.Status != (int)ReceiptStatus.Void).Sum(x => x.Amount);
                        var refund = invoiceDetail.Receiptdetails.Select(x => x.Refunddetails.Sum(x => x.Amount)).Sum();
                        var writeOff = invoiceDetail.Writeoffs.Select(x => x.Amount).Sum() ?? 0;
                        item.Refund = refund;
                        var balance = invoiceDetailModel.Amount - paid - writeOff;
                        invoiceDetailModel.ItemId = invoiceDetail.ItemId ?? 0;
                        invoiceDetailModel.ItemId = invoiceDetail.ItemId ?? 0;
                        invoiceDetailModel.Status = invoiceDetail.Status;
                        if (paymentStatus.ToUpper() == "PAID" && balance > 0.0M)
                        {
                            continue;
                        }
                        if (paymentStatus.ToUpper() == "BALANCEDUE" && balance == 0.0M)
                        {
                            continue;
                        }
                        item.InvoiceDetails.Add(invoiceDetailModel);
                    }
                    invoiceList.Add(item);
                }
            }
            if (sortOrder.ToUpper() == "ASCENDING")
            {
                return invoiceList.OrderBy(x => x.Date);
            }
            else
            {
                return invoiceList.OrderByDescending(x => x.Date);
            }

        }

        public decimal GetBalanceByEntityId(int entityId)
        {
            return _unitOfWork.Invoices.GetInvoiceBalanceByEntityId(entityId);

        }

        public async Task<IEnumerable<InvoicePaymentModel>> GetInvoicePaymentsByEntityId(int entityId)
        {
            return await GetInvoicesBySearchCondition(entityId, "All", "", MySQL_MinDate, MySQL_MinDate);
        }
        public async Task<IEnumerable<InvoicePaymentModel>> GetInvoicesBySearchCondition(int entityId, string serachBy, string itemDescription, DateTime start, DateTime end)
        {
            var invoices = await _unitOfWork.Invoices
                .GetInvoicesBySearchConditionAsync(entityId, serachBy, itemDescription, start, end);

            List<InvoicePaymentModel> invoiceList = new List<InvoicePaymentModel>();

            if (invoices.Count() > 0)
            {
                foreach (var invoice in invoices)
                {
                    foreach (var invoiceDetail in invoice.Invoicedetails)
                    {

                        var item = new InvoicePaymentModel();
                        item.EntityId = invoice.BillableEntityId ?? 0;
                        item.InvoiceId = invoice.InvoiceId;
                        item.InvoiceDetailId = invoiceDetail.InvoiceDetailId;
                        item.InvoiceType = invoice.InvoiceType;
                        item.MembershipId = invoice.MembershipId ?? 0;
                        item.BillableEntityName = invoice.Entity.Name;
                        if (invoice.BillableEntityId != invoice.EntityId)
                        {
                            item.BillableEntityName = invoice.BillableEntity.Name;
                        }

                        item.EntityName = invoice.Entity.Name;
                        if (invoiceDetail.BillableEntityId > 0)
                        {
                            var billableEntity = await _unitOfWork.Entities.GetEntityByIdAsync((int)invoiceDetail.BillableEntityId);
                            item.EntityName = billableEntity.Name;
                        }

                        if (item.MembershipId > 0)
                        {
                            item.MembershipType = invoice.Membership.MembershipType.Name;
                            item.MembershipTypeId = invoice.Membership.MembershipTypeId ?? 0;
                        }
                        else
                        {
                            item.MembershipType = string.Empty;
                            item.MembershipTypeId = 0;
                        }
                        item.EventId = invoice.EventId;
                        item.Date = invoice.Date;
                        item.DueDate = invoice.DueDate;
                        item.Amount = invoiceDetail.Amount;
                        item.Total = invoiceDetail.Amount;
                        item.Discount = invoiceDetail.Discount;
                        item.Paid = invoiceDetail.Receiptdetails.Where(x => x.Status == (int)ReceiptStatus.Active || x.Status == (int)ReceiptStatus.Cancelled).Sum(x => x.Amount);
                        item.Refund = invoiceDetail.Receiptdetails.Select(x => x.Refunddetails.Sum(x => x.Amount)).Sum();
                        item.WriteOff = invoiceDetail.Writeoffs.Select(x => x.Amount ?? 0).Sum();
                        item.Balance = item.Amount - item.Paid - item.WriteOff;
                        item.Notes = invoice.Notes;
                        item.Description = invoiceDetail.Description;
                        item.ItemType = invoiceDetail.ItemType;
                        item.ItemId = invoiceDetail.ItemId ?? 0;
                        item.FeeId = invoiceDetail.FeeId ?? 0;
                        if (invoice.Paperinvoices.Count() > 0)
                        {
                            var checkForPaperInvoice = invoice.Paperinvoices.FirstOrDefault(i => i.InvoiceId == invoice.InvoiceId);
                            if (checkForPaperInvoice != null)
                            {
                                if (checkForPaperInvoice.Status == (int)BillingStatus.Finalizing)
                                {
                                    item.IsPaperInvoiceFinalized = false;
                                }
                            }
                        }

                        foreach (var paymentItem in invoiceDetail.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created))
                        {
                            var paymentDetail = new PaymentDetailModel();
                            paymentDetail.ReceiptId = paymentItem.ReceiptHeaderId ?? 0;
                            paymentDetail.ReceiptDetailId = paymentItem.ReceiptDetailId;
                            paymentDetail.Amount = paymentItem.Amount;
                            paymentDetail.Tax = paymentItem.Tax ?? 0;
                            paymentDetail.PaymentDate = paymentItem.ReceiptHeader.Date;
                            paymentDetail.PaymentMode = paymentItem.ReceiptHeader.PaymentMode;
                            paymentDetail.RefundDetailId = paymentItem.Refunddetails.Select(x => x.RefundId).FirstOrDefault();
                            paymentDetail.Refund = paymentItem.Refunddetails.Select(x => x.Amount).Sum();
                            paymentDetail.Status = paymentItem.Status;
                            item.PaymentDetails.Add(paymentDetail);
                        }
                        foreach (var writeOffItem in invoiceDetail.Writeoffs)
                        {
                            var writeOff = new WriteOffModel();
                            writeOff.Date = (DateTime)writeOffItem.Date;
                            writeOff.User = _mapper.Map<StaffUserModel>(writeOffItem.User);
                            writeOff.WriteOffId = writeOffItem.WriteOffId;
                            writeOff.Reason = writeOffItem.Reason;
                            writeOff.Amount = writeOffItem.Amount ?? 0;
                            item.WriteOffDetails.Add(writeOff);
                        }
                        invoiceList.Add(item);
                    }
                }
            }
            if (serachBy.ToUpper().Contains("LAST"))
            {
                return invoiceList.OrderByDescending(x => x.Date).Take(20);
            }
            else
            {
                return invoiceList.OrderByDescending(x => x.Date);
            }
        }
        public async Task<List<InvoicePaymentModel>> GetInvoicePaymentsByInvoiceId(int invoiceId)
        {
            var invoice = await _unitOfWork.Invoices.GetInvoicePaymentsByIdAsync(invoiceId);
            List<InvoicePaymentModel> invoices = new List<InvoicePaymentModel>();
            if (invoice != null)
            {
                //Add Details
                foreach (var invoiceDetail in invoice.Invoicedetails)
                {
                    var item = new InvoicePaymentModel();
                    item.EntityId = invoice.BillableEntityId ?? 0;
                    item.EntityName = invoice.Entity.Name;
                    item.InvoiceId = invoice.InvoiceId;
                    item.InvoiceType = invoice.InvoiceType;
                    item.MembershipId = invoice.MembershipId ?? 0;
                    if (invoice.Membership != null)
                    {
                        item.MembershipTypeId = invoice.Membership.MembershipTypeId ?? 0;
                        item.MembershipType = invoice.Membership.MembershipType.Name;
                    }
                    else
                    {
                        item.MembershipTypeId = 0;
                        item.MembershipType = string.Empty;
                    }
                    item.Date = invoice.Date;
                    item.DueDate = invoice.DueDate;
                    item.Amount = invoiceDetail.Amount;
                    item.Discount = invoiceDetail.Discount;
                    item.Paid = invoiceDetail.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created && x.Status != (int)ReceiptStatus.Void).Sum(x => x.Amount);
                    item.Refund = invoiceDetail.Receiptdetails.Select(x => x.Refunddetails.Sum(x => x.Amount)).Sum();
                    item.WriteOff = invoiceDetail.Writeoffs.Select(x => x.Amount).Sum() ?? 0;
                    item.Balance = item.Amount - item.Paid - item.WriteOff;
                    item.Notes = invoice.Notes;
                    item.Description = invoiceDetail.Description;
                    item.ItemType = invoiceDetail.ItemType;
                    item.ItemId = invoiceDetail.ItemId ?? 0;
                    item.FeeId = invoiceDetail.FeeId ?? 0;
                    item.ReceiptId = invoiceDetail.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created).OrderByDescending(f => f.ReceiptHeaderId).Select(x => x.ReceiptHeaderId).FirstOrDefault() ?? 0;


                    foreach (var paymentItem in invoiceDetail.Receiptdetails)
                    {
                        var paymentDetail = new PaymentDetailModel();
                        paymentDetail.ReceiptId = paymentItem.ReceiptHeaderId ?? 0;
                        paymentDetail.ReceiptDetailId = paymentItem.ReceiptDetailId;
                        paymentDetail.Amount = paymentItem.Amount;
                        paymentDetail.Tax = paymentItem.Tax ?? 0;
                        paymentDetail.PaymentDate = paymentItem.ReceiptHeader.Date;
                        paymentDetail.PaymentMode = paymentItem.ReceiptHeader.PaymentMode;
                        paymentDetail.RefundDetailId = paymentItem.Refunddetails.Select(x => x.RefundId).FirstOrDefault();
                        item.PaymentDetails.Add(paymentDetail);
                    }
                    invoices.Add(item);
                }
            }
            return invoices;
        }

        public async Task<InvoiceModel> GetInvoiceById(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(id);
            if (invoice != null)
            {
                return _mapper.Map<InvoiceModel>(invoice);
            }
            else
            {
                throw new KeyNotFoundException($"Invoice: {id} not found.");
            }
        }
        public async Task<InvoiceModel> GetInvoiceDetailsByInvoiceId(int invoiceId)
        {
            var invoice = await _unitOfWork.Invoices.GetInvoicePrintDetailsByIdAsync(invoiceId);

            InvoiceModel model = _mapper.Map<InvoiceModel>(invoice);
            var writeOffInvoiceDetailCount = 0;
            foreach (var item in invoice.Invoicedetails)
            {
                if (item.Writeoffs.Select(x => x.Amount ?? 0).Sum() >= item.Amount)
                {
                    writeOffInvoiceDetailCount++;
                }
            }
            if (writeOffInvoiceDetailCount == invoice.Invoicedetails.Count())
            {
                model.IsAllInvoiceDetailsWriteOff = true;
            }
            //Get billing Address

            BillingAddressModel billingAddress = new BillingAddressModel();

            var entity = await _unitOfWork.Entities.GetByIdAsync(invoice.BillableEntity.EntityId);

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
                personModel.StreetAddress = primaryAddress.StreetAddress;
                personModel.City = primaryAddress.City;
                personModel.State = primaryAddress.State;
                personModel.Zip = primaryAddress.Zip.FormatZip();
                personModel.Country = primaryAddress.Country;
                model.BillableEntity.Person = personModel;
            }
            else
            {
                var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);

                var companyModel = _mapper.Map<CompanyModel>(company);
                var primaryAddress = companyModel.Addresses.GetPrimaryAddress();
                billingAddress.BillToEmail = companyModel.Emails.GetPrimaryEmail();
                billingAddress.BillToName = $"{company.CompanyName}";
                billingAddress.StreetAddress = primaryAddress.StreetAddress;
                billingAddress.City = primaryAddress.City;
                billingAddress.State = primaryAddress.State;
                billingAddress.Zip = primaryAddress.Zip.FormatZip();
                companyModel.StreetAddress = primaryAddress.StreetAddress;
                companyModel.City = primaryAddress.City;
                companyModel.State = primaryAddress.State;
                companyModel.Zip = primaryAddress.Zip.FormatZip();
                companyModel.Country = primaryAddress.Country;
                model.BillableEntity.Company = companyModel;
            }
            model.BillingAddress = billingAddress;

            var organization = await _unitOfWork.Organizations.GetByIdAsync(invoice.BillableEntity.OrganizationId ?? 0);
            if (organization != null)
            {
                model.Organization = _mapper.Map<OrganizationModel>(organization);
            }

            // get user details
            if (model.UserId > 0)
            {
                var user = _unitOfWork.Staffusers.GetStaffUserById(model.UserId);
                if (user != null)
                {
                    model.UserName = $"{user.FirstName} {user.LastName}";
                }
            }
            else
            {
                model.UserName = "";
            }

            if (invoice.Event != null)
            {
                model.Event = _mapper.Map<EventModel>(invoice.Event);
            }
            //model.AvailableCredit = await _unitOfWork.CreditTransactions.GetCreditBalanceByEntityIdAsync(entity.EntityId);
            return model;

        }
        public async Task<IEnumerable<MembershipDuesModel>> GetMembershipInvoiceDues()
        {
            var invoices = await _unitOfWork.Invoices.GetMembershipInvoicePayments();

            var result = invoices.Select(x => new MembershipDuesModel()
            {
                EntityId = x.BillableEntityId ?? 0,
                Name = x.BillableEntity.Name,
                InvoiceId = x.InvoiceId,
                CreateDate = x.Date,
                DueDate = x.DueDate,
                Description = x.Membership.MembershipType.Name,
                Frequency = String.Empty,
                TotalDue = x.Invoicedetails.Sum(x => x.Price - x.Discount),
                Paid = x.Invoicedetails.Select(r => r.Receiptdetails.Sum(x => x.Amount)).Sum(),
                InvoiceType = x.InvoiceType
            }).ToList();
            return result;
        }
        /// <summary>
        /// Generic Method to create an Invoice
        /// This should be called to create and invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<InvoiceModel> CreateInvoice(InvoiceModel model)
        {
            InvoiceModel invoiceModel = new InvoiceModel();

            //return empty if no invoice details
            if (model.InvoiceDetails.Count == 0)
            {
                return invoiceModel;
            }
            var isValid = await ValidInvoiceAsync(model);
            if (isValid)
            {
                //Map Model Data
                //var newInvoice = _mapper.Map<Invoice>(model);

                var invoice = new Invoice();

                invoice.BillableEntityId = model.BillableEntityId;
                invoice.BillingType = model.BillingType;
                invoice.Date = model.Date;
                invoice.DueDate = model.DueDate;
                invoice.InvoiceType = model.InvoiceType;
                invoice.MembershipId = model.MembershipId;
                invoice.Notes = model.Notes;
                invoice.EntityId = model.EntityId;
                invoice.UserId = model.UserId;
                invoice.Notes = model.Notes;
                invoice.Status = model.Status;

                //Add Details
                var membership = await _unitOfWork.Memberships.GetByIdAsync(invoice.MembershipId ?? 0);
                var startDate = membership.StartDate;
                var endDate = membership.EndDate;
                foreach (var item in model.InvoiceDetails)
                {
                    var invoiceDetail = new Invoicedetail();

                    invoiceDetail.ItemType = item.ItemType;
                    invoiceDetail.Description = item.Description;
                    if (item.Description.ToUpper().Trim().Contains("MEMBERSHIP FEE"))
                    {
                        invoiceDetail.Description = "Membership Fee : Period " + startDate.ToString("MM/dd/yyyy") + " - " + endDate.ToString("MM/dd/yyyy");
                    }
                    invoiceDetail.Amount = item.Amount;
                    invoiceDetail.FeeId = item.FeeId;
                    invoiceDetail.GlAccount = item.GlAccount;
                    invoiceDetail.Price = item.Price;
                    invoiceDetail.Price = item.Price;
                    invoiceDetail.Quantity = item.Quantity;
                    invoiceDetail.Status = item.Status;
                    invoice.Invoicedetails.Add(invoiceDetail);
                }
                try
                {
                    await _unitOfWork.Invoices.AddAsync(invoice);
                    await _unitOfWork.CommitAsync();

                    // Map invoice
                    invoiceModel.InvoiceId = invoice.InvoiceId;
                    invoiceModel.BillableEntityId = invoice.BillableEntityId;
                    invoiceModel.BillingType = invoice.BillingType;
                    invoiceModel.Date = invoice.Date;
                    invoiceModel.DueDate = invoice.DueDate;
                    invoiceModel.InvoiceType = invoice.InvoiceType;
                    invoiceModel.MembershipId = invoice.MembershipId;
                    invoiceModel.Notes = invoice.Notes;
                    invoiceModel.EntityId = invoice.EntityId ?? 0;
                    invoiceModel.UserId = invoice.UserId ?? 0;
                    invoiceModel.Notes = invoice.Notes;
                    invoiceModel.Status = invoice.Status;
                    invoiceModel.InvoiceDetails = _mapper.Map<List<InvoiceDetailModel>>(invoice.Invoicedetails);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
            }
            return invoiceModel;
        }
        public async Task<InvoiceModel> CreateItemInvoice(GeneralInvoiceModel model)
        {
            InvoiceModel invoiceModel = new InvoiceModel();

            //return empty if no invoice details
            if (model.InvoiceItems.Count == 0)
            {
                return invoiceModel;
            }
            var isValid = true;
            if (isValid)
            {

                var invoice = new Invoice();

                invoice.BillableEntityId = model.BillableEntityId;
                invoice.BillingType = BillingTypes.GENERAL;
                invoice.InvoiceType = InvoiceType.INDIVIDUAL;
                invoice.Date = DateTime.Now;
                invoice.DueDate = model.DueDate;
                invoice.Notes = model.Notes;
                invoice.EntityId = model.EntityId;
                invoice.UserId = model.UserId;
                invoice.Status = (int)InvoiceStatus.Finalized;
                invoice.InvoiceItemType = 3;

                //Add Details

                foreach (var item in model.InvoiceItems)
                {
                    var invoiceDetail = new Invoicedetail();
                    var itemDetail = new Item();
                    itemDetail = await _unitOfWork.Items.GetItemDetailByIdAsync(item.ItemId);
                    if (itemDetail.EnableStock == 1)
                    {
                        if (itemDetail.StockCount == 0)
                        {
                            throw new InvalidOperationException("No stocks available for this item.");
                        }
                        if (item.Quantity > itemDetail.StockCount)
                        {
                            throw new InvalidOperationException("Ordered quantity is more than stock left.");
                        }
                        itemDetail.StockCount -= item.Quantity;
                        try
                        {
                            //itemDetail.ItemTypeNavigation = null;
                            ItemModel itemModel = _mapper.Map<ItemModel>(itemDetail);
                            Item itemToUpdate = _mapper.Map<Item>(itemModel);
                            await _unitOfWork.Items.UpdateItem(itemToUpdate);
                            await _unitOfWork.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Unable to Update Item.");
                        }
                    }
                    invoiceDetail.ItemType = itemDetail.ItemType;
                    invoiceDetail.Description = itemDetail.Description;
                    //invoiceDetail.Amount = item.UnitRate * item.Quantity;
                    invoiceDetail.Amount = item.Amount ?? item.UnitRate * item.Quantity;
                    invoiceDetail.ItemId = item.ItemId;
                    invoiceDetail.GlAccount = itemDetail.ItemGlAccountNavigation.Code;
                    invoiceDetail.Price = item.UnitRate;
                    invoiceDetail.Quantity = item.Quantity;
                    invoiceDetail.Status = (int)Status.Active;
                    invoiceDetail.BillableEntityId = item.BillableEntityId ?? null;
                    invoice.Invoicedetails.Add(invoiceDetail);
                }
                try
                {
                    await _unitOfWork.Invoices.AddAsync(invoice);
                    await _unitOfWork.CommitAsync();
                    // Map invoice
                    invoiceModel = _mapper.Map<InvoiceModel>(invoice);
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }

            }
            return invoiceModel;
        }

        public async Task<Invoice> UpdateItemInvoice(GeneralInvoiceModel model)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(model.InvoiceId);                

                invoice.BillableEntityId = model.BillableEntityId;
                invoice.DueDate = model.DueDate;
                invoice.Notes = model.Notes;
                invoice.EntityId = model.EntityId;
                invoice.UserId = model.UserId;
                var invoiceDetails = _unitOfWork.InvoiceDetails.Find(x => x.InvoiceId == model.InvoiceId).ToList();
                foreach (var invoiceDetail in invoiceDetails)
                {
                    if (model.InvoiceItems.Any(x => x.InvoiceDetailId == invoiceDetail.InvoiceDetailId))
                    {
                        ItemModel itemModel = model.InvoiceItems.Where(x => x.InvoiceDetailId == invoiceDetail.InvoiceDetailId).FirstOrDefault();                        
                        var itemDetail = await _unitOfWork.Items.GetByIdAsync(itemModel.ItemId);
                        if (itemDetail.EnableStock == 1 && itemModel.Quantity != invoiceDetail.Quantity)
                        {
                            if (itemDetail.StockCount <= 0)
                            {
                                if (itemModel.Quantity > invoiceDetail.Quantity)
                                {
                                    throw new InvalidOperationException("No stocks available for this item.");
                                }
                            }
                            else
                            {
                                if ((invoiceDetail.Quantity - itemModel.Quantity) > itemDetail.StockCount)
                                {
                                    throw new InvalidOperationException("Ordered quantity is more than stock left.");
                                }
                            }
                            if (itemModel.Quantity < invoiceDetail.Quantity)
                            {
                                itemDetail.StockCount += invoiceDetail.Quantity - itemModel.Quantity;
                            }
                            else if (itemModel.Quantity > invoiceDetail.Quantity)
                            {
                                itemDetail.StockCount -= (itemModel.Quantity - invoiceDetail.Quantity);
                            }
                            else
                            {
                                itemDetail.StockCount -= itemModel.Quantity;
                            }

                            try
                            {
                                _unitOfWork.Items.Update(itemDetail);
                                await _unitOfWork.CommitAsync();
                            }
                            catch (Exception ex)
                            {
                                throw new InvalidOperationException($"Unable to Update Item.");
                            }
                        }

                        invoiceDetail.Price = itemModel.UnitRate;
                        invoiceDetail.Amount = itemModel.UnitRate * itemModel.Quantity;
                        invoiceDetail.Quantity = itemModel.Quantity;
                        invoiceDetail.BillableEntityId = itemModel.BillableEntityId;

                        _unitOfWork.InvoiceDetails.Update(invoiceDetail);
                        continue;
                    }

                    _unitOfWork.InvoiceDetails.Remove(invoiceDetail);
                    invoice.Invoicedetails.Remove(invoiceDetail);
                }

                foreach (var invoiceItem in model.InvoiceItems.Where(x => x.InvoiceDetailId == 0).ToList())
                {
                    var invoiceDetail = new Invoicedetail();
                    var itemDetail = await _unitOfWork.Items.GetItemDetailByIdAsync(invoiceItem.ItemId);
                    if (itemDetail.EnableStock == 1)
                    {
                        if (itemDetail.StockCount == 0)
                        {
                            throw new InvalidOperationException("No stocks available for this item.");
                        }
                        if (invoiceItem.Quantity > itemDetail.StockCount)
                        {
                            throw new InvalidOperationException("Ordered quantity is more than stock left.");
                        }

                        itemDetail.StockCount -= invoiceItem.Quantity;

                        try
                        {
                            ItemModel itemModel = _mapper.Map<ItemModel>(itemDetail);
                            Item itemToUpdate = _mapper.Map<Item>(itemModel);

                            _unitOfWork.Items.Update(itemToUpdate);
                            await _unitOfWork.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Unable to Update Item.");
                        }
                    }
                    invoiceDetail.ItemType = itemDetail.ItemType;
                    invoiceDetail.Description = itemDetail.Description;
                    invoiceDetail.Amount = invoiceItem.UnitRate * invoiceItem.Quantity;
                    invoiceDetail.ItemId = invoiceItem.ItemId;
                    invoiceDetail.GlAccount = itemDetail.ItemGlAccountNavigation.Code;
                    invoiceDetail.Price = invoiceItem.UnitRate;
                    invoiceDetail.Quantity = invoiceItem.Quantity;
                    invoiceDetail.BillableEntityId = invoiceItem.BillableEntityId;
                    invoiceDetail.Status = (int)Status.Active;
                    invoice.Invoicedetails.Add(invoiceDetail);
                }
                invoice.EntityId = model.EntityId;
                _unitOfWork.Invoices.Update(invoice);
                await _unitOfWork.CommitAsync();
                return invoice;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<InvoiceModel> CreateNewMembershipInvoice(MembershipSessionModel model)
        {
            InvoiceModel invoice = new InvoiceModel();

            invoice.EntityId = model.EntityId;
            invoice.Date = DateTime.Now;
            invoice.DueDate = model.StartDate;
            invoice.BillingType = BillingTypes.MEMBERSHIP;
            invoice.InvoiceType = InvoiceType.INDIVIDUAL;
            invoice.MembershipId = model.MembershipId;
            invoice.BillableEntityId = model.BillableEntityId;
            invoice.Status = (int)InvoiceStatus.Finalized;
            invoice.UserId = model.UserId;
            invoice.Notes = model.Notes;

            //Add Line Items
            var membershipFee = model.MembershipFees;
            int counter = 0;
            foreach (var feeItem in model.MembershipFeeIds)
            {
                var invoiceDetail = new InvoiceDetailModel();
                var fee = await _unitOfWork.Membershipfees.GetMembershipFeeByIdAsync(feeItem);
                var glAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(fee.GlAccountId ?? 0);
                var editedFee = membershipFee[counter];

                if (glAccount != null)
                {
                    invoiceDetail.ItemType = (int)InvoiceItemType.Membership;
                    invoiceDetail.Quantity = 1;
                    invoiceDetail.Description = fee.Description;
                    invoiceDetail.GlAccount = fee.GlAccount.Code;
                    if (fee.FeeAmount != editedFee)
                    {
                        invoiceDetail.Price = editedFee;
                    }
                    else
                    {
                        invoiceDetail.Price = fee.FeeAmount;
                    }
                    invoiceDetail.Taxable = 0;
                    invoiceDetail.TaxRate = 0;
                    invoiceDetail.TaxAmount = 0;
                    invoiceDetail.Amount = invoiceDetail.Price;
                    invoiceDetail.Status = (int)InvoiceStatus.Finalized;
                    invoiceDetail.FeeId = fee.FeeId;

                    invoice.InvoiceDetails.Add(invoiceDetail);
                }
                else
                {
                    _logger.LogInformation($"Gl Account not defined for Fee Id: {fee.FeeId} .");
                }
                counter++;
            }
            try
            {
                invoice = await CreateInvoice(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return invoice;
        }

        public async Task<InvoiceModel> CreateMembershipBillingInvoice(int membershipId, string invoiceType, DateTime nextBilldate, int cycleId = 0)
        {
            InvoiceModel invoice = new InvoiceModel();
            Paperinvoice deletedPaperInvoice = null;
            Paperinvoice finalizedPaperInvoice = null;
            Invoice pastDue = null;

            var user = await _unitOfWork.Staffusers.GetStaffUserByUserNameAsync("BillingService");
            var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync(membershipId);
            var membershipConnections = await _unitOfWork.MembershipConnections.GetMembershipConnectionsByMembershipIdAsync(membershipId);
            if (membership == null)
            {
                throw new NullReferenceException($"Membership: {membershipId} not found.");
            }

            var membershipConnection = membershipConnections.FirstOrDefault();


            if (invoiceType == InvoiceType.PAPER)
            {
                deletedPaperInvoice = await _unitOfWork.PaperInvoices.GetDeletedOrEditedPaperInvoicesByMembershipId(membershipId, cycleId);
                finalizedPaperInvoice = await _unitOfWork.PaperInvoices.GetFinalizedPaperInvoicesByCycleId(membershipId, cycleId);
                pastDue = finalizedPaperInvoice?.Invoice;
            }
            else
            {
                // if there is already an Invoice with finalized status then do not create
                pastDue = await _unitOfWork.Invoices.GetFinalizedInvoicesByMembershipId(membershipId, nextBilldate);
            }

            if (pastDue == null && deletedPaperInvoice == null)
            {
                if (membershipConnection != null)
                {
                    invoice.EntityId = membershipConnection.EntityId;
                }
                else
                {
                    invoice.EntityId = membership.BillableEntityId ?? 0;
                }
                invoice.Date = DateTime.Now;
                invoice.DueDate = membership.NextBillDate;
                invoice.BillingType = BillingTypes.MEMBERSHIP;
                invoice.InvoiceType = invoiceType;
                invoice.MembershipId = membership.MembershipId;
                invoice.BillableEntityId = membership.BillableEntityId;
                invoice.Status = (int)InvoiceStatus.Draft;
                if (user != null)
                {
                    invoice.UserId = user.UserId;
                }
                else
                {
                    invoice.UserId = 0;
                }

                //Add Line Items
                foreach (var feeItem in membership.Billingfees)
                {
                    var invoiceDetail = new InvoiceDetailModel();
                    var fee = feeItem.Fee;
                    var discount = feeItem.Discount;
                    var glAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(feeItem.MembershipFee.GlAccountId ?? 0);

                    if (glAccount != null && feeItem.MembershipFee.BillingFrequency == (int)FeeBillingFrequency.Recurring)
                    {
                        invoiceDetail.ItemType = (int)InvoiceItemType.Membership;
                        invoiceDetail.Quantity = 1;
                        invoiceDetail.Description = feeItem.MembershipFee.Description;
                        invoiceDetail.GlAccount = glAccount.Code;
                        invoiceDetail.Price = feeItem.Fee;
                        invoiceDetail.Discount = feeItem.Discount;
                        invoiceDetail.Taxable = 0;
                        invoiceDetail.TaxRate = 0;
                        invoiceDetail.TaxAmount = 0;
                        invoiceDetail.Amount = feeItem.Fee - feeItem.Discount;
                        invoiceDetail.Status = (int)InvoiceStatus.Draft;
                        invoiceDetail.FeeId = feeItem.MembershipFeeId;
                        invoice.InvoiceDetails.Add(invoiceDetail);
                    }
                    else
                    {
                        _logger.LogInformation($"Gl Account not defined for Fee Id: {feeItem.MembershipFeeId} or it is not a recurring fee so skipping.");
                    }

                }
            }
            try
            {
                invoice = await CreateInvoice(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return invoice;
        }

        public async Task<Invoice> UpdateInvoice(InvoiceModel model)
        {            
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(model.InvoiceId);

            if (invoice == null)
            {
                throw new InvalidOperationException($"Invoice: {model.InvoiceId} not found.");
            }
            var isValid = await ValidInvoiceAsync(model);
            if (isValid)
            {
                //Map Model Data
                invoice.EntityId = model.EntityId;
                invoice.Date = model.Date == null ? invoice.Date : model.Date;
                invoice.DueDate = model.DueDate == null ? invoice.DueDate : model.DueDate;
                invoice.BillingType = model.BillingType == null ? invoice.BillingType : model.BillingType;
                invoice.InvoiceType = model.InvoiceType == null ? invoice.InvoiceType : model.InvoiceType;
                invoice.MembershipId = model.MembershipId;
                invoice.BillableEntityId = model.BillableEntityId;
                invoice.Status = (int)model.Status;
                invoice.Notes = model.Notes;

                //var invoiceDetails = invoice.Invoicedetails.ToList();
                var invoiceDetails = _unitOfWork.InvoiceDetails.Find(x => x.InvoiceId == model.InvoiceId).ToList();

                foreach (var invoiceItem in invoiceDetails)
                {
                    if (model.InvoiceDetails.Any(x => x.InvoiceDetailId == invoiceItem.InvoiceDetailId))
                    {
                        //Update  Detail
                        Invoicedetail invoiceDetail = invoiceDetails.Where(x => x.InvoiceDetailId == invoiceItem.InvoiceDetailId).FirstOrDefault();
                        InvoiceDetailModel invoiceDetailModel = model.InvoiceDetails.Where(x => x.InvoiceDetailId == invoiceItem.InvoiceDetailId).FirstOrDefault();
                        //Map data
                        invoiceDetail.InvoiceId = invoiceItem.InvoiceId;
                        invoiceDetail.Description = invoiceItem.Description;
                        invoiceDetail.Price = invoiceDetailModel.Amount;
                        invoiceDetail.Amount = invoiceDetailModel.Amount;
                        invoiceDetail.Quantity = invoiceItem.Quantity;
                        invoiceDetail.Taxable = invoiceItem.Taxable;
                        invoiceDetail.GlAccount = invoiceItem.GlAccount;
                        invoiceDetail.Status = invoiceItem.Status;
                        invoiceDetail.FeeId = invoiceItem.FeeId;

                        _unitOfWork.InvoiceDetails.Update(invoiceDetail);


                        continue;

                    }

                    _unitOfWork.InvoiceDetails.Remove(invoiceItem);
                    invoice.Invoicedetails.Remove(invoiceItem);
                }

                foreach (var invoiceItem in model.InvoiceDetails.Where(x => x.InvoiceDetailId == 0).ToList())
                {
                    var glAccount = await _unitOfWork.GlAccounts.GetGlaccountByIdAsync(invoiceItem.GlAccountId);

                    Invoicedetail invoiceDetail = new Invoicedetail();
                    //Map data
                    invoiceDetail.InvoiceId = invoiceItem.InvoiceId;
                    invoiceDetail.Description = invoiceItem.Description;
                    invoiceDetail.Price = invoiceItem.Amount;
                    invoiceDetail.Amount = invoiceItem.Amount;
                    invoiceDetail.Quantity = 1;
                    invoiceDetail.Taxable = 0;
                    invoiceDetail.GlAccount = glAccount.Code;
                    invoiceDetail.Status = 0;
                    invoiceDetail.FeeId = invoiceItem.FeeId;
                    invoiceDetail.ItemType = 4;

                    invoice.Invoicedetails.Add(invoiceDetail);

                    Billingfee billingFee = new Billingfee();
                    billingFee.MembershipId = model.MembershipId ?? 0;
                    billingFee.MembershipFeeId = invoiceItem.FeeId;
                    billingFee.Fee = invoiceItem.Amount;
                    billingFee.Status = 1;

                    await _unitOfWork.BillingFees.AddAsync(billingFee);
                }

                _unitOfWork.Invoices.Update(invoice);
                await _unitOfWork.CommitAsync();
            }
            return invoice;
        }

        public async Task<bool> DeleteInvoice(int InvoiceId)
        {
            try
            {
                Invoice Invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(InvoiceId);

                if (Invoice != null)
                {
                    _unitOfWork.Invoices.Remove(Invoice);
                    await _unitOfWork.CommitAsync();
                    return true;

                }
                throw new InvalidOperationException($"Invoice: {InvoiceId} not found.");
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeletePendingInvoices(string invoiceType)
        {
            var pedningInvoice = _unitOfWork.Invoices.Find(x => x.InvoiceType == invoiceType && x.Status == (int)InvoiceStatus.Draft);

            if (pedningInvoice != null)
            {
                _unitOfWork.Invoices.RemoveRange(pedningInvoice);
                await _unitOfWork.CommitAsync();

            }
            return true;
        }

        private async Task<bool> ValidInvoiceAsync(InvoiceModel model)
        {
            //Validate  Name
            if (model.EntityId == 0)
            {
                throw new InvalidOperationException($"Entity Id not defined.");
            }
            if (model.BillableEntityId == 0)
            {
                throw new NullReferenceException($"Billable Member not defined.");
            }
            else
            {
                //Check if billable person exists
                var billableEntity = await _unitOfWork.Entities.GetByIdAsync(model.BillableEntityId ?? 0);
                if (billableEntity == null)
                {
                    throw new KeyNotFoundException($"No entity found with ID: {model.BillableEntityId}.");
                }
            }
            if (model.InvoiceDetails.Count() == 0)
            {
                throw new InvalidOperationException($"No item details found.");
            }

            return true;
        }

        public async Task<IEnumerable<ReceivablesReportMembershipDueModel>> GetAllOutstandingReceivables()
        {
            var invoices = await _unitOfWork.Invoices.GetAllActiveInvoicesWithDuesAsync();

            var result = invoices.Select(x => new ReceivablesReportMembershipDueModel()
            {
                PersonId = (int)(x.Entity.PersonId > 0 ? x.Entity.PersonId : x.Entity.CompanyId),
                EntityId = x.Entity.EntityId,
                BillableMemberName = GetBillableEntity.GetBillableName(x.BillableEntity),
                //BillableMemberName = x.BillableEntity.Name,
                //MemberName = x.Entity.Name,
                MemberName = GetBillableEntity.GetBillableName(x.Entity),
                InvoiceId = x.InvoiceId,
                CreatedDate = x.Date.ToShortDateString(),
                DueDate = x.DueDate.ToShortDateString(),
                Description = x.BillingType,
                TotalDue = x.Invoicedetails.Where(x => x.Status == (int)InvoiceStatus.Finalized).Sum(x => x.Amount),
                Paid = x.Invoicedetails.Where(x => x.Status == (int)InvoiceStatus.Finalized).Select(r => r.Receiptdetails.Sum(x => x.Amount)).Sum(),
                BillingType = x.BillingType
            }).ToList();
            result = result.OrderBy(x => DateTime.Parse(x.DueDate)).ToList();
            return result.Where(x => x.TotalDue - x.Paid > 0);
        }

        public async Task<IEnumerable<InvoicePaymentModel>> GetInvoicesByMultipleInvoiceIds(int[] invoiceIds)
        {
            var invoice = await _unitOfWork.Invoices.GetInvoicesByMultipleInvoiceIdsAsync(invoiceIds);
            if (invoice != null)
            {
                return _mapper.Map<IEnumerable<InvoicePaymentModel>>(invoice);
            }
            else
            {
                throw new KeyNotFoundException($"Invoice: {invoiceIds} not found.");
            }
        }

        public async Task<IEnumerable<InvoicePaymentModel>> GetInvoicesWithBalanceByEntityId(int entityId)
        {
            var invoicesList = await _unitOfWork.Invoices.GetInvoicesWithBalanceByEntityIdAsync(entityId);
            List<InvoicePaymentModel> invoicePaymentModel = new List<InvoicePaymentModel>();

            if (invoicesList != null)
            {
                foreach (var invoice in invoicesList)
                {
                    InvoicePaymentModel invoiceItem = new InvoicePaymentModel();
                    invoiceItem.InvoiceId = invoice.InvoiceId;
                    invoiceItem.Date = invoice.Date;
                    invoiceItem.DueDate = invoice.DueDate;
                    invoiceItem.Description = invoice.MembershipId > 0 ? "Membership" : "General";
                    invoiceItem.Total = invoice.Invoicedetails.Where(x => x.Status == (int)InvoiceStatus.Finalized).Sum(x => x.Amount);
                    invoiceItem.Paid = invoice.Invoicedetails.Where(x => x.Status == (int)InvoiceStatus.Finalized).Select(x => x.Receiptdetails.Sum(x => x.Amount)).Sum();
                    invoiceItem.Balance = invoiceItem.Total - invoiceItem.Paid;
                    invoicePaymentModel.Add(invoiceItem);
                }
            }
            else
            {
                throw new KeyNotFoundException($"Entity: {entityId} not found.");
            }

            return invoicePaymentModel;
        }

        public async Task<bool> SendHtmlInvoice(EmailMessageModel model)
        {

            var invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(model.InvoiceId);

            //Check if invoice has already been paid
            var invoicePayment = await GetInvoicePaymentsByInvoiceId(model.InvoiceId);
            var entity = await _unitOfWork.Entities.GetEntityByIdAsync(invoice.BillableEntityId ?? 0);
            var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(entity.OrganizationId ?? 0);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            string emailAddress = string.Empty;
            string name = string.Empty;

            BatchEmailNotificationModel emailModel = new BatchEmailNotificationModel();

            if (entity.PersonId != null)
            {
                var person = await _unitOfWork.Persons.GetEmailsByPersonId(entity.PersonId ?? 0);
                emailModel.Name = person.FirstName + " " + person.LastName;
                emailModel.EmailAddress = model.ReceipientAddress;
                emailModel.Subject = model.Subject;
            }
            else
            {
                var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);
                emailModel.Name = company.CompanyName;
                emailModel.Subject = model.Subject;
                emailModel.EmailAddress = model.ReceipientAddress;
            }

            emailModel.Invoice = _mapper.Map<InvoiceModel>(invoice);
            emailModel.InvoiceId = invoice.InvoiceId;
            emailModel.Organization = _mapper.Map<OrganizationModel>(organization);
            emailModel.BaseUrl = tenant.BaseUrl;
            emailModel.Invoice.IsBalanceAmount = true;
            emailModel.Invoice.BalanceAmount = invoicePayment.Sum(x => x.Balance);

            var billingEmail = new Billingemail();
            billingEmail.BillingCycleId = 0;
            billingEmail.InvoiceId = invoice.InvoiceId;
            billingEmail.Status = (int)EmailStatus.Pending;
            billingEmail.Token = GenerateEmailToken();
            billingEmail.EmailAddress = model.ReceipientAddress;
            emailModel.PaymentUrl = $"organization={emailModel.Organization.Name}&id={Base64UrlEncoder.Encode(billingEmail.Token)}";

            try
            {
                await _unitOfWork.BillingEmails.AddAsync(billingEmail);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }

            var result = await _emailService.SendBatchInvoiceNotification(emailModel);

            return result;
        }
        private string GenerateEmailToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
        public async Task<Invoicedetail> GetInvoiceDetail(int id)
        {
            var invoice = await _unitOfWork.InvoiceDetails.GetByIdAsync(id);
            if (invoice != null)
            {
                return invoice;
            }
            else
            {
                throw new KeyNotFoundException($"Invoice: {id} not found.");
            }
        }
    }
}
