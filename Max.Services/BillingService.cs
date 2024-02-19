using AutoMapper;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Max.Services
{

    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        static readonly ILogger _logger = Serilog.Log.ForContext<BillingService>();
        private readonly IMapper _mapper;
        public BillingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<bool> IsBillingJobDue()
        {

            _logger.Information("Checking Job Due Status");

            var job = await _unitOfWork.BillingJobs.GetBillingJobByDateAsync(DateTime.Now);
            if (job != null)
            {
                _logger.Information($"Billing Job is already created.");
                return false;
            }
            return true;
        }

        public async Task<Billingcycle> GetBillingCycleById(int id)
        {
            return await _unitOfWork.BillingCycles.GetBillingCycleByIdAsync(id);
        }

        public async Task<List<BillingCycleModel>> GetBillingCycles(int cycleType = 0)
        {
            var billingCycles = await _unitOfWork.BillingCycles.GetAllBillingCyclesAsync();
            var billingBatches = await _unitOfWork.BillingBatches.GetAllBillingBatchesAsync();
            var periodList = await _unitOfWork.MembershipPeriods.GetAllAsync();

            return billingCycles.Where(x => x.CycleType == cycleType).Select(x => new BillingCycleModel()
            {
                BillingCycleId = x.BillingCycleId,
                CycleName = x.CycleName,
                RunDate = x.RunDate,
                ThroughDate = x.ThroughDate,
                TotalAmount = x.Paperinvoices.Select(x => x.Invoice.Invoicedetails.Sum(x => x.Price)).Sum(),
                InvoiceCount = x.Paperinvoices.Count(),
                Status = x.Status,
                BillingBatches = billingBatches.Where(b => b.BatchCycleId == x.BillingCycleId).Select(y => new BillingBatchModel()
                {
                    BillingCycleId = x.BillingCycleId,
                    BillingBatchId = y.BillingBatchId,
                    MembershipType = y.MembershipType.Name,
                    Category = y.MembershipType.CategoryNavigation.Name,
                    Period = y.MembershipType.PeriodNavigation.Name,
                    Frequency = periodList.Where(z => z.MembershipPeriodId == y.MembershipType.PaymentFrequency).Select(z => z.Name).FirstOrDefault()
                }).ToList()
            }).ToList();
        }
        public async Task<Billingjob> CreateBillingJob(int cycleId)
        {

            Billingjob model = new Billingjob();
            model.Status = (int)BillingJobStatus.Created;
            model.CreateDate = DateTime.Now;
            model.BillingCycleId = cycleId;
            try
            {
                await _unitOfWork.BillingJobs.AddAsync(model);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Billing Job creation failed:{ex.Message}");
            }

            return model;

        }

        public async Task<Billingcycle> CreateBillingCycle(BillingCycleModel cycleModel)
        {

            Billingcycle model = new Billingcycle();

            //Check for duplicate batchname

            var billingCycle = await _unitOfWork.BillingCycles.GetBillingCycleByNameAsync(cycleModel.CycleName);

            if (billingCycle != null)
            {
                throw new InvalidOperationException("This batch name is already in use. Please use uniqueue batch name.");
            }

            //Check if Billing Cycle has overlapping sequence
            var existingCycles = await _unitOfWork.BillingCycles.GetPendingCyclesAsync(cycleModel.CycleType);

            foreach (var batch in existingCycles.Select(x => x.Billingbatches))
            {
                foreach (var type in cycleModel.MembershipType)
                {
                    if (batch.Any(x => x.MembershipTypeId == Convert.ToInt32(type)))
                    {
                        throw new InvalidOperationException("A batch already exist with same category. Please delete it or finalize first.");
                    }
                }
            }

            model.Status = (int)BillingStatus.Created;
            model.CycleName = cycleModel.CycleName;
            model.RunDate = DateTime.Now;
            model.ThroughDate = cycleModel.ThroughDate;
            model.CycleType = cycleModel.CycleType;

            //Add batches for each Membership Type
            foreach (var type in cycleModel.MembershipType)
            {
                Billingbatch batch = new Billingbatch();
                batch.MembershipTypeId = Convert.ToInt32(type);
                batch.Status = (int)BillingStatus.Created;
                model.Billingbatches.Add(batch);
            }

            try
            {
                await _unitOfWork.BillingCycles.AddAsync(model);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"Billing creation failed:{ex.Message}");
            }
            return model;
        }
        public async Task<bool> FinzalizeBillingCycle(int cycleId)
        {

            var billingCycle = await _unitOfWork.BillingCycles.GetBillingCycleByIdAsync(cycleId);

            if (billingCycle.Status == (int)BillingStatus.Generated)
            {
                //Set billing job start Date to now. The billing service will pick up the job in next cycle
                var job = await _unitOfWork.BillingJobs.GetBillingJobByCycleIdAsync(cycleId);
                if (job.Status == (int)BillingJobStatus.Running && job.StartDate.Date == Constants.MySQL_MinDate.Date)
                {
                    billingCycle.Status = (int)BillingStatus.Finalizing;
                    job.StartDate = DateTime.Now;
                    try
                    {
                        _unitOfWork.BillingCycles.Update(billingCycle);
                        _unitOfWork.BillingJobs.Update(job);
                        await _unitOfWork.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Billing creation failed:{ex.Message}");
                        throw new Exception("An error has occured. Please try later.");
                    }
                }
                else
                {
                    throw new Exception("Can not finalize this job. It is either running or has already been finzalized");
                }
            }
            else
            {
                throw new Exception("Can not finalize this job. It is either running or has already been finzalized");
            }
        }
        public async Task<BillingJobModel> GetNextBillingJob()
        {
            try
            {
                var job = await _unitOfWork.BillingJobs.GetNextBillingJobAsync();

                if (job != null)
                {
                    return _mapper.Map<BillingJobModel>(job);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GetNextBillingJob failed:{ex.Message}");
            }
            return new BillingJobModel();
        }
        public async Task<BillingJobModel> GetNextRenewalJob()
        {
            try
            {
                var job = await _unitOfWork.BillingJobs.GetNextRenewalJobAsync();

                if (job != null)
                {
                    return _mapper.Map<BillingJobModel>(job);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GetNextRenewalJob failed:{ex.Message}");
            }
            return new BillingJobModel();
        }
        public async Task<BillingJobModel> GetNextBillingFinalizationJob()
        {
            try
            {
                var job = await _unitOfWork.BillingJobs.GetBillingFinalizationJobByDateAsync(DateTime.Now);

                if (job != null)
                {
                    return _mapper.Map<BillingJobModel>(job);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GetNextBillingFinalizationJob failed:{ex.Message}");
            }

            return new BillingJobModel();
        }

        public async Task<BillingJobModel> GetNextRenewalFinalizationJob()
        {
            try
            {
                var job = await _unitOfWork.BillingJobs.GetRenewalFinalizationJobByDateAsync(DateTime.Now);

                if (job != null)
                {
                    return _mapper.Map<BillingJobModel>(job);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"GetNextRenewalFinalizationJob failed:{ex.Message}");
            }

            return new BillingJobModel();
        }
        public async Task<bool> UpdateJobStatus(int jobId, int status)
        {
            if (jobId <= 0)
            {
                throw new InvalidOperationException($"Invalid Job Id.");
            }
            var job = await _unitOfWork.BillingJobs.GetByIdAsync(jobId);
            if (job == null)
            {
                throw new NullReferenceException($"Job not found.");
            }
            job.Status = status;
            job.EndDate = DateTime.Now;
            _unitOfWork.BillingJobs.Update(job);
            await _unitOfWork.CommitAsync();

            return true;
        }
        public async Task<bool> UpdateCycleStatus(int cycleId, int status)
        {
            if (cycleId <= 0)
            {
                throw new InvalidOperationException($"Invalid Cycle Id.");
            }
            var cycle = await _unitOfWork.BillingCycles.GetByIdAsync(cycleId);
            if (cycle == null)
            {
                throw new NullReferenceException($"Cycle not found.");
            }
            cycle.Status = status;

            _unitOfWork.BillingCycles.Update(cycle);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteBillingCycle(int cycleId)
        {
            if (cycleId <= 0)
            {
                throw new InvalidOperationException($"Invalid Cycle Id.");
            }
            // Delete Invoices
            var paperInvoices = await _unitOfWork.PaperInvoices.GetAllPaperInvoicesByCycleId(cycleId);
            foreach (var pinvoice in paperInvoices)
            {
                //Get related invoice
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(pinvoice.InvoiceId);

                //Remove Invoice & children [InvoiceDetils has cascade on Delete]

                _unitOfWork.Invoices.Remove(invoice);
                _unitOfWork.PaperInvoices.Remove(pinvoice);
            }
            //Delete Job
            var job = await _unitOfWork.BillingJobs.GetBillingJobByCycleIdAsync(cycleId);
            _unitOfWork.BillingJobs.Remove(job);

            var cycle = await _unitOfWork.BillingCycles.GetByIdAsync(cycleId);

            var batches = await _unitOfWork.BillingBatches.GetAllBillingBatchesByCycleIdAsync(cycleId);

            batches.ToList().ForEach(b =>
            {
                b.MembershipType = null;
                b.BatchCycle = null;
            });
            //Delete Batches 
            _unitOfWork.BillingBatches.RemoveRange(batches);

            //Delet Billing cycle notification
            var notification = await _unitOfWork.BillingCycleNotifications.GetNotificationByBillingCycleIdAsync(cycleId);
            if (notification != null)
            {
                _unitOfWork.BillingCycleNotifications.Remove(notification);
            }
            //Delete cycle
            _unitOfWork.BillingCycles.Remove(cycle);
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteBillingCycle: Failed to delete Cycle:{cycleId} failed with error {ex.Message} {ex.StackTrace}");
            }

            return true;
        }
        public async Task<bool> RegenrateBillingCycle(int cycleId)
        {
            if (cycleId <= 0)
            {
                throw new InvalidOperationException($"Invalid Cycle Id.");
            }
            // Delete Invoices
            var paperInvoices = await _unitOfWork.PaperInvoices.GetPaperInvoicesByCycleId(cycleId);
            foreach (var pinvoice in paperInvoices)
            {
                if (pinvoice.Status == (int)PaperInvoiceStatus.Inactive)
                {
                    //Get related invoice
                    var invoice = await _unitOfWork.Invoices.GetByIdAsync(pinvoice.InvoiceId);

                    //Remove Invoice & children [InvoiceDetils has cascade on Delete]
                    if (pinvoice.Status == (int)PaperInvoiceStatus.Inactive)
                    {
                        _unitOfWork.Invoices.Remove(invoice);
                        _unitOfWork.PaperInvoices.Remove(pinvoice);
                    }
                }
            }
            //Update Job Status
            var job = await _unitOfWork.BillingJobs.GetBillingJobByCycleIdAsync(cycleId);
            job.Status = (int)BillingStatus.Created;
            _unitOfWork.BillingJobs.Update(job);

            var cycle = await _unitOfWork.BillingCycles.GetByIdAsync(cycleId);
            cycle.Status = (int)BillingStatus.Created;

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.Error($"RegenrateBillingCycle: Failed to regenrate Cycle:{cycleId} failed with error {ex.Message} {ex.StackTrace}");
            }

            return true;
        }

        public async Task<List<Billingemail>> GetEmailsForBillingCycle(int cycleId)
        {
            var billingEmails = await _unitOfWork.BillingEmails.GetBillingEmailsByCycleIdAsync(cycleId);

            return billingEmails.ToList();
        }

        public async Task<BatchEmailNotificationModel> GetEmailNotificationDetailById(int emailId)
        {
            var billingEmail = await _unitOfWork.BillingEmails.GetBillingEmailByIdAsync(emailId);
            BatchEmailNotificationModel emailModel = new BatchEmailNotificationModel();
            if (billingEmail.Status == (int)EmailStatus.Pending)
            {
                // Get Billable Entity email Address
                var invoice = await _unitOfWork.Invoices.GetInvoiceByIdAsync(billingEmail.InvoiceId);
                var entity = await _unitOfWork.Entities.GetEntityByIdAsync(invoice.BillableEntityId ?? 0);
                var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(entity.OrganizationId ?? 0);

                string emailAddress = string.Empty;
                string name = string.Empty;

                if (entity.PersonId != null)
                {
                    var person = await _unitOfWork.Persons.GetEmailsByPersonId(entity.PersonId ?? 0);
                    billingEmail.EmailAddress = person.Emails.Where(x => x.IsPrimary == (int)Status.Active).Select(x => x.EmailAddress).FirstOrDefault();
                    emailModel.Name = person.FirstName + " " + person.LastName;
                    emailModel.EmailAddress = billingEmail.EmailAddress;
                }
                else
                {
                    var company = await _unitOfWork.Companies.GetCompanyByIdAsync(entity.CompanyId ?? 0);
                    if (!string.IsNullOrEmpty(company.Email))
                    {
                        billingEmail.EmailAddress = company.Email;
                    }
                    else
                    {
                        billingEmail.EmailAddress = company.Emails.Where(x => x.IsPrimary == (int)Status.Active).Select(x => x.EmailAddress).FirstOrDefault();
                    }
                    emailModel.Name = company.CompanyName;
                    emailModel.EmailAddress = billingEmail.EmailAddress;
                }

                billingEmail.Token = GenerateEmailToken();
                emailModel.BillingEmailId = billingEmail.BillingEmailId;
                emailModel.Invoice = _mapper.Map<InvoiceModel>(invoice);
                emailModel.InvoiceId = invoice.InvoiceId;
                emailModel.Organization = _mapper.Map<OrganizationModel>(organization);
                emailModel.PaymentUrl = $"organization={emailModel.Organization.Name}&id={Base64UrlEncoder.Encode(billingEmail.Token)}";
                _unitOfWork.BillingEmails.Update(billingEmail);
            }
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update email records.");
            }
            return emailModel;
        }

        public async Task<bool> UpdateBillingEmailStatus(int billingEmailId, string response, bool sent)
        {
            var billingEmail = await _unitOfWork.BillingEmails.GetBillingEmailByIdAsync(billingEmailId);
            if (billingEmail != null)
            {
                billingEmail.Response = response;
                if (sent)
                {
                    billingEmail.Status = (int)EmailStatus.Sent;
                }
                _unitOfWork.BillingEmails.Update(billingEmail);
                try
                {
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to update email records.");
                }

            }

            return true;
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
        public async Task<List<BillingCycleNotifications>> GetBillingNotifications()
        {
            try
            {
                List<BillingCycleNotifications> billingCycleNotifications = new List<BillingCycleNotifications>();
                var billingCycles = await _unitOfWork.BillingCycles.GetAllAsync();
                if (billingCycles.Any())
                {
                    foreach (var item in billingCycles)
                    {
                        var billingNotification = await _unitOfWork.BillingCycleNotifications.GetNotificationByBillingCycleIdAsync(item.BillingCycleId);
                        if (billingNotification == null)
                        {
                            BillingCycleNotification billingCycleNotificationModel = new BillingCycleNotification();
                            if (item.Status == (int)BillingStatus.Created)
                            {
                                billingCycleNotificationModel.NotificationText = "Billing " + item.CycleName + " is created";
                            }
                            else if (item.Status == (int)BillingStatus.Generated)
                            {
                                billingCycleNotificationModel.NotificationText = "Billing " + item.CycleName + " is generated";
                            }
                            else if (item.Status == (int)BillingStatus.Finalized)
                            {
                                billingCycleNotificationModel.NotificationText = "Billing " + item.CycleName + " is finalized";
                            }
                            else if (item.Status == (int)BillingStatus.Finalizing)
                            {
                                billingCycleNotificationModel.NotificationText = "Billing " + item.CycleName + " is finalizing";
                            }
                            billingCycleNotificationModel.BillingCycleId = item.BillingCycleId;
                            billingCycleNotificationModel.IsRead = (int)Status.InActive;
                            await _unitOfWork.BillingCycleNotifications.AddAsync(billingCycleNotificationModel);
                            await _unitOfWork.CommitAsync();
                        }
                        else
                        {
                            if (item.Status == (int)BillingStatus.Created)
                            {
                                billingNotification.NotificationText = "Billing " + item.CycleName + " is created";
                            }
                            else if (item.Status == (int)BillingStatus.Generated)
                            {
                                billingNotification.NotificationText = "Billing " + item.CycleName + " is generated";
                            }
                            else if (item.Status == (int)BillingStatus.Finalized)
                            {
                                billingNotification.NotificationText = "Billing " + item.CycleName + " is finalized";
                            }
                            else if (item.Status == (int)BillingStatus.Finalizing)
                            {
                                billingNotification.NotificationText = "Billing " + item.CycleName + " is finalizing";
                            }
                            _unitOfWork.BillingCycleNotifications.Update(billingNotification);
                            await _unitOfWork.CommitAsync();
                        }
                    }
                    billingCycleNotifications = await SetUnReadBillingBotifications();
                }
                return billingCycleNotifications;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<BillingCycleNotifications>> ClearAllBillingNotifications()
        {
            var getAllUnReadBillingNotifications = await _unitOfWork.BillingCycleNotifications.GetAllUnReadBillingNotificationAsync();
            if (getAllUnReadBillingNotifications.Any())
            {
                foreach (var item in getAllUnReadBillingNotifications)
                {
                    var billingCycleNotification = await _unitOfWork.BillingCycleNotifications.GetByIdAsync(item.NotificationId);
                    if (billingCycleNotification != null)
                    {
                        billingCycleNotification.IsRead = (int)Status.Active;
                        _unitOfWork.BillingCycleNotifications.Update(billingCycleNotification);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }
            var billingCycleNotifications = await SetUnReadBillingBotifications();
            return billingCycleNotifications;
        }

        public async Task<List<BillingCycleNotifications>> ClearBillingNotifications(BillingCycleNotifications model)
        {
            var getUnReadBillingNotification = await _unitOfWork.BillingCycleNotifications.GetByIdAsync(model.BillingCycleNotificationId);
            if (getUnReadBillingNotification != null)
            {
                getUnReadBillingNotification.IsRead = (int)Status.Active;
                _unitOfWork.BillingCycleNotifications.Update(getUnReadBillingNotification);
                await _unitOfWork.CommitAsync();
            }
            var billingNotifications = await SetUnReadBillingBotifications();
            return billingNotifications;
        }

        private async Task<List<BillingCycleNotifications>> SetUnReadBillingBotifications()
        {
            List<BillingCycleNotifications> billingCycleNotifications = new List<BillingCycleNotifications>();
            var getAllUnReadBillingNotifications = await _unitOfWork.BillingCycleNotifications.GetAllUnReadBillingNotificationAsync();
            foreach (var item in getAllUnReadBillingNotifications)
            {
                BillingCycleNotifications billingCycleNotification = new BillingCycleNotifications();
                var billingNotification = await _unitOfWork.BillingCycles.GetBillingCycleByIdAsync(item.BillingCycleId);
                if (billingCycleNotification != null)
                {
                    billingCycleNotification.BillingCycleId = item.BillingCycleId;
                    billingCycleNotification.status = billingNotification.Status;
                    billingCycleNotification.CycleName = billingNotification.CycleName;
                    billingCycleNotification.Notification = item.NotificationText;
                    billingCycleNotification.BillingCycleNotificationId = item.NotificationId;
                    billingCycleNotifications.Add(billingCycleNotification);
                }
            }
            return billingCycleNotifications;
        }

    }
}