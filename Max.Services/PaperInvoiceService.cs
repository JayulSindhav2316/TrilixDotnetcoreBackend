using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;
using System.Linq;
using Max.Core;
using AutoMapper;
using Max.Services.Helpers;

namespace Max.Services
{
    public class PaperInvoiceService : IPaperInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMembershipService _membershipService;
        private readonly IInvoiceService _invoiceService;
        public PaperInvoiceService(IUnitOfWork unitOfWork, IMapper mapper, IMembershipService membershipService, IInvoiceService invoiceService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _membershipService = membershipService;
            _invoiceService = invoiceService;
        }

        public async Task<IEnumerable<Paperinvoice>> GetPaperInvoicesWithInvoicesByCycleId(int id)
        {
            return await _unitOfWork.PaperInvoices.GetPaperInvoicesWithInvoicesByCycleId(id);
        }

        public async Task<IEnumerable<PaperInvoiceModel>> GetPaperInvoicesByCycleId(int id)
        {
            var result = await _unitOfWork.PaperInvoices.GetPaperInvoiceDetailsByCycleId(id);

            return result.Select(x => new PaperInvoiceModel()
            {
                PaperBillingCycleId = x.PaperBillingCycleId,
                PaperInvoiceId = x.PaperInvoiceId,
                InvoiceId = x.InvoiceId,
                EntityId = x.EntityId ?? 0,
                BillableName = GetBillableEntity.GetBillableName(x.Entity),
                Name = GetBillableEntity.GetBillableName(x.Invoice.Entity),
                DueDate = x.DueDate,
                Description = x.Description,
                Amount = x.Invoice.Invoicedetails.Sum(s => s.Price),
                CreateDate = x.PaperBillingCycle.RunDate,
                PreferredBillingNotifictaion = x.Entity.PreferredBillingCommunication switch
                {
                    0 => "Paper Invoice",
                    1 => "Email",
                    2 => "Paper Invoice & Email",
                    _ => "Paper Invoice"
                },
                Invoice = new InvoiceModel()
                {
                    UserId = (int)x.Invoice.UserId,
                    Membership = new MembershipModel()
                    {
                        MembershipTypeId = x.Invoice.Membership.MembershipTypeId,
                        MembershipType = new MembershipTypeModel()
                        { Name = x.Invoice.Membership.MembershipType.Name },
                        MembershipConnections = x.Invoice.Membership.Membershipconnections
                            .Select(a => new MembershipConnectionModel()
                            {
                                EntityId = a.EntityId
                            }).ToList()
                    },
                },
                AdditionalMembersCount = x.Invoice.Membership.Membershipconnections?
                  .Where(m => m.EntityId != x.Invoice.EntityId)?.DistinctBy(x => x.EntityId).Count(),
                MembershipCount = x.Invoice.Membership.Membershipconnections.Count.ToString(),
                MembershipType = x.Invoice.Membership.MembershipType.Name
            });
        }
        public async Task<IEnumerable<PaperInvoiceModel>> GetRenewalPaperInvoicesByCycleId(int id)
        {
            var result = await _unitOfWork.PaperInvoices.GetPaperInvoiceDetailsByCycleId(id);

            return result.Select(x => new PaperInvoiceModel()
            {
                PaperBillingCycleId = x.PaperBillingCycleId,
                PaperInvoiceId = x.PaperInvoiceId,
                InvoiceId = x.InvoiceId,
                EntityId = x.EntityId ?? 0,
                BillableName = GetBillableEntity.GetBillableName(x.Entity),
                Name = GetBillableEntity.GetBillableName(x.Invoice.Entity),
                DueDate = x.DueDate,
                Description = x.Description,
                Amount = x.Invoice.Invoicedetails.Sum(s => s.Price),
                CreateDate = x.PaperBillingCycle.RunDate,
                PreferredBillingNotifictaion = x.Entity.PreferredBillingCommunication switch
                {
                    0 => "Paper Invoice",
                    1 => "Email",
                    2 => "Paper Invoice & Email",
                    _ => "Paper Invoice"
                },
                Invoice = new InvoiceModel()
                {
                    UserId = (int)x.Invoice.UserId,
                    Membership = new MembershipModel()
                    {
                        MembershipTypeId = x.Invoice.Membership.MembershipTypeId,
                        MembershipType = new MembershipTypeModel()
                        { Name = x.Invoice.Membership.MembershipType.Name },
                        MembershipConnections = x.Invoice.Membership.Membershipconnections
                            .Select(a => new MembershipConnectionModel()
                            {
                                EntityId = a.EntityId
                            }).ToList()
                    },
                },
                MembershipCount = x.Invoice.Membership.Membershipconnections.Count.ToString(),
                AdditionalMembersCount = x.Invoice.Membership.Membershipconnections?
                  .Where(m => m.EntityId != x.Invoice.EntityId)?.DistinctBy(x => x.EntityId).Count(),
                MembershipType = x.Invoice.Membership.MembershipType.Name,
                MembershipEndDate = x.Invoice.Membership.EndDate,
                MembershipNewEndDate = x.Invoice.Status == (int)InvoiceStatus.Draft ? MembershipUtil.GetRenewalEndDate(x.Invoice.Membership) : x.Invoice.Membership.EndDate

            });
        }
        public async Task<PaperInvoiceModel> GetPaperInvoiceDetailByInvoiceId(int id)
        {
            var result = await _unitOfWork.PaperInvoices.GetPaperInvoiceDetailsByInvoiceId(id);
            var paperInvoice = _mapper.Map<PaperInvoiceModel>(result);
            var model = new PaperInvoiceModel();

            model.PaperBillingCycleId = result.PaperBillingCycleId;
            model.PaperInvoiceId = result.PaperInvoiceId;
            model.InvoiceId = result.InvoiceId;
            model.EntityId = result.EntityId ?? 0;
            model.BillableName = result.Entity.Name;
            model.Name = result.Invoice.Entity.Name;
            model.DueDate = result.DueDate;
            model.Description = result.Description;
            model.Amount = result.Amount;
            model.CreateDate = result.CreateDate;
            model.PreferredBillingNotifictaion = result.Entity.PreferredBillingCommunication switch
            {
                0 => "Paper Invoice",
                1 => "Email",
                2 => "Paper Invoice & Email",
                _ => "Paper Invoice"
            };
            model.Status = result.Status;

            return model;
        }
        public async Task<List<PaperInvoiceModel>> GetPaperPrliminaryInvoices()
        {
            var result = await _unitOfWork.PaperInvoices.GetPaperPrliminaryInvoices();

            return result.Select(x => new PaperInvoiceModel()
            {
                PaperBillingCycleId = x.PaperBillingCycleId,
                PaperInvoiceId = x.PaperInvoiceId,
                InvoiceId = x.InvoiceId,
                EntityId = x.EntityId ?? 0,
                BillableName = GetBillableEntity.GetBillableName(x.Entity),
                Name = GetBillableEntity.GetBillableName(x.Invoice.Entity),
                DueDate = x.DueDate,
                Description = x.Description,
                Amount = x.Amount,
                CreateDate = x.PaperBillingCycle.RunDate
            }).ToList();
        }
        public async Task<Paperinvoice> GetPaperInvoiceById(int id)
        {
            return await _unitOfWork.PaperInvoices.GetPaperInvoiceByIdAsync(id);
        }
        public async Task<Paperinvoice> CreatePaperInvoice(PaperInvoiceModel model)
        {
            Paperinvoice paperInvoice = new Paperinvoice();

            paperInvoice.PaperBillingCycleId = model.PaperBillingCycleId;
            paperInvoice.CreateDate = DateTime.Now;
            paperInvoice.EntityId = model.EntityId;
            paperInvoice.Amount = model.Amount;
            paperInvoice.DueDate = model.DueDate;
            paperInvoice.Description = model.Description;
            paperInvoice.InvoiceId = model.InvoiceId;
            paperInvoice.Status = 0;

            await _unitOfWork.PaperInvoices.AddAsync(paperInvoice);
            await _unitOfWork.CommitAsync();
            return paperInvoice;
        }
        public async Task<bool> UpdatePaperInvoice(PaperInvoiceModel model)
        {
            Paperinvoice paperInvoice = await _unitOfWork.PaperInvoices.GetByIdAsync(model.PaperInvoiceId);

            if (paperInvoice != null)
            {
                paperInvoice.Amount = model.Amount;
                paperInvoice.Status = (int)PaperInvoiceStatus.Edited;

                _unitOfWork.PaperInvoices.Update(paperInvoice);
                await _unitOfWork.CommitAsync();

                return true;
            }
            return false;
        }
        public async Task<bool> FinalizePaperInvoicesByCycleId(int id)
        {
            var paperInvoices = await _unitOfWork.PaperInvoices.GetPaperInvoicesForFinalizationByCycleId(id);
            var cycle = await _unitOfWork.BillingCycles.GetBillingCycleByIdAsync(id);

            foreach (var paperInvoice in paperInvoices)
            {
                // check if the member is still has paperInvoice payment methods
                var membership = await _unitOfWork.Memberships.GetByIdAsync(paperInvoice.Invoice.MembershipId ?? 0);
                if (membership.AutoPayEnabled == (int)Status.Active)
                {
                    _unitOfWork.Invoices.Remove(paperInvoice.Invoice);
                    _unitOfWork.PaperInvoices.Remove(paperInvoice);
                    continue;
                }
                //Update invoiceDetails status
                foreach (var invoiceDetail in paperInvoice.Invoice.Invoicedetails)
                {
                    invoiceDetail.Status = (int)InvoiceStatus.Finalized;
                }

                if (paperInvoice.Entity.PreferredBillingCommunication == (int)BillingCommunication.PaperAndEmail
                        || paperInvoice.Entity.PreferredBillingCommunication == (int)BillingCommunication.EmailInvoice
                   )
                {
                    //Create Email queue
                    var billingEmaill = new Billingemail();
                    billingEmaill.BillingCycleId = id;
                    billingEmaill.InvoiceId = paperInvoice.InvoiceId;
                    billingEmaill.Status = (int)EmailStatus.Pending;
                    await _unitOfWork.BillingEmails.AddAsync(billingEmaill);
                }

                var periodStartDate = membership.NextBillDate;
                var periodEndDate = membership.NextBillDate;
                Membershiptype membershiptype = await _unitOfWork.Membershiptypes.GetByIdAsync(membership.MembershipTypeId ?? 0);
                if (membership != null)
                {
                    var frequency = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.PaymentFrequency ?? 0);
                    var period = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.PaymentFrequency ?? 0);
                    if (period.PeriodUnit == "Year")
                    {
                        periodEndDate = membership.NextBillDate.AddYears(period.Duration);
                    }
                    else if (frequency.PeriodUnit == "Month")
                    {
                        periodEndDate = membership.NextBillDate.AddMonths(period.Duration);
                    }
                    else
                    {
                        periodEndDate = membership.NextBillDate.AddDays(period.Duration);
                    }
                }

