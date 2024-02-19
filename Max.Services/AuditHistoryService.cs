using Max.Core;
using Max.Core.Models;
using Max.Data.Audit;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Services
{
    public class AuditHistoryService : IAuditHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;
        public AuditHistoryService(IUnitOfWork unitOfWork, ILogger<InvoiceService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
        }
        public async Task<List<AuditHistoryModel>> GetAuditHistoryById(string primaryKey)
        {
            var Invoice = await _unitOfWork.Invoices.GetInvoiceSummaryByIdAsync(int.Parse(primaryKey));
            if (Invoice == null)
            {
                return new List<AuditHistoryModel>();
            }

            List<AuditHistoryModel> result = new List<AuditHistoryModel>();

            var auditHistory = await _unitOfWork.AuditHistory.GetAuditHistoryByIdAsync(primaryKey);
            if (auditHistory != null)
            {
                foreach (var changeItem in auditHistory.AuditChanges)
                {
                    result.AddRange(await PrepareAuditLogTrailAsync(changeItem));
                }
            }
            if (Invoice.Invoicedetails.Any())
            {
                foreach (var invoicedetail in Invoice.Invoicedetails)
                {
                    var invoiceHistory = await _unitOfWork.AuditHistory.GetAuditHistoryByIdAsync(invoicedetail.InvoiceDetailId.ToString());

                    string additionalText = $" From Invoice # {invoicedetail.InvoiceId}-{invoicedetail.InvoiceDetailId} ,Description :{invoicedetail.Description}";
                    foreach (var changeItem in invoiceHistory.AuditChanges)
                    {
                        result.AddRange(await PrepareAuditLogTrailAsync(changeItem, skipAddedEntityState: true, additionalText: additionalText));
                    }
                    foreach (var writeoff in invoicedetail.Writeoffs)
                    {
                        var writeOffHistory = await _unitOfWork.AuditHistory.GetAuditHistoryByIdAsync(writeoff.WriteOffId.ToString());
                        WriteOffAuditLoagHistory(result, writeOffHistory.AuditChanges.ToList());
                    }
                }
            }
            if (Invoice.MembershipId != null)
            {
                var membership = await _unitOfWork.Memberships.GetMembershipByIdAsync((int)Invoice.MembershipId);
                if (membership != null)
                {
                    foreach (var billingfee in membership.Billingfees)
                    {
                        var entityAdded = await _unitOfWork.AuditHistory.GetModifiedAuditHistoryByIdAsync(billingfee.BillingFeeId.ToString());
                        if (entityAdded != null && entityAdded.Any())
                        {
                            foreach (var changeItem in entityAdded)
                            {
                                result.AddRange(await PrepareAuditLogTrailAsync(changeItem, skipAddedEntityState: true));
                            }
                        }
                    }
                }
            }
            if (Invoice.Paperinvoices != null)
            {
                foreach (var paperinvoice in Invoice.Paperinvoices)
                {
                    var entityAdded = await _unitOfWork.AuditHistory.GetAddedAuditHistoryByIdAsync(paperinvoice.PaperInvoiceId.ToString());
                    if (entityAdded != null && !entityAdded.Any())
                    {
                        foreach (var changeItem in entityAdded)
                        {
                            result.AddRange(await PrepareAuditLogTrailAsync(changeItem, skipAddedEntityState: true));
                        }
                    }
                }
            }

            var deletedAuditHistory = await _unitOfWork.AuditHistory.GetDeletedAuditHistoryByIdAsync(primaryKey);
            if (deletedAuditHistory != null)
            {
                foreach (var changeItem in deletedAuditHistory)
                {
                    string auditSummary = $"{changeItem.DateTimeOffset.ToUniversalTime().ToString("ddd, dd MMM yyyy hh:mm tt 'GMT'")} : {changeItem.EntityState.ToString()} by {changeItem.ByUser}";

                    var left = JsonConvert.DeserializeObject<ExpandoObject>(changeItem.OldValues == null ? "" : changeItem.OldValues?.ToString());
                    var right = JsonConvert.DeserializeObject<ExpandoObject>(changeItem.NewValues == null ? "" : changeItem.NewValues.ToString());
                    var comparer = new Comparer();
                    IEnumerable<Difference> jsonDiffData;
                    var isEqual = comparer.Compare(left, right, out jsonDiffData);
                    if (jsonDiffData != null)
                    {
                        List<AuditChangesModel> auditChangesModels = new List<AuditChangesModel>();
                        foreach (var keyItem in jsonDiffData)
                        {
                            if (changeItem.EntityState == EntityState.Deleted)
                            {
                                if (keyItem.MemberPath.Contains("Description"))
                                {
                                    auditChangesModels.Add(new AuditChangesModel { AuditChangesText = $"Deleted {keyItem.MemberPath}->{keyItem.Value1}" });
                                }
                            }
                        }
                        result.Add(new AuditHistoryModel { AuditSummary = auditSummary, DisplayName = string.Empty, EntityStatus = changeItem.EntityState.ToString(), PrimaryKey = primaryKey, AuditChanges = auditChangesModels });
                    }

                }
            }

            return result.OrderBy(x => x.DateTimeOffSet).ToList();
        }

        private static void WriteOffAuditLoagHistory(List<AuditHistoryModel> result, List<AuditEntity> entityAdded)
        {
            List<AuditChangesModel> auditChangesModels = new List<AuditChangesModel>();
            foreach (var changeItem in entityAdded)
            {
                if (changeItem.AuditMetaData.Table == "writeoff")
                {
                    var writeOff = JsonConvert.DeserializeObject<dynamic>(changeItem.NewValues.ToString());
                    if (writeOff != null)
                    {
                        auditChangesModels.Add(new AuditChangesModel { AuditChangesText = $"Added Reason <b>{writeOff?.Reason}</b>" });
                        string auditSummary = $"{changeItem.DateTimeOffset.ToUniversalTime().ToString("ddd, dd MMM yyyy hh:mm tt 'GMT'")} : {changeItem.EntityState.ToString()} by {changeItem.ByUser}";
                        result.Add(new AuditHistoryModel
                        {
                            AuditSummary = auditSummary,
                            DateTimeOffSet = changeItem.DateTimeOffset,
                            DisplayName = changeItem.AuditMetaData.DisplayName,
                            EntityStatus = changeItem.EntityState.ToString(),
                            PrimaryKey = changeItem.AuditMetaData.ReadablePrimaryKey,
                            AuditChanges = auditChangesModels
                        });
                    }
                }
            }
        }

        private async Task<List<AuditHistoryModel>> PrepareAuditLogTrailAsync(AuditEntity changeItem, bool skipAddedEntityState = false, string additionalText = "")
        {
            List<AuditHistoryModel> result = new List<AuditHistoryModel>();


            string auditSummary = $"{changeItem.DateTimeOffset.ToUniversalTime().ToString("ddd, dd MMM yyyy hh:mm tt 'GMT'")} : {changeItem.EntityState.ToString()} by {changeItem.ByUser}";

            var left = JsonConvert.DeserializeObject<ExpandoObject>(changeItem.OldValues == null ? "" : changeItem.OldValues?.ToString());
            var right = JsonConvert.DeserializeObject<ExpandoObject>(changeItem.NewValues == null ? "" : changeItem.NewValues.ToString());
            var comparer = new Comparer();
            IEnumerable<Difference> jsonDiffData;
            var isEqual = comparer.Compare(left, right, out jsonDiffData);
            if (jsonDiffData != null && jsonDiffData.Any())
            {
                //var diffData = jsonDiffData.ToObject<Dictionary<string, dynamic>>();
                List<AuditChangesModel> auditChangesModels = new List<AuditChangesModel>();
                foreach (var keyItem in jsonDiffData)
                {
                    if (changeItem.EntityState == EntityState.Modified)
                    {
                        if (keyItem.MemberPath.Contains("Amount") || keyItem.MemberPath.Contains("Price") || keyItem.MemberPath.Contains("Fee"))
                        {
                            auditChangesModels.Add(new AuditChangesModel { AuditChangesText = $"{keyItem.MemberPath} change from <b>${keyItem.Value1}</b> to <b>${keyItem.Value2}</b> {additionalText}" });
                        }
                        else
                        {
                            string fromValue = FormatValue(keyItem.MemberPath, keyItem.Value1);
                            string toValue = FormatValue(keyItem.MemberPath, keyItem.Value2);
                            auditChangesModels.Add(new AuditChangesModel { AuditChangesText = $"{FormatMemberPath(keyItem.MemberPath)} change from <b>{fromValue}</b> to <b>{toValue}</b>  {additionalText}" });
                        }
                    }
                }
                if (!skipAddedEntityState)
                {
                    result.Add(new AuditHistoryModel { AuditSummary = auditSummary, DateTimeOffSet = changeItem.DateTimeOffset, DisplayName = changeItem.AuditMetaData.DisplayName, EntityStatus = changeItem.EntityState.ToString(), PrimaryKey = changeItem.AuditMetaData.ReadablePrimaryKey, AuditChanges = auditChangesModels });
                }
                else
                {
                    if (changeItem.EntityState != EntityState.Added)
                    {
                        result.Add(new AuditHistoryModel { AuditSummary = auditSummary, DateTimeOffSet = changeItem.DateTimeOffset, DisplayName = changeItem.AuditMetaData.DisplayName, EntityStatus = changeItem.EntityState.ToString(), PrimaryKey = changeItem.AuditMetaData.ReadablePrimaryKey, AuditChanges = auditChangesModels });
                    }
                }

            }

            return result;
        }
        private string FormatMemberPath(string memberPath)
        {
            if (string.IsNullOrEmpty(memberPath))
                return memberPath;
            if (memberPath.Contains("Date"))
            {
                return "Invoice Date";
            }
            return memberPath;
        }
        private string FormatValue(string Key, string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (Key.Contains("DueDate") || Key.Contains("Date"))
            {
                return DateTime.Parse(text).ToString("d");
            }
            if (Key.Contains("Status"))
            {
                return Enum.ToObject(typeof(InvoiceStatus), Convert.ToInt32(text)).ToString();
            }

            return text;
        }
    }
}
