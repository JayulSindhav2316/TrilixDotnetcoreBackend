using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using iTextSharp.text.rtf.field;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;

namespace Max.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ReportModel>> GetAllReports()
        {
            var reports = await _unitOfWork.Reports.GetAllReportsAsync();
            return reports.Select(report => _mapper.Map<ReportModel>(report)).ToList();
        }

        public async Task<ReportModel> GetReportById(int id)
        {
            var report = await _unitOfWork.Reports.GetReportByIdAsync(id);
            return _mapper.Map<ReportModel>(report);
        }

        public async Task<List<ReportFieldModel>> GetReportFieldsByCategoryId(int id)
        {
            var fields = await _unitOfWork.Reports.GetReportFieldsByCategoryId(id);
            return _mapper.Map<List<ReportFieldModel>>(fields);
        }

        public async Task<Report> CreateReport(ReportConfigurationModel model)
        {
            //Delete prvious previewed Reports
            await DeletePreviewModeReport(model.UserId);

            Report report = new Report();
            if (!await ValidReport(model))
            {
                return report;
            }

            //Check if its preview mode exists

            if (model.ReportId > 0 & model.PreviewMode == (int)Status.InActive)
            {
                report = await UpdateReport(model);
                return report;
            }

            if (model.PreviewMode == (int)Status.Active)
            {
                Random random = new Random();
                report.Title = $"{model.Title}-{random.Next(100, 10000)}";
            }
            else
            {
                report.Title = model.Title;
            }

            report.Fields = model.Fields.ToString();
            report.UserId = model.UserId;
            report.Description = model.Description;
            report.PreviewMode = model.PreviewMode;
            report.isCommunity = model.isCommunity;
            report.LastUpdatedOn = model.LastUpdatedOn;
            report.isFavorite = model.isFavorite;
            report.ReportType = model.ReportType;

            if (model.ReportType == "Membership")
            {
                var membership = new Membershipreport();
                membership.Categories = model.Categories.ToString();
                membership.MembershipTypes = model.MembershipTypes.ToString();
                membership.Status = model.Status;
                report.Membershipreports.Add(membership);
            }

            if (model.ReportType == "Opportunities")
            {
                var opportunity = new Opportunityreport();
                opportunity.Pipeline = model.Pipeline;
                opportunity.Stages = model.Stages.ToString();
                opportunity.Products = model.Products.ToString();
                report.Opportunityreports.Add(opportunity);
            }

            if (model.ReportType == "Events")
            {

            }

            if (model.ReportFilters.Count > 0)
            {
                string datePattern = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z";
                foreach (var item in model.ReportFilters)
                {
                    if (item.ReportFieldId > 0)
                    {
                        var filter = new Reportfilter();
                        filter.ReportFieldId = item.ReportFieldId;
                        filter.Operator = item.Operator;
                        //Strip time part from date
                        if (Regex.IsMatch(item.Value, datePattern))
                        {
                            DateTime date = DateTime.Parse(item.Value);
                            filter.Value = date.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            filter.Value = item.Value;
                        }
                        report.Reportfilters.Add(filter);
                    }

                }
            }
            if (model.ReportSortOrders.Count > 0)
            {

                foreach (var item in model.ReportSortOrders)
                {
                    if (!item.FieldName.IsNullOrEmpty())
                    {
                        if (item.FieldName == "Account Name")
                        {
                            item.FieldName = "Company Name";
                        }
                        var field = await _unitOfWork.ReportFields.GetFieldByTitleAsync(item.FieldName);
                        var sortOrder = new Reportsortorder();
                        sortOrder.FieldName = item.FieldName;
                        sortOrder.Order = item.Order;
                        sortOrder.FieldPathName = field.TableName + "_" + field.FieldName;
                        report.Reportsortorders.Add(sortOrder);
                    }

                }
            }
            if (model.Users.Length > 0)
            {
                string[] userList = model.Users.Split(",");
                foreach (var userId in userList)
                {
                    var user = new Reportshare();
                    user.UserId = report.UserId;
                    user.SharedToUserId = Int32.Parse(userId);
                    report.Reportshares.Add(user);
                }
            }
            try
            {
                await _unitOfWork.Reports.AddAsync(report);
                await _unitOfWork.CommitAsync();
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Create Report.");
            }
        }

        public async Task<Report> UpdateReport(ReportConfigurationModel model)
        {

            Report report = await _unitOfWork.Reports.GetReportByIdAsync(model.ReportId);

            if (report == null)
            {
                throw new Exception("Cant find Report.");
            }

            if (!await ValidReport(model))
            {
                return report;
            }

            if (model.ReportType == "Membership")
            {
                var membershipReport = report.Membershipreports.ToList();
                foreach (var item in membershipReport)
                {
                    //var membership = model.MembershipReports.Where(x => x.ReportId == item.ReportId).FirstOrDefault();
                    item.Categories = model.Categories.ToString();
                    item.MembershipTypes = model.MembershipTypes.ToString();
                    item.Status = model.Status;
                    _unitOfWork.MembershipReports.Update(item);
                }
            }

            if (model.ReportType == "Opportunities")
            {
                var opportunityReport = report.Opportunityreports.ToList();
                foreach (var item in opportunityReport)
                {
                    item.Pipeline = model.Pipeline;
                    item.Stages = model.Stages.ToString();
                    item.Products = model.Products.ToString();
                    _unitOfWork.OpportunityReports.Update(item);
                }
            }

            report.Fields = model.Fields.ToString();
            report.UserId = model.UserId;
            report.Title = model.Title;
            report.Description = model.Description;
            report.PreviewMode = model.PreviewMode;
            report.isCommunity = model.isCommunity;
            report.LastUpdatedOn = model.LastUpdatedOn;
            report.ReportType = model.ReportType;

            if (model.ReportFilters.Count > 0 || report.Reportfilters.Count > 0)
            {
                string datePattern = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z";
                //Remove items which have been deleted
                var reportFilters = report.Reportfilters.ToList();
                foreach (var item in reportFilters)
                {
                    if (model.ReportFilters.Any(x => x.ReportFilterId == item.ReportFilterId))
                    {
                        var filter = model.ReportFilters.Where(x => x.ReportFilterId == item.ReportFilterId).FirstOrDefault();
                        item.Operator = filter.Operator;
                        //Strip time part from date
                        if (Regex.IsMatch(item.Value, datePattern))
                        {
                            DateTime date = DateTime.Parse(filter.Value);
                            item.Value = date.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            item.Value = filter.Value;
                        }
                        _unitOfWork.ReportFilters.Update(item);
                        continue;
                    }
                    _unitOfWork.ReportFilters.Remove(item);
                    report.Reportfilters.Remove(item);
                }
                // Add new filters
                foreach (var item in model.ReportFilters.Where(x => x.ReportFilterId == 0).ToList())
                {
                    if (item.ReportFieldId > 0)
                    {
                        var filter = new Reportfilter();
                        filter.ReportFieldId = item.ReportFieldId;
                        filter.Operator = item.Operator;
                        //Strip time part from date
                        if (Regex.IsMatch(item.Value, datePattern))
                        {
                            DateTime date = DateTime.Parse(item.Value);
                            filter.Value = date.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            filter.Value = item.Value;
                        }
                        report.Reportfilters.Add(filter);
                    }

                }
            }
            if (model.ReportSortOrders.Count > 0 || report.Reportsortorders.Count > 0)
            {
                //Remove items which have been deleted
                var reportSortOrders = report.Reportsortorders.ToList();
                foreach (var item in reportSortOrders)
                {
                    if (model.ReportSortOrders.Any(x => x.ReportSortOrderId == item.ReportSortOrderId))
                    {
                        var sortOrder = model.ReportSortOrders.Where(x => x.ReportSortOrderId == item.ReportSortOrderId).FirstOrDefault();
                        if (sortOrder.FieldName == "Account Name")
                        {
                            sortOrder.FieldName = "Company Name";
                        }
                        var field = await _unitOfWork.ReportFields.GetFieldByTitleAsync(sortOrder.FieldName);
                        item.FieldName = sortOrder.FieldName;
                        item.Order = sortOrder.Order;
                        item.FieldPathName = field.TableName + "_" + field.FieldName;
                        _unitOfWork.ReportSortorders.Update(item);
                        continue;
                    }
                    _unitOfWork.ReportSortorders.Remove(item);
                    report.Reportsortorders.Remove(item);
                }
                // Add new sort Orders
                foreach (var item in model.ReportSortOrders.Where(x => x.ReportSortOrderId == 0).ToList())
                {
                    if (!item.FieldName.IsNullOrEmpty())
                    {
                        if (item.FieldName == "Account Name")
                        {
                            item.FieldName = "Company Name";
                        }
                        var sortOrder = new Reportsortorder();
                        var field = await _unitOfWork.ReportFields.GetFieldByTitleAsync(item.FieldName);
                        sortOrder.FieldName = item.FieldName;
                        sortOrder.Order = item.Order;
                        sortOrder.FieldPathName = field.TableName + "_" + field.FieldName;
                        report.Reportsortorders.Add(sortOrder);

                    }

                }
            }

            //Remove deleted shared users

            var reportShares = report.Reportshares.ToList();
            string[] removedUserList = model.RemovedUsers.Split(",");
            if (model.RemovedUsers.Length > 0)
            {
                foreach (var item in reportShares)
                {
                    if (removedUserList.Any(x => Int32.Parse(x) == item.SharedToUserId))
                    {
                        _unitOfWork.ReportShares.Remove(item);
                        report.Reportshares.Remove(item);
                    }
                }
            }

            if (model.Users.Length > 0)
            {
                string[] userList = model.Users.Split(",");
                foreach (var userId in userList)
                {
                    //check if user already exists
                    if (!report.Reportshares.Any(x => x.SharedToUserId == Int32.Parse(userId)))
                    {
                        var user = new Reportshare();
                        user.UserId = report.UserId;
                        user.SharedToUserId = Int32.Parse(userId);
                        report.Reportshares.Add(user);
                    }

                }
            }
            else
            {
                if (report.Reportshares.Count > 0)
                {
                    _unitOfWork.ReportShares.RemoveRange(report.Reportshares);
                }
            }
            try
            {
                _unitOfWork.Reports.Update(report);
                await _unitOfWork.CommitAsync();
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Update Report.");
            }
        }

        public async Task<Report> UpdateFavoriteReports(ReportConfigurationModel model)
        {
            Report report = await _unitOfWork.Reports.GetReportByIdAsync(model.ReportId);

            if (report == null)
            {
                throw new Exception("Cant find Report.");
            }

            report.isFavorite = model.isFavorite;

            try
            {
                _unitOfWork.Reports.Update(report);
                await _unitOfWork.CommitAsync();
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update Report with Error: {ex.Message}");
            }

        }

        public async Task<List<ReportConfigurationModel>> GetMembershipReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetMembershipReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetContactsAccountsReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetContactsAccountsReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetEventReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetEventReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetOpportunityReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetOpportunityReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetContactsAccountsCommunityReports()
        {
            var reports = await _unitOfWork.Reports.GetContactsAccountsCommunityReportsAsync();

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetMembershipCommunityReports()
        {
            var reports = await _unitOfWork.Reports.GetMembershipCommunityReportsAsync();

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetOpportunitiesCommunityReports()
        {
            var reports = await _unitOfWork.Reports.GetOpportunitiesCommunityReportsAsync();

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetContactsAccountsSharedReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetContactsAccountsSharedReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetMembershipSharedReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetMembershipSharedReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<List<ReportConfigurationModel>> GetOpportunitiesSharedReportByUserId(int userId)
        {
            var reports = await _unitOfWork.Reports.GetOpportunitiesSharedReportsByUserAsync(userId);

            return _mapper.Map<List<ReportConfigurationModel>>(reports);
        }

        public async Task<ReportConfigurationModel> GetReportConfiguration(int id)
        {
            Report report = await _unitOfWork.Reports.GetReportByIdAsync(id);
            return _mapper.Map<ReportConfigurationModel>(report);
        }

        public async Task<ReportModel> GetReport(int id)
        {
            Report report = await _unitOfWork.Reports.GetReportByIdAsync(id);
            ReportModel reportModel = new ReportModel();
            DataTable data = new DataTable();
            if (report != null)
            {
                List<int?> customFieldIds = new List<int?>();
                reportModel.Title = report.Title;
                foreach (var fieldId in report.Fields.Split(","))
                {
                    var field = await _unitOfWork.Reports.GetReportFieldById(Int32.Parse(fieldId));

                    if (field != null)
                    {
                        if (field.TableName == "customfield")
                        {
                            customFieldIds.Add(field.CustomFieldId);
                        }
                        DataColumn column = new DataColumn($"{field.TableName}_{field.FieldName}");
                        column.DataType = typeof(string);
                        if (!data.Columns.Contains($"{field.TableName}_{field.FieldName}"))
                        {
                            data.Columns.Add(column);
                            reportModel.Columns.Add(new ReportColumnModel($"{field.TableName}_{field.FieldName}", field.FieldTitle));
                        }

                    }

                }

                try
                {
                    if (report.ReportType == "Membership")
                    {
                        var reportData = await _unitOfWork.Reports.GetMembershipReportData(report);

                        if (reportData != null)
                        {
                            foreach (var row in reportData)
                            {
                                DataRow dataRow = data.NewRow();

                                int colIndex = 0;
                                decimal feeCharged = 0;
                                decimal payment = 0;

                                foreach (DataColumn column in data.Columns)
                                {
                                    if (column.ColumnName == "membershipcategory_membershipCategoryId")
                                    {
                                        dataRow[colIndex] = row.Membership.MembershipType.CategoryNavigation.MembershipCategoryId;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membershipcategory_Name")
                                    {
                                        dataRow[colIndex] = row.Membership.MembershipType.CategoryNavigation.Name;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membershipcategory_Status")
                                    {
                                        var status = string.Empty;
                                        if (row.Membership.MembershipType.CategoryNavigation.Status == (int)Status.Active)
                                        {
                                            status = "Active";
                                        }
                                        else
                                        {
                                            status = "InActive";
                                        }
                                        dataRow[colIndex] = status;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membershiptype_membershipTypeId")
                                    {
                                        dataRow[colIndex] = row.Membership.MembershipType.MembershipTypeId;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membershiptype_Name")
                                    {
                                        dataRow[colIndex] = row.Membership.MembershipType.Name;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membershiptype_Description")
                                    {
                                        dataRow[colIndex] = row.Membership.MembershipType.Description;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membershiptype_Status")
                                    {
                                        var status = string.Empty;
                                        if (row.Membership.MembershipType.Status == (int)Status.Active)
                                        {
                                            status = "Active";
                                        }
                                        else
                                        {
                                            status = "InActive";
                                        }
                                        dataRow[colIndex] = status;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_MembershipId")
                                    {
                                        dataRow[colIndex] = row.MembershipId;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_StartDate")
                                    {
                                        dataRow[colIndex] = row.Membership.StartDate.ToShortDateString();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_EndDate")
                                    {
                                        dataRow[colIndex] = row.Membership.EndDate.ToShortDateString();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_NextBillDate")
                                    {
                                        dataRow[colIndex] = row.Membership.NextBillDate.ToShortDateString();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_RenewDate")
                                    {
                                        var renewDate = string.Empty;
                                        if (!(row.Membership.RenewalDate.Date == Constants.MySQL_MinDate))
                                        {
                                            renewDate = row.Membership.RenewalDate.ToShortDateString();
                                        }
                                        dataRow[colIndex] = renewDate;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_TerminationDate")
                                    {
                                        var terminationDate = string.Empty;
                                        if (!(row.Membership.TerminationDate.Date == Constants.MySQL_MinDate))
                                        {
                                            terminationDate = row.Membership.TerminationDate.ToShortDateString();
                                        }
                                        dataRow[colIndex] = terminationDate;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_Status")
                                    {
                                        var status = string.Empty;
                                        switch (row.Membership.Status)
                                        {
                                            case 0:
                                                status = "Inactive";
                                                break;
                                            case 1:
                                                status = "Active";
                                                break;
                                            case 2:
                                                status = "Expired";
                                                break;
                                            case 3:
                                                status = "Terminated";
                                                break;
                                            case 4:
                                                status = "On Hold";
                                                break;
                                        }
                                        dataRow[colIndex] = status;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_BillableEntityId")
                                    {
                                        dataRow[colIndex] = row.Membership.BillableEntityId;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "entity_Name")
                                    {
                                        dataRow[colIndex] = row.Membership.BillableEntity.Name;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Prefix")
                                    {
                                        dataRow[colIndex] = row.Entity.People.Select(x => x.Prefix).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_FirstName")
                                    {
                                        dataRow[colIndex] = row.Entity.People.Select(x => x.FirstName).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_LastName")
                                    {
                                        dataRow[colIndex] = row.Entity.People.Select(x => x.LastName).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Gender")
                                    {
                                        dataRow[colIndex] = row.Entity.People.Select(x => x.Gender).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Designation")
                                    {
                                        dataRow[colIndex] = row.Entity.People.Select(x => x.Designation).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_DOB")
                                    {
                                        var datarow = row.Entity.People.Select(x => x.DateOfBirth).FirstOrDefault();
                                        if (datarow.HasValue)
                                        {
                                            dataRow[colIndex] = String.Format("{0:MM/dd/yyyy}", datarow);
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = "";
                                        }
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_PrimaryPhone")
                                    {
                                        var phones = _mapper.Map<List<PhoneModel>>(row.Entity.People.SelectMany(x => x.Phones).ToList());
                                        dataRow[colIndex] = phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_PrimaryEmail")
                                    {
                                        var emails = _mapper.Map<List<EmailModel>>(row.Entity.People.SelectMany(x => x.Emails).ToList());
                                        dataRow[colIndex] = emails.GetPrimaryEmail();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_CompanyName")
                                    {
                                        dataRow[colIndex] = row.Membership.BillableEntity.Companies.Select(x => x.CompanyName).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Website")
                                    {
                                        dataRow[colIndex] = row.Membership.BillableEntity.Companies.Select(x => x.Website).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Address")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().StreetAddress;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_City")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().City;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_State")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().State;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Zip")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().Zip;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_PrimaryEmail")
                                    {
                                        var emails = _mapper.Map<List<EmailModel>>(row.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).ToList());
                                        dataRow[colIndex] = emails.GetPrimaryEmail();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_PrimaryPhone")
                                    {
                                        var phones = _mapper.Map<List<PhoneModel>>(row.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).ToList());
                                        dataRow[colIndex] = phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_BillingFee")
                                    {
                                        dataRow[colIndex] = row.Membership.Billingfees.Sum(x => x.Fee).ToString("c");
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_MembershipFee")
                                    {
                                        dataRow[colIndex] = row.Membership.MembershipType.Membershipfees.Sum(x => x.FeeAmount).ToString("c");
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "receipt_ReceiptId")
                                    {
                                        if (row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault() != null)
                                        {
                                            dataRow[colIndex] = row.Membership.Invoices.SelectMany(x => x?.Invoicedetails).Select(x => x?.Receiptdetails).FirstOrDefault().Select(x => x?.ReceiptHeaderId).FirstOrDefault().ToString();
                                        }
                                        else dataRow[colIndex] = string.Empty;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "receipt_Date")
                                    {
                                        if (row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault() != null)
                                        {
                                            var receiptDate = dataRow[colIndex] = row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeader.Date).FirstOrDefault().ToShortDateString();
                                            if (receiptDate.ToString() != "1/1/0001")
                                            {
                                                dataRow[colIndex] = receiptDate;
                                            }
                                            else
                                            {
                                                dataRow[colIndex] = string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = string.Empty;
                                        }
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "receipt_PaymentMode")
                                    {
                                        if (row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault() != null)
                                        {
                                            dataRow[colIndex] = row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeader.PaymentMode).FirstOrDefault();
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = string.Empty;
                                        }
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "receipt_Amount")
                                    {
                                        if (row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault() != null)
                                        {
                                            var receiptAmount = row.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount).ToString("c");
                                            if (receiptAmount != "$0.00")
                                            {
                                                dataRow[colIndex] = receiptAmount;
                                            }
                                            else
                                            {
                                                dataRow[colIndex] = string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = string.Empty;
                                        }
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "receipt_Portal")
                                    {
                                        string portaldata = "Staff";
                                        if (row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault() != null)
                                        {
                                            int portal = row.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeader.Portal).FirstOrDefault();
                                            if (portal == (int)Portal.MemberPortal)
                                            {
                                                portaldata = "Member";
                                            }
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = string.Empty;
                                        }
                                        dataRow[colIndex] = portaldata;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "membership_BalanceDue")
                                    {
                                        feeCharged = row.Membership.Billingfees.Sum(x => x.Fee);
                                        payment = row.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount);
                                        var balanceDue = feeCharged - payment;
                                        dataRow[colIndex] = balanceDue.ToString("c");
                                        colIndex++;
                                    }
                                }
                                var isEmpty = dataRow.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString())));
                                if (!isEmpty)
                                    data.Rows.Add(dataRow);
                            }
                        }
                    }

                    if (report.ReportType == "Opportunities")
                    {
                        foreach(var field in reportModel.Columns)
                        {
                            int currentIndex = reportModel.Columns.IndexOf(field);
                            if (field.Field.StartsWith("opportunity"))
                            {
                                string name = "opportunity_PipelineName";
                                reportModel.Columns.Insert(currentIndex, new ReportColumnModel(name, "Pipeline"));
                                DataColumn column = new DataColumn(name);
                                column.DataType = typeof(string);
                                data.Columns.Add(column);

                                name = "opportunity_StageName";
                                reportModel.Columns.Insert(currentIndex+1, new ReportColumnModel(name, "Stage"));
                                column = new DataColumn(name);
                                column.DataType = typeof(string);
                                data.Columns.Add(column);

                                name = "opportunity_ProductName";
                                column = new DataColumn(name);
                                column.DataType = typeof(string);
                                data.Columns.Add(column);
                                reportModel.Columns.Insert(currentIndex+2, new ReportColumnModel(name, "Product"));
                                break;
                            }
                        }
                        var reportData = await _unitOfWork.Reports.GetOpportunityReportData(report);

                        if (reportData != null)
                        {
                            foreach (var row in reportData)
                            {
                                DataRow dataRow = data.NewRow();

                                int colIndex = 0;
                                decimal feeCharged = 0;
                                decimal payment = 0;

                                foreach (DataColumn column in data.Columns)
                                {
                                    if (column.ColumnName == "person_Prefix")
                                    {
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_FirstName")
                                    {
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_LastName")
                                    {
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Gender")
                                    {
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Designation")
                                    {
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_DOB")
                                    {
                                        var datarow = "";
                                        //if (datarow.HasValue)
                                        //{
                                        //    dataRow[colIndex] = String.Format("{0:MM/dd/yyyy}", datarow);
                                        //}
                                        //else
                                        //{
                                        //    dataRow[colIndex] = "";
                                        //}
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_PrimaryPhone")
                                    {
                                        //var phones = _mapper.Map<List<PhoneModel>>(row.Entity.People.SelectMany(x => x.Phones).ToList());
                                        //dataRow[colIndex] = phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_PrimaryEmail")
                                    {
                                        //var emails = _mapper.Map<List<EmailModel>>(row.Entity.People.SelectMany(x => x.Emails).ToList());
                                        //dataRow[colIndex] = emails.GetPrimaryEmail();
                                        dataRow[colIndex] = "";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_CompanyName")
                                    {
                                        dataRow[colIndex] = row.Company.CompanyName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Website")
                                    {
                                        dataRow[colIndex] = row.Company.Website;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Address")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Company.Addresses.ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().StreetAddress;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_City")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Company.Addresses.ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().City;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_State")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Company.Addresses.ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().State;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Zip")
                                    {
                                        var addresses = _mapper.Map<List<AddressModel>>(row.Company.Addresses.ToList());
                                        dataRow[colIndex] = addresses.GetPrimaryAddress().Zip;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_PrimaryEmail")
                                    {
                                        var emails = _mapper.Map<List<EmailModel>>(row.Company.Emails.ToList());
                                        dataRow[colIndex] = emails.GetPrimaryEmail();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_PrimaryPhone")
                                    {
                                        var phones = _mapper.Map<List<PhoneModel>>(row.Company.Phones.ToList());
                                        dataRow[colIndex] = phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_EstimatedCloseDate")
                                    {
                                        dataRow[colIndex] = String.Format("{0:MM/dd/yyyy}", row.EstimatedCloseDate);
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_Potential")
                                    {
                                        dataRow[colIndex] = "$" + row.Potential;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_Owner")
                                    {
                                        dataRow[colIndex] = row.StaffUser.FirstName + " " + row.StaffUser.LastName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_AccountContact")
                                    {
                                        if (row.AccountContact != null )
                                        {
                                            dataRow[colIndex] = row.AccountContact.Name;
                                        }
                                        else
                                            dataRow[colIndex] = string.Empty;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_Probability")
                                    {
                                        dataRow[colIndex] = row.Probability + "%";
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_PipelineName")
                                    {
                                        dataRow[colIndex] = row.Pipeline.Name;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_ProductName")
                                    {
                                        dataRow[colIndex] = row.Pipeline.Pipelineproducts.Where(x => x.ProductId == row.ProductId).Select(x => x.ProductName).FirstOrDefault();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "opportunity_StageName")
                                    {
                                        dataRow[colIndex] = row.Pipeline.Pipelinestages.Where(x => x.StageId == row.StageId).Select(x => x.StageName).FirstOrDefault();
                                        colIndex++;
                                    }
                                }
                                var isEmpty = dataRow.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString())));
                                if (!isEmpty)
                                    data.Rows.Add(dataRow);
                            }
                        }
                    }

                    if (report.ReportType == "Contacts")
                    {

                        var reportData = await _unitOfWork.Reports.GetContactsReportData(report);

                        if (reportData != null)
                        {
                            foreach (var row in reportData)
                            {
                                DataRow dataRow = data.NewRow();

                                int colIndex = 0;
                                int counter = 0;
                                PersonModel model = new PersonModel();
                                model = _mapper.Map<PersonModel>(row);

                                foreach (DataColumn column in data.Columns)
                                {
                                    if (column.ColumnName == "person_Prefix")
                                    {
                                        dataRow[colIndex] = row.Prefix;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_FirstName")
                                    {
                                        dataRow[colIndex] = row.FirstName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_MiddleName")
                                    {
                                        dataRow[colIndex] = row.MiddleName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_LastName")
                                    {
                                        dataRow[colIndex] = row.LastName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_CasualName")
                                    {
                                        dataRow[colIndex] = row.CasualName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Suffix")
                                    {
                                        dataRow[colIndex] = row.Suffix;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Gender")
                                    {
                                        dataRow[colIndex] = row.Gender;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_DateOfBirth")
                                    {
                                        var datarow = row.DateOfBirth;
                                        if (datarow.HasValue)
                                        {
                                            dataRow[colIndex] = String.Format("{0:MM/dd/yyyy}", datarow);
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = "";
                                        }
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_PreferredContact")
                                    {
                                        var preferredContact = string.Empty;
                                        if (row.PreferredContact == (int)PreferredContact.PrimaryAddress)
                                        {
                                            preferredContact = "Primary Address";
                                        }
                                        if (row.PreferredContact == (int)PreferredContact.PrimaryEmail)
                                        {
                                            preferredContact = "Primary Email";
                                        }
                                        if (row.PreferredContact == (int)PreferredContact.PrimaryPhone)
                                        {
                                            preferredContact = "Primary Phone";
                                        }
                                        if (row.PreferredContact == (int)PreferredContact.PrimaryText)
                                        {
                                            preferredContact = "Primary Text";
                                        }
                                        dataRow[colIndex] = preferredContact;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Designation")
                                    {
                                        dataRow[colIndex] = row.Designation;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "phone_PhoneType")
                                    {
                                        dataRow[colIndex] = model.Phones.GetPrimaryPhoneType();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "phone_PhoneNumber")
                                    {
                                        dataRow[colIndex] = model.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "email_EmailAddressType")
                                    {
                                        dataRow[colIndex] = model.Emails.GetPrimaryEmailType();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "email_EmailAddress")
                                    {
                                        var emails = _mapper.Map<List<EmailModel>>(row.Emails.ToList());
                                        dataRow[colIndex] = emails.GetPrimaryEmail();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_AddressType")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.AddressType;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_Address1")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        model.PrimaryAddress = primaryAddress.StreetAddress;
                                        dataRow[colIndex] = model.PrimaryAddress;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_City")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.City;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_State")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.State;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_Zip")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.Zip.FormatZip();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_Country")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.Country;
                                        colIndex++;
                                    }

                                    if (column.ColumnName == "company_CompanyName")
                                    {
                                        if (row.Company is null)
                                        {
                                            dataRow[colIndex] = string.Empty;
                                            colIndex++;
                                        }
                                        else
                                        {
                                            dataRow[colIndex] = row.Company.CompanyName;
                                            colIndex++;
                                        }
                                    }
                                    if (column.ColumnName == "company_Title")
                                    {
                                        dataRow[colIndex] = row.Title;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Roles")
                                    {
                                        dataRow[colIndex] = string.Join(", ", row.Entity.Entityroles.Select(x => x.ContactRole.Name));
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_FacebookName")
                                    {
                                        dataRow[colIndex] = row.FacebookName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_TwitterName")
                                    {
                                        dataRow[colIndex] = row.TwitterName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_InstagramName")
                                    {
                                        dataRow[colIndex] = row.InstagramName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_LinkedinName")
                                    {
                                        dataRow[colIndex] = row.LinkedinName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "person_Website")
                                    {
                                        dataRow[colIndex] = row.Website;
                                        colIndex++;
                                    }
                                    if (column.ColumnName.StartsWith("connection"))
                                    {
                                        List<string> names = new List<string>();
                                        foreach (var item in row.Entity.RelationEntities)
                                        {
                                            if (column.ColumnName.EndsWith("PeopleName") && item.RelatedEntity.PersonId != null)
                                            {
                                                names.Add(item.RelatedEntity.Name);
                                            }
                                            if (column.ColumnName.EndsWith("CompanyName") && item.RelatedEntity.CompanyId != null)
                                            {
                                                names.Add(item.RelatedEntity.Name);
                                            }

                                        }
                                        foreach (var item in row.Entity.RelationRelatedEntities)
                                        {
                                            if (column.ColumnName.EndsWith("PeopleName") && item.Entity.PersonId != null)
                                            {
                                                names.Add(item.Entity.Name);
                                            }
                                            if (column.ColumnName.EndsWith("CompanyName") && item.Entity.CompanyId != null)
                                            {
                                                names.Add(item.Entity.Name);
                                            }
                                        }
                                        dataRow[colIndex] = string.Join(", ", names);
                                        colIndex++;
                                    }
                                    if (column.ColumnName.StartsWith("customfield"))
                                    {
                                        dataRow[colIndex] = row.Entity.Customfielddata.Where(x => x.CustomFieldId == customFieldIds[counter]).Select(x => x.Value.Trim('[', ']').Replace("\"", "")).FirstOrDefault();
                                        colIndex++;
                                        counter++;
                                    }

                                }
                                var isEmpty = dataRow.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString())));
                                if (!isEmpty)
                                    data.Rows.Add(dataRow);

                            }
                        }
                    }

                    if (report.ReportType == "Accounts")
                    {
                        var reportData = await _unitOfWork.Reports.GetAccountsReportData(report);

                        if (reportData != null)
                        {
                            foreach (var row in reportData)
                            {
                                DataRow dataRow = data.NewRow();

                                int colIndex = 0;
                                int counter = 0;
                                CompanyModel model = new CompanyModel();
                                model = _mapper.Map<CompanyModel>(row);

                                foreach (DataColumn column in data.Columns)
                                {

                                    if (column.ColumnName == "company_CompanyName")
                                    {
                                        dataRow[colIndex] = row.CompanyName;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "company_Website")
                                    {
                                        dataRow[colIndex] = row.Website;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "phone_PhoneType")
                                    {
                                        dataRow[colIndex] = model.Phones.GetPrimaryPhoneType();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "phone_PhoneNumber")
                                    {
                                        dataRow[colIndex] = model.Phones.GetPrimaryPhoneNumber().FormatPhoneNumber();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "email_EmailAddressType")
                                    {
                                        dataRow[colIndex] = model.Emails.GetPrimaryEmailType();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "email_EmailAddress")
                                    {
                                        dataRow[colIndex] = model.Emails.GetPrimaryEmail();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_AddressType")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.AddressType;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_Address1")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.StreetAddress; ;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_City")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.City;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_State")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.State;
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_Zip")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.Zip.FormatZip();
                                        colIndex++;
                                    }
                                    if (column.ColumnName == "address_Country")
                                    {
                                        var primaryAddress = model.Addresses.GetPrimaryAddress();
                                        dataRow[colIndex] = primaryAddress.Country;
                                        colIndex++;
                                    }
                                    if (column.ColumnName.StartsWith("connection"))
                                    {
                                        if (row.Entity != null)
                                        {
                                            List<string> names = new List<string>();
                                            foreach (var item in row.Entity.RelationEntities)
                                            {
                                                if (column.ColumnName.EndsWith("PeopleName") && item.RelatedEntity.PersonId != null)
                                                {
                                                    names.Add(item.RelatedEntity.Name);
                                                }
                                                if (column.ColumnName.EndsWith("CompanyName") && item.RelatedEntity.CompanyId != null)
                                                {
                                                    names.Add(item.RelatedEntity.Name);
                                                }

                                            }
                                            foreach (var item in row.Entity.RelationRelatedEntities)
                                            {
                                                if (column.ColumnName.EndsWith("PeopleName") && item.Entity.PersonId != null)
                                                {
                                                    names.Add(item.Entity.Name);
                                                }
                                                if (column.ColumnName.EndsWith("CompanyName") && item.Entity.CompanyId != null)
                                                {
                                                    names.Add(item.Entity.Name);
                                                }
                                            }
                                            dataRow[colIndex] = string.Join(", ", names);
                                        }
                                        else
                                            dataRow[colIndex] = string.Empty;
                                        colIndex++;
                                    }
                                    if (column.ColumnName.StartsWith("customfield"))
                                    {
                                        dataRow[colIndex] = row.Entity.Customfielddata.Where(x => x.CustomFieldId == customFieldIds[counter]).Select(x => x.Value.Trim('[', ']').Replace("\"", "")).FirstOrDefault();
                                        colIndex++;
                                        counter++;
                                    }

                                }
                                var isEmpty = dataRow.ItemArray.All(x => x == null || (x != null && string.IsNullOrWhiteSpace(x.ToString())));
                                if (!isEmpty)
                                    data.Rows.Add(dataRow);

                            }
                        }
                    }

                    if (report.Reportsortorders.Count > 0)
                    {
                        foreach (var sortItem in report.Reportsortorders)
                        {
                            data.DefaultView.Sort = sortItem.FieldPathName + " " + sortItem.Order;
                            reportModel.Rows = data.DefaultView.ToTable();
                        }
                    }
                    else
                    {
                        reportModel.Rows = data;
                    }

                    return reportModel;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to Create {report.ReportType} Report with Error: {ex.Message}");
                }

            }

            return reportModel;
        }

        public async Task<bool> DeleteReport(int reportId)
        {
            var report = await _unitOfWork.Reports.GetReportByIdAsync(reportId);

            if (report.Membershipreports.Count > 0)
            {
                _unitOfWork.MembershipReports.RemoveRange(report.Membershipreports);
            }
            if (report.Opportunityreports.Count > 0)
            {
                _unitOfWork.OpportunityReports.RemoveRange(report.Opportunityreports);
            }
            if (report.Reportfilters.Count > 0)
            {
                _unitOfWork.ReportFilters.RemoveRange(report.Reportfilters);
            }
            if (report.Reportshares.Count > 0)
            {
                _unitOfWork.ReportShares.RemoveRange(report.Reportshares);
            }
            if (report.Reportsortorders.Count > 0)
            {
                _unitOfWork.ReportSortorders.RemoveRange(report.Reportsortorders);
            }
            try
            {
                _unitOfWork.Reports.Remove(report);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Delete Report.");
            }

        }

        public async Task<bool> DeletePreviewModeReport(int userId)
        {
            var reports = await _unitOfWork.Reports.GetPreviewModeReportsByUserAsync(userId);

            foreach (var report in reports)
            {
                if (report.Reportfilters.Count > 0)
                {
                    _unitOfWork.ReportFilters.RemoveRange(report.Reportfilters);
                }
                if (report.Reportshares.Count > 0)
                {
                    _unitOfWork.ReportShares.RemoveRange(report.Reportshares);
                }
                if (report.Reportsortorders.Count > 0)
                {
                    _unitOfWork.ReportSortorders.RemoveRange(report.Reportsortorders);
                }
                if (report.Membershipreports.Count > 0)
                {
                    _unitOfWork.MembershipReports.RemoveRange(report.Membershipreports);
                }
                if (report.Opportunityreports.Count > 0)
                {
                    _unitOfWork.OpportunityReports.RemoveRange(report.Opportunityreports);
                }
            }
            try
            {
                _unitOfWork.Reports.RemoveRange(reports);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Delete Report.");
            }

        }

        public async Task<List<ReportFieldTableModel>> GetReportTableFields(int categoryId)
        {

            var reportfields = await _unitOfWork.Reports.GetReportCustomFieldsByCategoryId(categoryId);

            // Check if there is any custom fields exist outside of reportfield table
            int customFieldFor = 0;
            if (categoryId == 2) customFieldFor = 1;
            if (categoryId == 3) customFieldFor = 2;
            var customfields = await _unitOfWork.CustomFields.GetCustomFieldsByCustomFieldFor(customFieldFor);
            if (customfields.Count > 0)
            {
                int?[] textNumbers = { 1, 2, 3, 7, 8 };
                int?[] dropdownNumbers = { 9, 10, 11 };
                foreach (var field in customfields)
                {
                    if (reportfields.Any(item => item.CustomFieldId == field.CustomFieldId))
                    {
                        continue;
                    }
                    else
                    {
                        var reportField = new Reportfield();
                        reportField.CustomFieldId = field.CustomFieldId;
                        if (customFieldFor == 1) reportField.ReportCategoryId = 2;
                        else reportField.ReportCategoryId = 3;
                        reportField.FieldName = field.Label;
                        reportField.TableName = "customfield";
                        reportField.FieldTitle = field.Label;
                        reportField.Label = "Additional Info";
                        if (textNumbers.Contains(field.FieldTypeId))
                        {
                            reportField.DataType = "String";
                            reportField.DisplayType = "Text";
                        }
                        else if (dropdownNumbers.Contains(field.FieldTypeId))
                        {
                            reportField.DataType = "String";
                            reportField.DisplayType = "Dropdown";
                        }
                        else if (field.FieldTypeId == 6)
                        {
                            reportField.DataType = "Int";
                            reportField.DisplayType = "Number";
                        }
                        else
                        {
                            reportField.DataType = "Date";
                            reportField.DisplayType = "Calendar";
                        }
                        reportField.DisplayOrder = 7;
                        await _unitOfWork.ReportFields.AddAsync(reportField);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }

            if (customfields.Count < reportfields.Count())
            {
                foreach (var field in reportfields)
                {
                    if (!customfields.Any(item => item.CustomFieldId == field.CustomFieldId))
                    {
                        _unitOfWork.ReportFields.Remove(field);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }


            var fields = await _unitOfWork.Reports.GetReportFieldsByCategoryId(categoryId);

            List<ReportFieldTableModel> tableFieldList = new List<ReportFieldTableModel>();

            if (fields != null)
            {
                string table = string.Empty;

                ReportFieldTableModel node = new ReportFieldTableModel();
                foreach (var item in fields)
                {
                    if (table == string.Empty)
                    {
                        node.Label = item.Label;
                        node.Table = item.TableName;
                        node.Selectable = true;
                        table = item.TableName;
                    }
                    if (item.TableName != table)
                    {
                        if (node.Label != null)
                        {
                            tableFieldList.Add(node);
                            node = new ReportFieldTableModel();
                            node.Label = item.Label;
                            node.Table = item.TableName;
                            node.Selectable = true;
                            table = item.TableName;
                        }
                    }
                    ReportFieldTableModel childNode = new ReportFieldTableModel();

                    childNode.Label = item.FieldTitle;
                    childNode.Table = item.TableName;
                    childNode.Data = item.ReportFieldId;
                    childNode.Type = item.DataType;
                    childNode.Selectable = true;
                    childNode.Icon = item.DisplayType;
                    node.Children.Add(childNode);

                }

                tableFieldList.Add(node);
            }
            return (tableFieldList);
        }
        
        public async Task<Report> CloneReport(int reportId, int userId)
        {
            Report report = await _unitOfWork.Reports.GetReportByIdAsync(reportId);

            if (report == null)
            {
                throw new Exception("Cant Find Report.");
            }

            Report clonedReport = new Report();
            clonedReport.Fields = report.Fields.ToString();
            clonedReport.UserId = userId;
            clonedReport.Title = $"Copy of {report.Title}";
            clonedReport.Description = report.Description;
            clonedReport.PreviewMode = report.PreviewMode;
            clonedReport.isCommunity = 0;
            clonedReport.LastUpdatedOn = new DateTime();
            clonedReport.ReportType = report.ReportType;

            if (report.ReportType == "Membership")
            {
                var membershipReport = report.Membershipreports.Where(x => x.ReportId == report.ReportId).FirstOrDefault();
                var membership = new Membershipreport();
                membership.Categories = membershipReport.Categories;
                membership.MembershipTypes = membershipReport.MembershipTypes;
                membership.Status = membershipReport.Status;
                clonedReport.Membershipreports.Add(membership);
            }

            if (report.ReportType == "Opportunities")
            {
                var opportunityReport = report.Opportunityreports.Where(x => x.ReportId == report.ReportId).FirstOrDefault();
                var opportunity = new Opportunityreport();
                opportunity.Pipeline = opportunityReport.Pipeline;
                opportunity.Stages = opportunityReport.Stages;
                opportunity.Products = opportunity.Products;
                clonedReport.Opportunityreports.Add(opportunity);
            }

            if (report.Reportfilters.Count > 0)
            {
                foreach (var item in report.Reportfilters)
                {
                    string datePattern = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}.\d{3}Z";
                    if (item.ReportFieldId > 0)
                    {
                        var filter = new Reportfilter();
                        filter.ReportFieldId = item.ReportFieldId;
                        filter.Operator = item.Operator;
                        //Strip time part from date
                        if (Regex.IsMatch(item.Value, datePattern))
                        {
                            DateTime date = DateTime.Parse(item.Value);
                            filter.Value = date.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            filter.Value = item.Value;
                        }
                        clonedReport.Reportfilters.Add(filter);
                    }

                }
            }
            if (report.Reportsortorders.Count > 0)
            {

                foreach (var item in report.Reportsortorders)
                {
                    if (!item.FieldName.IsNullOrEmpty())
                    {
                        var field = await _unitOfWork.ReportFields.GetFieldByTitleAsync(item.FieldName);
                        var sortOrder = new Reportsortorder();
                        sortOrder.FieldName = item.FieldName;
                        sortOrder.Order = item.Order;
                        sortOrder.FieldPathName = item.FieldPathName;
                        clonedReport.Reportsortorders.Add(sortOrder);
                    }

                }
            }
            try
            {
                await _unitOfWork.Reports.AddAsync(clonedReport);
                await _unitOfWork.CommitAsync();
                return clonedReport;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Clone Report.");
            }

        }

        private async Task<bool> ValidReport(ReportConfigurationModel model)
        {
            if (model.Title.IsNullOrEmpty())
            {
                throw new NullReferenceException($"Report Title is required.");
            }

            if (model.Fields.IsNullOrEmpty())
            {
                throw new NullReferenceException($"No Field selected for the Report.");
            }

            //check for duplicate

            var report = await _unitOfWork.Reports.GetReportByTitleAndUserIdAsync(model.UserId, model.Title);

            if (report != null)
            {
                if (report.Any(x => x.PreviewMode == 0 && x.ReportId != model.ReportId && x.ReportType == model.ReportType) && model.PreviewMode == 0)
                {
                    throw new InvalidOperationException($"There is already a report with same name.");
                }
            }

            return true;
        }

        public async Task<Reportcategory> GetReportCategoryIdByReportType(string reportType)
        {
            var reportCategoryId = await _unitOfWork.Reports.GetReportCategoryIdByReportType(reportType);
            return reportCategoryId;
        }

        public async Task<Reportfield> GetCustomFieldIdByReportFieldId(int id)
        {
            var customFieldId = await _unitOfWork.Reports.GetCustomFieldIdByReportFieldId(id);
            return customFieldId;
        }
    }
}