                List<Invoicedetail> invoiceDetailsListUpdated = new List<Invoicedetail>();
                foreach (var invoiceDetailItem in paperInvoice.Invoice.Invoicedetails)
                {
                    if (invoiceDetailItem.Description.ToUpper().Trim().Contains("MEMBERSHIP FEE"))
                    {
                        invoiceDetailItem.Description = "Membership Fee : Period " + periodStartDate.ToString("MM/dd/yyyy") + " - " + periodEndDate.ToString("MM/dd/yyyy");
                    }
                    invoiceDetailsListUpdated.Add(invoiceDetailItem);
                }
                paperInvoice.Invoice.Invoicedetails = invoiceDetailsListUpdated;
                paperInvoice.Invoice.Status = (int)InvoiceStatus.Finalized;
                paperInvoice.Status = (int)InvoiceStatus.Finalized;
                _unitOfWork.PaperInvoices.Update(paperInvoice);
                try
                {
                    if (membership.EndDate <= cycle.ThroughDate)
                    {
                        await _membershipService.RenewMembership(membership.MembershipId);
                    }
                    else
                    {
                        await _membershipService.UpdateNextBillDate(membership.MembershipId);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update membership records.");
                }
            }
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update invoice records.");
            }
            return true;
        }

        public async Task<bool> FinalizeRenewalPaperInvoicesByCycleId(int id)
        {
            var paperInvoices = await _unitOfWork.PaperInvoices.GetPaperInvoicesForFinalizationByCycleId(id);
            var cycle = await _unitOfWork.BillingCycles.GetBillingCycleByIdAsync(id);

            foreach (var paperInvoice in paperInvoices)
            {
                // check if the member is still has paperInvoice payment methods
                var membership = await _unitOfWork.Memberships.GetByIdAsync(paperInvoice.Invoice.MembershipId ?? 0);
                if (membership.AutoPayEnabled == (int)Status.Active)
                {
                    _unitOfWork.Invoices.Remove(paperInvoice.Invoice);
                    _unitOfWork.PaperInvoices.Remove(paperInvoice);
                    continue;
                }
                //Update invoiceDetails status
                foreach (var invoiceDetail in paperInvoice.Invoice.Invoicedetails)
                {
                    invoiceDetail.Status = (int)InvoiceStatus.Finalized;
                }

                if (paperInvoice.Entity.PreferredBillingCommunication == (int)BillingCommunication.PaperAndEmail
                        || paperInvoice.Entity.PreferredBillingCommunication == (int)BillingCommunication.EmailInvoice
                   )
                {
                    //Create Email queue
                    var billingEmaill = new Billingemail();
                    billingEmaill.BillingCycleId = id;
                    billingEmaill.InvoiceId = paperInvoice.InvoiceId;
                    billingEmaill.Status = (int)EmailStatus.Pending;
                    await _unitOfWork.BillingEmails.AddAsync(billingEmaill);
                }
                var endDate = membership.EndDate;
                Membershiptype membershiptype = await _unitOfWork.Membershiptypes.GetByIdAsync(membership.MembershipTypeId ?? 0);
                if (membership != null)
                {
                    var frequency = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.PaymentFrequency ?? 0);
                    var period = await _unitOfWork.MembershipPeriods.GetMembershipPeriodByIdAsync(membershiptype.Period ?? 0);
                    if (period.PeriodUnit == "Year")
                    {
                        endDate = membership.EndDate.AddYears(period.Duration);
                    }
                    else if (frequency.PeriodUnit == "Month")
                    {
                        endDate = membership.EndDate.AddMonths(period.Duration);
                    }
                    else
                    {
                        endDate = membership.EndDate.AddDays(period.Duration);
                    }
                }
                var startDate = membership.EndDate.AddDays(1);
                List<Invoicedetail> invoiceDetailsListUpdated = new List<Invoicedetail>();
                foreach (var invoiceDetailItem in paperInvoice.Invoice.Invoicedetails)
                {
                    if (invoiceDetailItem.Description.ToUpper().Trim().Contains("MEMBERSHIP FEE"))
                    {
                        invoiceDetailItem.Description = "Membership Fee : Period " + startDate.ToString("MM/dd/yyyy") + " - " + endDate.ToString("MM/dd/yyyy");
                    }
                    invoiceDetailsListUpdated.Add(invoiceDetailItem);
                }
                paperInvoice.Invoice.Invoicedetails = invoiceDetailsListUpdated;
                paperInvoice.Invoice.Status = (int)InvoiceStatus.Finalized;
                paperInvoice.Status = (int)InvoiceStatus.Finalized;
                _unitOfWork.PaperInvoices.Update(paperInvoice);
                try
                {
                    if (membership.EndDate <= cycle.ThroughDate)
                    {
                        await _membershipService.RenewMembership(membership.MembershipId);
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update membership records.");
                }
            }
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update invoice records.");
            }
            return true;
        }
        public async Task<IEnumerable<PaperInvoiceModel>> GetLastManualBillingDrafts(int billingCycleId)
        {
            if (billingCycleId == 0)
            {
                billingCycleId = await _unitOfWork.BillingCycles.GetLastFinalizedBillingCycleIdAsync();
            }
            return await GetPaperInvoicesByCycleId(billingCycleId);
        }

        public async Task<IEnumerable<InvoiceModel>> GetManualBillingInvoices(int billingCycleId)
        {
            List<InvoiceModel> invoices = new List<InvoiceModel>();

            var paperInvoices = await GetPaperInvoicesByCycleId(billingCycleId);

            foreach (var paperInvoice in paperInvoices)
            {
                if (paperInvoice.PreferredBillingNotifictaion.Contains("Paper"))
                {
                    var invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(paperInvoice.InvoiceId);
                    if (invoice != null)
                    {
                        var invoiceModel = _mapper.Map<InvoiceModel>(invoice);
                        var entity = await _unitOfWork.Entities.GetByIdAsync(invoice.BillableEntityId ?? 0);
                        BillingAddressModel billingAddress = new BillingAddressModel();
                        if (entity.PersonId != null)
                        {
                            var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);

                            var personModel = _mapper.Map<PersonModel>(person);

                            billingAddress.BillToEmail = personModel.Emails.GetPrimaryEmail();
                            billingAddress.BillToName = $"{personModel.Prefix} {personModel.FirstName} {personModel.LastName}";

                            var address = person.Addresses.Where(x => x.IsPrimary == (int)Status.Active).FirstOrDefault();
                            if (address != null)
                            {
                                billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                                billingAddress.City = address.City;
                                billingAddress.State = address.State;
                                billingAddress.Zip = address.Zip.FormatZip(); ;
                            }
                            else
                            {
                                address = person.Addresses.FirstOrDefault();
                                if (address != null)
                                {
                                    billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                                    billingAddress.City = address.City;
                                    billingAddress.State = address.State;
                                    billingAddress.Zip = address.Zip.FormatZip(); ;
                                }
                            }

                        }
                        else
                        {
                            var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);
                            CompanyModel companyModel = _mapper.Map<CompanyModel>(company);

                            billingAddress.BillToEmail = companyModel.Emails.GetPrimaryEmail();
                            billingAddress.BillToName = company.CompanyName;
                            var primaryAddress = companyModel.Addresses.GetPrimaryAddress();
                            billingAddress.StreetAddress = primaryAddress.StreetAddress;
                            billingAddress.City = primaryAddress.City;
                            billingAddress.State = primaryAddress.State;
                            billingAddress.Zip = primaryAddress.Zip.FormatZip();

                        }
                        invoiceModel.BillingAddress = billingAddress;
                        var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(invoice.UserId ?? 0);
                        if (staffUser != null)
                        {
                            invoiceModel.UserName = $"{staffUser.FirstName} {staffUser.LastName}";
                        }

                        invoices.Add(invoiceModel);
                    }
                }
            }
            return invoices;
        }
        public async Task<InvoiceModel> GetManualBillingInvoiceByInvoiceId(int invoiceId)
        {
            InvoiceModel invoiceModel = new InvoiceModel();

            var invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(invoiceId);

            if (invoice != null)
            {
                invoiceModel = _mapper.Map<InvoiceModel>(invoice);
                var entity = await _unitOfWork.Entities.GetByIdAsync(invoice.BillableEntityId ?? 0);
                BillingAddressModel billingAddress = new BillingAddressModel();
                if (entity.PersonId != null)
                {
                    var person = await _unitOfWork.Persons.GetPersonByIdAsync(entity.PersonId ?? 0);

                    var personModel = _mapper.Map<PersonModel>(person);

                    billingAddress.BillToEmail = personModel.Emails.GetPrimaryEmail();
                    billingAddress.BillToName = $"{personModel.Prefix} {personModel.FirstName} {personModel.LastName}";

                    var primaryAddress = personModel.Addresses.GetPrimaryAddress();
                    if (primaryAddress != null)
                    {
                        billingAddress.StreetAddress = primaryAddress.StreetAddress;
                        billingAddress.City = primaryAddress.City;
                        billingAddress.State = primaryAddress.State;
                        billingAddress.Zip = primaryAddress.Zip.FormatZip();
                    }
                    else
                    {
                        var address = person.Addresses.FirstOrDefault();
                        if (address != null)
                        {
                            billingAddress.StreetAddress = $"{address.Address1} {address.Address2} {address.Address3}";
                            billingAddress.City = address.City;
                            billingAddress.State = address.State;
                            billingAddress.Zip = address.Zip.FormatZip();
                        }
                    }

                }
                else
                {
                    var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);
                    billingAddress.BillToEmail = company.Email;
                    billingAddress.BillToName = company.CompanyName;
                    var companyModel = _mapper.Map<CompanyModel>(company);
                    var primaryAddress = companyModel.Addresses.GetPrimaryAddress();
                    if (primaryAddress != null)
                    {
                        billingAddress.StreetAddress = primaryAddress.StreetAddress;
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
                invoiceModel.BillingAddress = billingAddress;
                var staffUser = await _unitOfWork.Staffusers.GetByIdAsync(invoice.UserId ?? 0);
                if (staffUser != null)
                {
                    invoiceModel.UserName = $"{staffUser.FirstName} {staffUser.LastName}";
                }
            }

            return invoiceModel;
        }

        public async Task<bool> DeletePaperInvoice(int paperInvoiceId)
        {
            Paperinvoice paperInvoice = await _unitOfWork.PaperInvoices.GetByIdAsync(paperInvoiceId);

            if (paperInvoice != null)
            {
                paperInvoice.Status = (int)PaperInvoiceStatus.Deleted;

                _unitOfWork.PaperInvoices.Update(paperInvoice);
                await _unitOfWork.CommitAsync();

                return true;
            }
            return false;
        }

    }
}
