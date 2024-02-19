using AuthorizeNetCore;
using AuthorizeNetCore.PaymentProfileModels;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Max.Core.Constants;

namespace Max.Billing
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TelemetryClient _telemetryClient;
        public IServiceScopeFactory _serviceScopeFactory;
        private readonly AppSettings _appSettings;
        public Worker(IOptions<AppSettings> appSettings, ILogger<Worker> logger,
          TelemetryClient telemetryClient,
          IConfiguration configuration,
          IServiceScopeFactory serviceScopeFactory)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
            _telemetryClient = telemetryClient;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {

            await base.StartAsync(cancellationToken);

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {

            await base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Startup: Loading Tenants");
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var tenantService = scope.ServiceProvider.GetService<ITenantService>();
                //Clear the TenantList 

                TenantManager.Tenants = new List<Tenant>();

                var tenants = await tenantService.GetAllTenants();
                using (IEnumerator<Tenant> tenantEnumerator = tenants.GetEnumerator())
                {
                    while (tenantEnumerator.MoveNext())
                    {
                        Tenant tenant = tenantEnumerator.Current;
                        TenantManager.Tenants.Add(tenant);
                        _logger.LogInformation($"Startup: Added {tenant.OrganizationName} with connection as {tenant.ConnectionString} to Tenants");
                    }
                }
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("Execute Async"))
                {
                    foreach (var tenant in TenantManager.Tenants)
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                        _logger.LogInformation($"Worker Current Tenant: {tenant.OrganizationName}");
                        TenantManager.OrganizationName = tenant.OrganizationName;
                        _logger.LogInformation("Worker: checking if a Job is due");
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var autoBillingService = scope.ServiceProvider.GetService<IAutoBillingService>();
                            var organizationServcie = scope.ServiceProvider.GetService<IOrganizationService>();

                            //Check if there is a job due for processing

                            var jobDue = await autoBillingService.GetNextAutoBillingJob();
                            if (jobDue.AutoBillingJobId > 0)
                            {
                                //Run the job
                                _logger.LogInformation($"Worker:Auto Billing Job is due to run. {jobDue.AutoBillingJobId} Job Type: {jobDue.JobType}");
                                await ProcessJob(jobDue);
                            }
                            var isJobDue = await autoBillingService.IsAutoBillingJobDue();
                            if (isJobDue)
                            {
                                _logger.LogInformation("Worker:Job is due. Create a new one");
                                await CreateJob(isJobDue);
                            }
                            //Check for manual Billing finalization
                            var billingService = scope.ServiceProvider.GetService<IBillingService>();
                            var manualFinalizationJobDue = await billingService.GetNextBillingFinalizationJob();
                            if (manualFinalizationJobDue.BillingJobId > 0)
                            {
                                //Run the job
                                _logger.LogInformation($"Worker:Manual Billing Finalization Job is due to run. {manualFinalizationJobDue.BillingJobId}");
                                await FinalizePaperInvoice(manualFinalizationJobDue);
                                manualFinalizationJobDue.BaseUrl = tenant.BaseUrl;
                                await SendBillingEmailNotifcations(manualFinalizationJobDue);
                            }

                            //Check for manual Billing
                            var manualJobDue = await billingService.GetNextBillingJob();
                            if (manualJobDue.BillingJobId > 0)
                            {
                                //Run the job
                                _logger.LogInformation($"Worker:Manual Billing Job is due to run. {manualJobDue.BillingJobId}");
                                await ProcessPaperInvoice(manualJobDue);
                            }

                            //Check for Renewal finalization
                            var renewalFinalizationJobDue = await billingService.GetNextRenewalFinalizationJob();
                            if (renewalFinalizationJobDue.BillingJobId > 0)
                            {
                                //Run the job
                                _logger.LogInformation($"Worker:Renewal Finalization Job is due to run. {renewalFinalizationJobDue.BillingJobId}");
                                await FinalizePaperInvoice(renewalFinalizationJobDue);
                                renewalFinalizationJobDue.BaseUrl = tenant.BaseUrl;
                                await SendBillingEmailNotifcations(renewalFinalizationJobDue);
                            }

                            //Check for renewal
                            var renewalJobDue = await billingService.GetNextRenewalJob();
                            if (renewalJobDue.BillingJobId > 0)
                            {
                                //Run the job
                                _logger.LogInformation($"Worker:Renewal Job is due to run. {renewalJobDue.BillingJobId}");
                                await ProcessRenewalPaperInvoice(renewalJobDue);
                            }
                        }

                        await Task.Delay(10000, stoppingToken);
                    }


                }
                await Task.Delay(1000, stoppingToken);
            }
        }
        public async Task<bool> CreateJob(Object obj)
        {
            bool jobCreated = false;
            AutoBillingJobModel model = new AutoBillingJobModel();
            _logger.LogInformation("CreateJob.Creating Job");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var autoBillingService = scope.ServiceProvider.GetService<IAutoBillingService>();
                    var job = await autoBillingService.CreatAutoBillingJob();
                    if (job != null)
                    {
                        _logger.LogInformation($"Worker:CreateJob: Job Created {job.AutoBillingJobId}");
                        jobCreated = true;
                    }
                    else
                    {
                        _logger.LogInformation($"Worker:CreateJob: Failed to create job.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Worker.CreateJob: {ex.Message}");
            }
            return jobCreated;
        }
        public async Task<bool> ProcessJob(Object newJob)
        {
            AutoBillingJobModel model = new AutoBillingJobModel();
            var job = (AutoBillingJobModel)newJob;
            decimal totalDue = 0;
            decimal approved = 0;
            decimal declined = 0;
            bool jobProcessed = false;

            _logger.LogInformation($"ProcessJob.Processing Job {job.AutoBillingJobId} -> {job.BillingType} : {job.InvoiceType} - {job.ThroughDate}");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var autobillingService = scope.ServiceProvider.GetService<IAutoBillingService>();
                    var invoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                    var billingDocumentService = scope.ServiceProvider.GetService<IBillingDocumentsService>();
                    var autoBillingDraftService = scope.ServiceProvider.GetService<IAutoBillingDraftService>();
                    var authNetDraftService = scope.ServiceProvider.GetService<IAuthNetDraftService>();
                    var receiptService = scope.ServiceProvider.GetService<IReceiptHeaderService>();
                    var paymentTransactionService = scope.ServiceProvider.GetService<IPaymentTransactionService>();
                    var transactionService = scope.ServiceProvider.GetService<ITransactionService>();
                    var membershipService = scope.ServiceProvider.GetService<IMembershipService>();

                    _logger.LogInformation($"Worker:ProcessJob: Updating Job status to Running.");

                    await autobillingService.UpdateJobStatus(job.AutoBillingJobId, (int)BillingJobStatus.Running);

                    //Create Billing Document
                    _logger.LogInformation($"Worker:ProcessJob: creating billing document");

                    BillingDocumentModel documentModel = new BillingDocumentModel
                    {
                        BillingType = job.BillingType,
                        InvoiceType = job.InvoiceType,
                        ThroughDate = job.ThroughDate,
                        AbpdId = job.AbpdId,
                        IsFinalized = job.JobType == AutoBillingDraftType.Finalized ? 1 : 0
                    };

                    autobillingService = scope.ServiceProvider.GetService<IAutoBillingService>();
                    var document = await billingDocumentService.CreateAutoBillingDocument(documentModel);

                    _logger.LogInformation($"Worker:ProcessJob: Checking Dues");
                    var dues = await membershipService.GetAllMembershipDueByThroughDate((int)BillingType.Auto, job.ThroughDate);
                    if (dues != null)
                    {
                        _logger.LogInformation($"Worker:ProcessJob: creating invoices");
                        foreach (var dueItem in dues)
                        {
                            _logger.LogInformation($"Worker:ProcessJob: Creating invoice for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}.");
                            var invoice = await invoiceService.CreateMembershipBillingInvoice(dueItem.MembershipId, InvoiceType.CREDITCARD, dueItem.NextBillDate);
                            _logger.LogInformation($"Worker:ProcessJob: Creating Draft for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}.");
                            if (invoice.InvoiceId > 0)
                            {
                                var draft = await autoBillingDraftService.CreateAutoBillingCreditCardDraft(document, invoice);

                            }
                        }
                        _logger.LogInformation($"Worker:ProcessJob: Setting Billing document status to Generated.");
                        await billingDocumentService.UpdateBillingDocumentStatus(document.BillingDocumentId, (int)BillingStatus.Generated);
                        _logger.LogInformation($"Worker:ProcessJob: Checking if Job Type is finalized.");
                        if (job.JobType == AutoBillingDraftType.Finalized)
                        {
                            //Get ProcessorDetails
                            _logger.LogInformation($"Worker:ProcessJob: Draft is due for Finalization.");
                            var processorService = scope.ServiceProvider.GetService<IPaymentProcessorService>();

                            var processor = await processorService.GetPaymentProcessorByOrganizationId(2);

                            //Get AutoBilling Draft records
                            var draft = await autoBillingDraftService.GetAutobillingDraftsByBillingDocumentId(document.BillingDocumentId);
                            foreach (var item in draft)
                            {
                                _logger.LogInformation($"Worker:ProcessJob: Processing Payment for  for Person: {item.EntityId} - Membership:{item.MembershipId}.");

                                MaxPaymentProfileTransaction authNetPaymentTransaction = new MaxPaymentProfileTransaction(processor.TestUrl,
                                                                                                                    processor.LoginId,
                                                                                                                    processor.TransactionKey);
                                //Create Receipt

                                _logger.LogInformation($"Worker:ProcessJob: Creating Receipt for Person: {item.EntityId} - Membership:{item.MembershipId} - Due Date: {item.NextDueDate}.");
                                var invoice = await invoiceService.GetInvoiceById(item.InvoiceId);
                                var draftItem = await autoBillingDraftService.GetAutobillingDraftById(item.AutoBillingDraftId);
                                var receipt = await receiptService.CreateDratfReceipt(invoice, draftItem);

                                //Process Payment

                                authNetPaymentTransaction.ReceiptId = receipt.Receiptid;
                                authNetPaymentTransaction.AutoBillingDraftId = item.AutoBillingDraftId;
                                authNetPaymentTransaction.ProfileId = item.ProfileId;
                                authNetPaymentTransaction.PaymentProfileId = item.PaymentProfileId;
                                authNetPaymentTransaction.Amount = item.Amount.ToString();

                                totalDue += item.Amount;

                                authNetPaymentTransaction.InvoiceNumber = invoice.InvoiceId.ToString();
                                authNetPaymentTransaction.InvoiceDescription = BillingTypes.MEMBERSHIP;

                                //Add Line Items

                                LineItems lineItems = new LineItems();
                                lineItems.LineItem = new LineItem[invoice.InvoiceDetails.Count];
                                int i = 0;
                                foreach (var invoiceItem in invoice.InvoiceDetails)
                                {
                                    LineItem lineItem = new LineItem();

                                    lineItem.ItemId = invoiceItem.InvoiceDetailId.ToString();
                                    lineItem.Name = invoiceItem.Description;
                                    lineItem.Quantity = invoiceItem.Quantity.ToString();
                                    lineItem.UnitPrice = invoiceItem.Price.ToString();
                                    lineItems.LineItem[i++] = lineItem;
                                }
                                authNetPaymentTransaction.LineItems = lineItems;

                                // Create PaymentRecord
                                PaymentTransactionModel paymentTransaction = new PaymentTransactionModel();
                                paymentTransaction.TransactionDate = DateTime.Now;
                                paymentTransaction.ReceiptId = receipt.Receiptid;
                                paymentTransaction.EntityId = item.EntityId;
                                paymentTransaction.Status = (int)PaymentTransactionStatus.Created;
                                paymentTransaction.Amount = item.Amount;
                                paymentTransaction.AutoBillingDraftId = item.AutoBillingDraftId;
                                paymentTransaction.PaymentType = PaymentType.CREDITCARD;

                                //process Payment
                                var request = authNetPaymentTransaction.CreatePaymentRequest();
                                _logger.LogInformation($"Worker:ProcessJob:TransactionRequest:" + request);

                                var response = await authNetPaymentTransaction.ProcessPayment();
                                _logger.LogInformation($"Worker:ProcessJob: Response for Person: {item.EntityId} - Membership:{item.MembershipId} - ReceiptId: {receipt.Receiptid}:{response} ");


                                //dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                                if (response != null)
                                {
                                    var txnResponse = response.TransactionResponse;
                                    if (txnResponse.ResponseCode == "1")
                                    {
                                        paymentTransaction.Status = (int)PaymentTransactionStatus.Approved;
                                        paymentTransaction.TransactionId = txnResponse.TransId;
                                        paymentTransaction.ResponseDetails = JsonConvert.SerializeObject(txnResponse);
                                        paymentTransaction.MessageDetails = JsonConvert.SerializeObject(txnResponse.Messages);
                                        paymentTransaction.ResponseCode = txnResponse.ResponseCode;
                                        paymentTransaction.AuthCode = txnResponse.AuthCode;
                                        paymentTransaction.AccountNumber = txnResponse.AccountNumber;
                                        paymentTransaction.CardType = txnResponse.AccountType;
                                        paymentTransaction.Result = (int)ReceiptStatus.Active;
                                        approved += item.Amount;
                                    }
                                    else
                                    {
                                        paymentTransaction.Status = (int)PaymentTransactionStatus.Declined;
                                        paymentTransaction.Result = (int)ReceiptStatus.Created;
                                        paymentTransaction.MessageDetails = JsonConvert.SerializeObject(txnResponse.Messages);
                                        if (txnResponse != null)
                                        {
                                            paymentTransaction.ResponseDetails = JsonConvert.SerializeObject(txnResponse);
                                        }
                                        declined += item.Amount;
                                    }
                                }
                                else
                                {
                                    paymentTransaction.Status = (int)PaymentTransactionStatus.NoResponse;
                                    paymentTransaction.Result = (int)ReceiptStatus.Created;
                                    paymentTransaction.MessageDetails = JsonConvert.SerializeObject(response.Messages);
                                }
                                _logger.LogInformation($"Worker:ProcessJob: Updating Payment status for Person: {item.EntityId} - Membership:{item.MembershipId} - ReceiptId: {receipt.Receiptid}.");

                                //Update PaymentTransaction with Response
                                var paymentResult = await paymentTransactionService.CreatePaymentTransaction(paymentTransaction);
                                paymentTransaction.PaymentTransactionId = paymentResult.PaymentTransactionId;

                                //Create GL Entries
                                await transactionService.UpdateTransactionStatus(paymentTransaction);

                                _logger.LogInformation($"Worker:ProcessJob: Process completed for Person: {item.EntityId} - Membership:{item.MembershipId} - ReceiptId: {receipt.Receiptid}.");
                            }
                            //Update Doc Status to finalized

                            await billingDocumentService.UpdateBillingDocumentStatus(document.BillingDocumentId, (int)BillingStatus.Finalized);

                            //Send notification
                            _logger.LogInformation($"Worker:ProcessJob: Sending notification.");
                            var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                            var notificationModel = new AutoBillingEmailNotificationModel();
                            notificationModel.Subject = $"Recurring billing service completed processing at {DateTime.Now}";
                            notificationModel.TotalDue = totalDue;
                            notificationModel.Approved = approved;
                            notificationModel.Declined = declined;
                            await notificationService.SendAutoBillingNotification(notificationModel);
                            _logger.LogInformation($"Worker:ProcessJob: Notification Sent.");
                        }
                        await autobillingService.UpdateJobStatus(job.AutoBillingJobId, (int)BillingJobStatus.Completed);
                    }
                    else
                    {
                        _logger.LogInformation($"Worker:ProcessJob: No membership Dues.");
                    }
                }
                jobProcessed = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Worker.ProcessJob: {ex.Message}");
            }
            return jobProcessed;
        }

        public async Task<bool> ProcessPaperInvoice(Object paperInvoiceJob)
        {
            BillingCycleModel model = new BillingCycleModel();
            var job = (BillingJobModel)paperInvoiceJob;
            bool processed = false;

            _logger.LogInformation($"ProcessJob.ProcessPaperInvoice Job {job.BillingJobId} -> {job.BillingCycleId}");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var billingService = scope.ServiceProvider.GetService<IBillingService>();
                    var paperInvoiceService = scope.ServiceProvider.GetService<IPaperInvoiceService>();
                    var invoiceService = scope.ServiceProvider.GetService<IInvoiceService>();

                    _logger.LogInformation($"Worker:ProcessPaperInvoice: Updating Job status to Running.");

                    await billingService.UpdateJobStatus(job.BillingJobId, (int)BillingJobStatus.Running);
                    var cycle = await billingService.GetBillingCycleById(job.BillingCycleId);

                    foreach (var batch in cycle.Billingbatches)
                    {
                        var membershipService = scope.ServiceProvider.GetService<IMembershipService>();
                        var dues = await membershipService.GetMembershipDuesByMembershipTypeAndThroughDate(batch.MembershipTypeId, cycle.ThroughDate);
                        if (dues != null)
                        {
                            _logger.LogInformation($"Worker:ProcessPaperInvoice: creating invoices");
                            foreach (var dueItem in dues.Where(x => x.AutoPayEnabled == (int)Status.InActive))
                            {
                                _logger.LogInformation($"Worker:ProcessPaperInvoice: Creating invoice for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}.");
                                var invoice = await invoiceService.CreateMembershipBillingInvoice(dueItem.MembershipId, InvoiceType.PAPER, dueItem.NextBillDate, cycle.BillingCycleId);
                                if (invoice.InvoiceId > 0)
                                {
                                    _logger.LogInformation($"Worker:ProcessPaperInvoice: Creating PaperInvoice for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}.");
                                    var paperInvoice = new PaperInvoiceModel
                                    {
                                        PaperBillingCycleId = cycle.BillingCycleId,
                                        EntityId = invoice.BillableEntityId ?? 0,
                                        Amount = invoice.Amount,
                                        InvoiceId = invoice.InvoiceId,
                                        DueDate = invoice.DueDate,
                                        Description = "Membership Dues"
                                    };
                                    await paperInvoiceService.CreatePaperInvoice(paperInvoice);
                                }
                                else
                                {
                                    _logger.LogInformation($"Worker:ProcessPaperInvoice: No invoice details for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}. Creation Skipped");
                                }

                            }
                            _logger.LogInformation($"Worker:ProcessPaperInvoice: Update Billing Cycle Status {(int)BillingStatus.Generated}");
                            //Update Cycle status
                            await billingService.UpdateCycleStatus(job.BillingCycleId, (int)BillingStatus.Generated);
                        }
                        else
                        {
                            _logger.LogInformation($"Worker:ProcessPaperInvoice: No membership Dues.");
                        }
                    }

                }
                processed = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Worker.ProcessPaperInvoice: {ex.Message}");
            }
            return processed;
        }
        public async Task<bool> ProcessRenewalPaperInvoice(Object paperInvoiceJob)
        {
            BillingCycleModel model = new BillingCycleModel();
            var job = (BillingJobModel)paperInvoiceJob;
            bool processed = false;

            _logger.LogInformation($"ProcessJob.ProcessPaperInvoice Job {job.BillingJobId} -> {job.BillingCycleId}");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var billingService = scope.ServiceProvider.GetService<IBillingService>();
                    var paperInvoiceService = scope.ServiceProvider.GetService<IPaperInvoiceService>();
                    var invoiceService = scope.ServiceProvider.GetService<IInvoiceService>();

                    _logger.LogInformation($"Worker:ProcessRenewalPaperInvoice: Updating Job status to Running.");

                    await billingService.UpdateJobStatus(job.BillingJobId, (int)BillingJobStatus.Running);
                    var cycle = await billingService.GetBillingCycleById(job.BillingCycleId);

                    foreach (var batch in cycle.Billingbatches)
                    {
                        var membershipService = scope.ServiceProvider.GetService<IMembershipService>();
                        var dues = await membershipService.GetMembershipRenewalsDuesByMembershipTypeAsync(batch.MembershipTypeId, cycle.ThroughDate);
                        if (dues != null)
                        {
                            _logger.LogInformation($"Worker:ProcessRenewalPaperInvoice: creating invoices");
                            foreach (var dueItem in dues.Where(x => x.AutoPayEnabled == (int)Status.InActive))
                            {
                                _logger.LogInformation($"Worker:ProcessRenewalPaperInvoice: Creating invoice for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}.");
                                var invoice = await invoiceService.CreateMembershipBillingInvoice(dueItem.MembershipId, InvoiceType.PAPER, dueItem.NextBillDate, cycle.BillingCycleId);
                                if (invoice.InvoiceId > 0)
                                {
                                    _logger.LogInformation($"Worker:ProcessRenewalPaperInvoice: Creating PaperInvoice for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}.");
                                    var paperInvoice = new PaperInvoiceModel
                                    {
                                        PaperBillingCycleId = cycle.BillingCycleId,
                                        EntityId = invoice.BillableEntityId ?? 0,
                                        Amount = invoice.Amount,
                                        InvoiceId = invoice.InvoiceId,
                                        DueDate = invoice.DueDate,
                                        Description = "Membership Renewal"
                                    };
                                    await paperInvoiceService.CreatePaperInvoice(paperInvoice);
                                }
                                else
                                {
                                    _logger.LogInformation($"Worker:ProcessPaperInvoice: No invoice details for Person: {dueItem.BillableEntity.EntityId} - Membership:{dueItem.MembershipId} - Due Date: {dueItem.NextBillDate}. Creation Skipped");
                                }

                            }
                            _logger.LogInformation($"Worker:ProcessPaperInvoice: Update Billing Cycle Status {(int)BillingStatus.Generated}");
                            //Update Cycle status
                            await billingService.UpdateCycleStatus(job.BillingCycleId, (int)BillingStatus.Generated);
                        }
                        else
                        {
                            _logger.LogInformation($"Worker:ProcessPaperInvoice: No membership Dues.");
                        }
                    }

                }
                processed = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Worker.ProcessRenewalPaperInvoice: {ex.Message}");
            }
            return processed;
        }
        public async Task<bool> FinalizePaperInvoice(Object finalizePaperInvoiceJob)
        {
            BillingCycleModel model = new BillingCycleModel();
            var job = (BillingJobModel)finalizePaperInvoiceJob;
            bool finalized = false;

            _logger.LogInformation($"Worker.FinalizePaperInvoice Job {job.BillingJobId} -> {job.BillingCycleId}");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var paperInvoiceService = scope.ServiceProvider.GetService<IPaperInvoiceService>();
                    var billingService = scope.ServiceProvider.GetService<IBillingService>();

                    var result = await paperInvoiceService.FinalizePaperInvoicesByCycleId(job.BillingCycleId);
                    if (result)
                    {
                        _logger.LogInformation($"Worker:FinalizePaperInvoice: Invoice finalized");
                        //Update Cycle status
                        _logger.LogInformation($"Worker:FinalizePaperInvoice: Update cycle status {(int)BillingStatus.Finalized}");
                        await billingService.UpdateCycleStatus(job.BillingCycleId, (int)BillingStatus.Finalized);
                        //Update Job status
                        _logger.LogInformation($"Worker:FinalizePaperInvoice: Update Job status {(int)BillingJobStatus.Completed}");
                        await billingService.UpdateJobStatus(job.BillingJobId, (int)BillingJobStatus.Completed);

                    }
                    else
                    {
                        _logger.LogInformation($"Worker:ProcessJob: No membership Dues.");
                    }
                }
                finalized = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Worker.FinalizePaperInvoice: {ex.Message}");
            }
            return finalized;
        }
        public async Task<bool> SendBillingEmailNotifcations(Object finalizePaperInvoiceJob)
        {
            BillingCycleModel model = new BillingCycleModel();
            var job = (BillingJobModel)finalizePaperInvoiceJob;
            var sent = false;
            _logger.LogInformation($"Worker.SendBillingEmailNotifcations Job {job.BillingJobId} -> {job.BillingCycleId} Base URL: {job.BaseUrl}");
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var billingService = scope.ServiceProvider.GetService<IBillingService>();
                    var emailService = scope.ServiceProvider.GetService<IEmailService>();
                    var billingEmails = await billingService.GetEmailsForBillingCycle(job.BillingCycleId);
                    foreach (var email in billingEmails.Where(x => x.Status == (int)EmailStatus.Pending))
                    {
                        var emailModel = await billingService.GetEmailNotificationDetailById(email.BillingEmailId);
                        emailModel.BaseUrl = job.BaseUrl;
                        if (!emailModel.EmailAddress.IsNullOrEmpty())
                        {
                            try
                            {
                                var result = await emailService.SendBatchInvoiceNotification(emailModel);
                                if (result)
                                {
                                    await billingService.UpdateBillingEmailStatus(email.BillingEmailId, "Sent", result);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation($"Worker.SendBillingEmailNotifcations: {ex.Message}");
                                await billingService.UpdateBillingEmailStatus(email.BillingEmailId, ex.Message, false);
                            }
                        }
                    }
                }
                sent = true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Worker.SendBillingEmailNotifcations: {ex.Message}");
            }
            return sent;
        }

        public override void Dispose()
        {
            // Dispose objects
        }
    }
}