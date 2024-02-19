using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        public ReportRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await membermaxContext.Reports.ToListAsync();
        }


        public async Task<Report> GetReportByIdAsync(int id)
        {
            return await membermaxContext.Reports
                //.Include(r => r.Reportarguments)
                .Include(r => r.Reportfilters)
                .Include(r => r.Reportshares)
                .Include(r => r.Reportsortorders)
                .Include(r => r.Membershipreports)
                .Include(r => r.Opportunityreports)
                .SingleOrDefaultAsync(m => m.ReportId == id);
        }

        public async Task<Report> GetReportByTitleAsync(string title)
        {
            return await membermaxContext.Reports.SingleOrDefaultAsync(m => m.Title == title);
        }

        public async Task<IEnumerable<Reportfield>> GetReportFieldsByCategoryId(int id)
        {
            return await membermaxContext.Reportfields.Where(x => x.ReportCategoryId==id).Include(r => r.ReportCategory).OrderBy(x => x.DisplayOrder).ToListAsync();
        }

        public async Task<IEnumerable<Reportfield>> GetReportCustomFieldsByCategoryId(int id)
        {
            return await membermaxContext.Reportfields.Where(x => x.ReportCategoryId == id).Include(r => r.ReportCategory).Where(x=>x.CustomFieldId != null).OrderBy(x => x.DisplayOrder).ToListAsync();
        }

        public async Task<Reportfield> GetReportFieldById(int id)
        {
            return await membermaxContext.Reportfields.Where(x => x.ReportFieldId == id).FirstOrDefaultAsync();
        }

        public async Task<Reportcategory> GetReportCategoryIdByReportType(string reportType)
        {
            return await membermaxContext.Reportcategories.Where(x => x.Title == reportType).FirstOrDefaultAsync();
        }

        public async Task<Reportfield> GetCustomFieldIdByReportFieldId(int id)
        {
            return await membermaxContext.Reportfields.Where(x => x.ReportFieldId == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Report>> GetPreviewModeReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Membershipreports)
                .Include(x => x.Opportunityreports)
                .Include(x => x.Reportfilters)
                .Include(x => x.Reportshares)
                .Include(x => x.Reportsortorders)
                .Where(x => x.UserId == id && x.PreviewMode == (int)Status.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetReportByTitleAndUserIdAsync(int userId, string title)
        {
            return await membermaxContext.Reports
            .Where(x => x.Title == title && x.UserId == userId)
               .ToListAsync();
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunityReportData(Report report)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Opportunity>();
            var opportunity = report.Opportunityreports.Where(x => x.ReportId == report.ReportId).FirstOrDefault();

            if (!opportunity.Pipeline.IsNullOrEmpty())
            {
                predicate = predicate.And(x => x.PipelineId == int.Parse(opportunity.Pipeline));
            }
            if (opportunity.Products.Length > 0)
            {
                int[] products = opportunity.Products.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                predicate = predicate.And(x => products.Contains(x.ProductId ?? 0));
            }
            if (opportunity.Stages.Length > 0)
            {
                int[] stages = opportunity.Stages.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                predicate = predicate.And(x => stages.Contains(x.StageId ?? 0));
            }

            if (report.Reportfilters.Count > 0)
            {
                foreach (var filter in report.Reportfilters)
                {
                    var field = filter.Field;
                    if (field.DataType == "Date")
                    {
                        if (field.FieldName == "DOB")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    //predicate = predicate.And(x => x.People.Select(x => x.DateOfBirth == filterDate).FirstOrDefault());
                                    break;
                                case "NEQ":
                                    //predicate = predicate.And(x => x.People.Select(x => x.DateOfBirth != filterDate).FirstOrDefault());
                                    break;
                                case "GT":
                                    //predicate = predicate.And(x => x.People.Select(x => x.DateOfBirth > filterDate).FirstOrDefault());
                                    break; ;
                                case "LT":
                                    //predicate = predicate.And(x => x.People.Select(x => x.DateOfBirth < filterDate).FirstOrDefault());
                                    break; ;
                            }
                        }
                        if (field.FieldName == "EstimatedCloseDate")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value).Date;
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.EstimatedCloseDate != null && x.EstimatedCloseDate.Value.Date == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.EstimatedCloseDate == null || x.EstimatedCloseDate.Value.Date != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.EstimatedCloseDate != null && x.EstimatedCloseDate.Value.Date > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.EstimatedCloseDate != null && x.EstimatedCloseDate.Value.Date < filterDate);
                                    break; ;
                            }
                        }
                    }
                    if (field.DataType == "Currency")
                    {
                        if (field.FieldName == "Potential")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Potential == decimal.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Potential != decimal.Parse(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Potential > decimal.Parse(filter.Value));
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Potential < decimal.Parse(filter.Value));
                                    break; ;
                            }
                        }
                    }
                    if (field.DataType == "String")
                    {
                        if (field.DisplayType == "Dropdown")
                        {
                            string[] values = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                            if (field.FieldName == "State")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => values.Contains(x.Company.Addresses.Select(x => x.State).FirstOrDefault()));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !values.Contains(x.Company.Addresses.Select(x => x.State).FirstOrDefault()));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            filter.Value = filter.Value.ToLower();
                            if (field.FieldName == "CompanyName")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.CompanyName.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.CompanyName.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.CompanyName.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.Company.CompanyName.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.CompanyName.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.CompanyName.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }
                            if (field.FieldName == "Website")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.Website.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.Website.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.Website.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.Company.Website.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.Website.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.Website.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }
                            if (field.FieldName == "PrimaryPhone" && field.TableName == "company")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.Phones.Select(x => x.PhoneNumber == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.Phones.Select(x => x.PhoneNumber != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.Phones.Select(x => x.PhoneNumber.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Company.Phones.Select(x => !x.PhoneNumber.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.Phones.Select(x => x.PhoneNumber.StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.Phones.Select(x => x.PhoneNumber.EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }
                            if (field.FieldName == "PrimaryEmail" && field.TableName == "company")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.Emails.Select(x => x.EmailAddress.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.Emails.Select(x => x.EmailAddress.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.Emails.Select(x => x.EmailAddress.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Company.Emails.Select(x => !x.EmailAddress.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.Emails.Select(x => x.EmailAddress.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.Emails.Select(x => x.EmailAddress.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }
                            if (field.FieldName == "Address")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Address1.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Address1.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Address1.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => !x.Address1.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Address1.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Address1.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }
                            if (field.FieldName == "City")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.City.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.City.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.City.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => !x.City.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.City.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.City.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }
                            if (field.FieldName == "Zip")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Zip == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Zip != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Zip.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => !x.Zip.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Zip.StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Company.Addresses.Select(x => x.Zip.EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }
                            if (field.FieldName == "Owner")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.StaffUser.FirstName.ToLower() + " " + x.StaffUser.LastName.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.StaffUser.FirstName.ToLower() + " " + x.StaffUser.LastName.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.StaffUser.FirstName.ToLower().Contains(filter.Value) || x.StaffUser.LastName.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.StaffUser.FirstName.ToLower().Contains(filter.Value) && !x.StaffUser.LastName.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.StaffUser.FirstName.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.StaffUser.LastName.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }
                            if (field.FieldName == "Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.AccountContact.Name.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.AccountContact.Name.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.AccountContact.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.AccountContact.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.AccountContact.Name.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.AccountContact.Name.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }
                        }
                    }
                    if (field.DataType == "Int")
                    {
                        if (field.FieldName == "Probability")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Probability == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Probability != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Probability.ToString().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Probability.ToString().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Probability.ToString().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Probability.ToString().EndsWith(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Probability > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.Probability < Int32.Parse(filter.Value));
                                    break;
                            }
                        }
                    }

                }
            }

            var query = membermaxContext.Opportunities
            .Where(predicate)
            .Include(x => x.StaffUser)
            .Include(x => x.Pipeline)
            .Include(x => x.Pipeline)
                .ThenInclude(x => x.Pipelineproducts)
            .Include(x => x.Pipeline)
                .ThenInclude(x => x.Pipelinestages)
            .Include(x => x.Company)
            .Include(x => x.Company)
                .ThenInclude(x => x.Addresses)
            .Include(x => x.Company)
                .ThenInclude(x => x.Emails)
            .Include(x => x.Company)
                .ThenInclude(x => x.Phones)
            .Include(x => x.AccountContact)
            .AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Membershipconnection>> GetMembershipReportData(Report report)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Membershipconnection>();

            var membership = report.Membershipreports.Where(x => x.ReportId == report.ReportId).FirstOrDefault();
            if (!membership.Status.IsNullOrEmpty())
            {
                int[] statuses = membership.Status.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                predicate = predicate.And(x => statuses.Contains(x.Membership.Status));
            }

            if (membership.Categories.Length > 0)
            {
                int[] categories = membership.Categories.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                predicate = predicate.And(x => categories.Contains(x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId));
            }
            if (membership.MembershipTypes.Length > 0)
            {
                int[] types = membership.MembershipTypes.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                predicate = predicate.And(x => types.Contains(x.Membership.MembershipType.MembershipTypeId));
            }

            if (report.Reportfilters.Count > 0)
            {
                foreach (var filter in report.Reportfilters)
                {
                    var field = filter.Field;
                    if (field.DataType == "Date")
                    {
                        if (field.FieldName == "StartDate")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.StartDate == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.StartDate != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.StartDate > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.StartDate < filterDate);
                                    break; ;
                            }
                        }

                        if (field.FieldName == "EndDate")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.EndDate == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.EndDate != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.EndDate > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.EndDate < filterDate);
                                    break; ;
                            }
                        }
                        if (field.FieldName == "TerminationDate")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.TerminationDate == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.TerminationDate != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.TerminationDate > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.TerminationDate < filterDate);
                                    predicate = predicate.And(x => x.Membership.TerminationDate > Constants.MySQL_MinDate);
                                    break; ;
                            }
                        }
                        if (field.FieldName == "NextBillDate")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.NextBillDate == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.NextBillDate != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.NextBillDate > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.NextBillDate < filterDate);
                                    predicate = predicate.And(x => x.Membership.NextBillDate > Constants.MySQL_MinDate);
                                    break; ;
                            }
                        }
                        if (field.FieldName == "RenewDate")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.RenewalDate == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.RenewalDate != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.RenewalDate > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.RenewalDate < filterDate);
                                    predicate = predicate.And(x => x.Membership.RenewalDate > Constants.MySQL_MinDate);
                                    break; ;
                            }
                        }
                        if (field.FieldName == "DOB")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.People.Select(x => x.DateOfBirth == filterDate).FirstOrDefault());
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Entity.People.Select(x => x.DateOfBirth != filterDate).FirstOrDefault());
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Entity.People.Select(x => x.DateOfBirth > filterDate).FirstOrDefault());
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Entity.People.Select(x => x.DateOfBirth < filterDate).FirstOrDefault());
                                    break; ;
                            }
                        }
                        if (field.FieldName == "Date")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails).Select(x => x.ReceiptHeader.Date == filterDate).FirstOrDefault());
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails).Select(x => x.ReceiptHeader.Date != filterDate).FirstOrDefault());
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails).Select(x => x.ReceiptHeader.Date > filterDate).FirstOrDefault());
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails).Select(x => x.ReceiptHeader.Date < filterDate).FirstOrDefault());
                                    break; ;
                            }
                        }
                    }
                    if (field.DataType == "Currency")
                    {
                        if (field.FieldName == "BalanceDue")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Select(x => x.Fee).FirstOrDefault() - x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) == Convert.ToDecimal(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Select(x => x.Fee).FirstOrDefault() - x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) != Convert.ToDecimal(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Select(x => x.Fee).FirstOrDefault() - x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) > Convert.ToDecimal(filter.Value));
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Select(x => x.Fee).FirstOrDefault() - x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) < Convert.ToDecimal(filter.Value));
                                    break; ;
                            }
                        }

                        if (field.FieldName == "BillingFee")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Sum(x => x.Fee) == Convert.ToDecimal(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Sum(x => x.Fee) != Convert.ToDecimal(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Sum(x => x.Fee) > Convert.ToDecimal(filter.Value));
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.Billingfees.Sum(x => x.Fee) < Convert.ToDecimal(filter.Value));
                                    break; ;
                            }
                        }

                        if (field.FieldName == "MembershipFee")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.MembershipType.Membershipfees.Sum(x => x.FeeAmount) == Convert.ToDecimal(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.MembershipType.Membershipfees.Sum(x => x.FeeAmount) != Convert.ToDecimal(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.Membershipfees.Sum(x => x.FeeAmount) > Convert.ToDecimal(filter.Value));
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.Membershipfees.Sum(x => x.FeeAmount) < Convert.ToDecimal(filter.Value));
                                    break; ;
                            }
                        }

                        if (field.FieldName == "Amount")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) == Convert.ToDecimal(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) != Convert.ToDecimal(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) > Convert.ToDecimal(filter.Value));
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).SelectMany(x => x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created)).Sum(x => x.Amount) < Convert.ToDecimal(filter.Value));
                                    break; ;
                            }
                        }
                    }

                    if (field.DataType == "Int")
                    {
                        if (field.FieldName == "membershipCategoryId")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId.ToString().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId.ToString().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId.ToString().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId.ToString().EndsWith(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.MembershipCategoryId < Int32.Parse(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "membershipTypeId")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId.ToString().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Membership.MembershipType.MembershipTypeId.ToString().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId.ToString().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId.ToString().EndsWith(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.MembershipType.MembershipTypeId < Int32.Parse(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "MembershipId")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.MembershipId == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.MembershipId != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.MembershipId.ToString().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.MembershipId.ToString().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.MembershipId.ToString().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.MembershipId.ToString().EndsWith(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.MembershipId > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.MembershipId < Int32.Parse(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "BillableEntityId")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId.ToString().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Membership.BillableEntityId.ToString().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId.ToString().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId.ToString().EndsWith(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.BillableEntityId < Int32.Parse(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "ReceiptId")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId).FirstOrDefault() == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId).FirstOrDefault() != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId.ToString().Contains(filter.Value)).FirstOrDefault());
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => !x.ReceiptHeaderId.ToString().Contains(filter.Value)).FirstOrDefault());
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId.ToString().StartsWith(filter.Value)).FirstOrDefault());
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId.ToString().EndsWith(filter.Value)).FirstOrDefault());
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId).FirstOrDefault() > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeaderId).FirstOrDefault() < Int32.Parse(filter.Value));
                                    break;
                            }
                        }
                    }

                    if (field.DataType == "String")
                    {
                        if (field.DisplayType == "Dropdown")
                        {
                            string[] values = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                            List<int?> status = new List<int?>();
                            if (field.FieldTitle == "Category Status")
                            {
                                if (values.Contains("Active")) status.Add(1);
                                if (values.Contains("Inactive")) status.Add(0);
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => status.Contains(x.Membership.MembershipType.CategoryNavigation.Status));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !status.Contains(x.Membership.MembershipType.CategoryNavigation.Status));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Type Status")
                            {
                                if (values.Contains("Active")) status.Add(1);
                                if (values.Contains("Inactive")) status.Add(0);
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => status.Contains(x.Membership.MembershipType.Status));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !status.Contains(x.Membership.MembershipType.Status));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Membership Status")
                            {

                                if (values.Contains("Inactive")) status.Add(0);
                                if (values.Contains("Active")) status.Add(1);
                                if (values.Contains("Expired")) status.Add(2);
                                if (values.Contains("Terminated")) status.Add(3);
                                if (values.Contains("On Hold")) status.Add(4);

                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => status.Contains(x.Membership.Status));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !status.Contains(x.Membership.Status));
                                        break;
                                }

                            }

                            if (field.FieldTitle == "Prefix")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => values.Contains(x.Entity.People.Select(x => x.Prefix).FirstOrDefault()));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !values.Contains(x.Entity.People.Select(x => x.Prefix).FirstOrDefault()));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Designation")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => values.Contains(x.Entity.People.Select(x => x.Designation).FirstOrDefault()));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !values.Contains(x.Entity.People.Select(x => x.Designation).FirstOrDefault()));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Gender")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => values.Contains(x.Entity.People.Select(x => x.Gender).FirstOrDefault()));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !values.Contains(x.Entity.People.Select(x => x.Gender).FirstOrDefault()));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Payment Mode")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => values.Contains(x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeader.PaymentMode).FirstOrDefault()));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => !values.Contains(x.Membership.Invoices.SelectMany(x => x.Invoicedetails).Select(x => x.Receiptdetails).FirstOrDefault().Select(x => x.ReceiptHeader.PaymentMode).FirstOrDefault()));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "State")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Any(x => x.IsPrimary == 1) && x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Any(address => values.Contains(address.State)));
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Any(x => x.IsPrimary == 1) && !x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Any(address => values.Contains(address.State)));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            filter.Value = filter.Value.ToLower();
                            if (field.FieldTitle == "Category Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.Name.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.Name.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.Membership.MembershipType.CategoryNavigation.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.Name.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.MembershipType.CategoryNavigation.Name.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Type Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Name.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Name.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.Membership.MembershipType.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Name.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Name.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Type Description")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Description.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Description.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Description.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.Membership.MembershipType.Description.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Description.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.MembershipType.Description.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Billable Entity Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Name.ToLower() == filter.Value);
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Name.ToLower() != filter.Value);
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => !x.Membership.BillableEntity.Name.ToLower().Contains(filter.Value));
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Name.ToLower().StartsWith(filter.Value));
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Name.ToLower().EndsWith(filter.Value));
                                        break;
                                }
                            }

                            if (field.FieldTitle == "First Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.FirstName.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.FirstName.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.FirstName.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => !x.FirstName.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.FirstName.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.FirstName.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Last Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.LastName.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.LastName.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.LastName.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => !x.LastName.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.LastName.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Entity.People.Select(x => x.LastName.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Primary Phone" && field.TableName == "person")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Phones).Select(x => x.PhoneNumber == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Phones).Select(x => x.PhoneNumber != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Phones).Select(x => x.PhoneNumber.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Phones).Select(x => !x.PhoneNumber.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Phones).Select(x => x.PhoneNumber.StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Phones).Select(x => x.PhoneNumber.EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Primary Email" && field.TableName == "person")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Emails).Select(x => !x.EmailAddress.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Entity.People.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Company Name")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.Select(x => x.CompanyName.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.Select(x => x.CompanyName.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.Select(x => x.CompanyName.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.Select(x => !x.CompanyName.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.Select(x => x.CompanyName.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.Select(x => x.CompanyName.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Address")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Address1.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Address1.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Address1.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => !x.Address1.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Address1.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Address1.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "City")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.City.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.City.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.City.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => !x.City.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.City.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.City.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Zip")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Zip == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Zip != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Zip.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => !x.Zip.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Zip.StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Addresses).Select(x => x.Zip.EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Primary Phone" && field.TableName == "company")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).Select(x => x.PhoneNumber == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).Select(x => x.PhoneNumber != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).Select(x => x.PhoneNumber.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).Select(x => !x.PhoneNumber.Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).Select(x => x.PhoneNumber.StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Phones).Select(x => x.PhoneNumber.EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }

                            if (field.FieldTitle == "Primary Email" && field.TableName == "company")
                            {
                                switch (filter.Operator)
                                {
                                    case "EQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower() == filter.Value).FirstOrDefault());
                                        break;
                                    case "NEQ":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower() != filter.Value).FirstOrDefault());
                                        break;
                                    case "CT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "NCT":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).Select(x => !x.EmailAddress.ToLower().Contains(filter.Value)).FirstOrDefault());
                                        break;
                                    case "SW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower().StartsWith(filter.Value)).FirstOrDefault());
                                        break;
                                    case "EW":
                                        predicate = predicate.And(x => x.Membership.BillableEntity.Companies.SelectMany(x => x.Emails).Select(x => x.EmailAddress.ToLower().EndsWith(filter.Value)).FirstOrDefault());
                                        break;
                                }
                            }
                        }
                    }
                }

            }

            var query = membermaxContext.Membershipconnections
                .Where(predicate)
                .Include(x => x.Membership)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.BillableEntity)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.BillableEntity)
                        .ThenInclude(x => x.People)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.BillableEntity)
                        .ThenInclude(x => x.Companies)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.BillableEntity)
                        .ThenInclude(x => x.Companies)
                            .ThenInclude(x => x.Addresses)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.BillableEntity)
                        .ThenInclude(x => x.Companies)
                            .ThenInclude(x => x.Phones)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.BillableEntity)
                        .ThenInclude(x => x.Companies)
                            .ThenInclude(x => x.Emails)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.Membershipfees)
                .Include(x => x.Entity)
                        .ThenInclude(x => x.People)
                .Include(x => x.Entity)
                        .ThenInclude(x => x.People)
                            .ThenInclude(x => x.Phones)
                .Include(x => x.Entity)
                        .ThenInclude(x => x.People)
                            .ThenInclude(x => x.Emails)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.Billingfees)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.Invoices)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.Invoices)
                        .ThenInclude(x => x.Invoicedetails)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.Invoices)
                        .ThenInclude(x => x.Invoicedetails)
                            .ThenInclude(x => x.Receiptdetails)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.Invoices)
                        .ThenInclude(x => x.Invoicedetails)
                            .ThenInclude(x => x.Receiptdetails)
                                .ThenInclude(x => x.ReceiptHeader)
                .AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetMembershipReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Include(x => x.Membershipreports)
                .Where(x => x.UserId == id && x.PreviewMode != 1 && x.ReportType == "Membership")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetMembershipCommunityReportsAsync()
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Where(x => x.PreviewMode != 1 && x.isCommunity == 1 && x.ReportType == "Membership")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetMembershipSharedReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Where(x => x.Reportshares.Any(x => x.SharedToUserId == id) && x.PreviewMode != (int)Status.Active && x.ReportType == "Membership")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetOpportunitiesSharedReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Where(x => x.Reportshares.Any(x => x.SharedToUserId == id) && x.PreviewMode != (int)Status.Active && x.ReportType == "Opportunities")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetContactsAccountsReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Where(x => x.UserId == id && x.PreviewMode != 1 && (x.ReportType == "Contacts" || x.ReportType == "Accounts"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetEventReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Where(x => x.UserId == id && x.PreviewMode != 1 && x.ReportType == "Events")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetOpportunityReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Where(x => x.UserId == id && x.PreviewMode != 1 && x.ReportType == "Opportunities")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetOpportunitiesCommunityReportsAsync()
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Where(x => x.PreviewMode != 1 && x.isCommunity == 1 && x.ReportType == "Opportunities")
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetContactsAccountsCommunityReportsAsync()
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Where(x => x.PreviewMode != 1 && x.isCommunity == 1 && (x.ReportType == "Contacts" || x.ReportType == "Accounts"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetContactsAccountsSharedReportsByUserAsync(int id)
        {
            return await membermaxContext.Reports
                .Include(x => x.User)
                .Include(x => x.Reportshares)
                .Where(x => x.Reportshares.Any(x => x.SharedToUserId == id) && x.PreviewMode != (int)Status.Active && (x.ReportType == "Contacts" || x.ReportType == "Accounts"))
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetContactsReportData(Report report)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Person>();

            if (report.Reportfilters.Count > 0)
            {
                foreach (var filter in report.Reportfilters)
                {
                    var field = filter.Field;
                    if (field.DataType == "Date")
                    {
                        if (field.FieldName == "DateOfBirth")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.DateOfBirth == filterDate);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.DateOfBirth != filterDate);
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.DateOfBirth > filterDate);
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.DateOfBirth < filterDate);
                                    break; ;
                            }
                        }

                        if (field.TableName == "customfield")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) == filterDate).FirstOrDefault());
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) != filterDate).FirstOrDefault());
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) > filterDate).FirstOrDefault());
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) < filterDate).FirstOrDefault());
                                    break; ;
                            }
                        }

                    }

                    if (field.DisplayType == "Text")
                    {
                        filter.Value = filter.Value.ToLower();
                        if (field.FieldName == "FirstName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.FirstName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.FirstName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.FirstName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.FirstName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.FirstName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.FirstName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "LastName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.LastName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.LastName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.LastName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.LastName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.LastName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.LastName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "MiddleName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.MiddleName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.MiddleName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.MiddleName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.MiddleName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.MiddleName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.MiddleName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "CasualName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.CasualName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.CasualName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.CasualName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.CasualName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.CasualName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.CasualName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "Suffix")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Suffix.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Suffix.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Suffix.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Suffix.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Suffix.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Suffix.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.FieldName == "EmailAddress")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower() == filter.Value && email.IsPrimary == 1));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower() != filter.Value && email.IsPrimary == 1));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower().Contains(filter.Value) && email.IsPrimary == 1));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Emails.Any(email => email.EmailAddress.ToLower().Contains(filter.Value) && email.IsPrimary == 1));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower().StartsWith(filter.Value) && email.IsPrimary == 1));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower().EndsWith(filter.Value) && email.IsPrimary == 1));
                                    break;
                            }
                        }
                        if (field.FieldName == "Address1")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower() == filter.Value && address.IsPrimary == 1));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower() != filter.Value && address.IsPrimary == 1));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).StartsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).EndsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                            }
                        }

                        if (field.FieldName == "City")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower() == filter.Value && address.IsPrimary == 1));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower() != filter.Value && address.IsPrimary == 1));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower().Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Addresses.Any(address => address.City.ToLower().Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower().StartsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower().EndsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                            }
                        }
                        if (field.FieldName == "FacebookName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.FacebookName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.FacebookName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.FacebookName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.FacebookName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.FacebookName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.FacebookName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "TwitterName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.TwitterName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.TwitterName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.TwitterName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.TwitterName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.TwitterName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.TwitterName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "LinkedinName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.LinkedinName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.LinkedinName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.LinkedinName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.LinkedinName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.LinkedinName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.LinkedinName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.FieldName == "Website")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Website.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Website.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Website.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Website.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Website.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Website.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.FieldName == "InstagramName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.InstagramName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.InstagramName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.InstagramName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.InstagramName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.InstagramName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.InstagramName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.FieldName == "CompanyName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Company.CompanyName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Company.CompanyName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Company.CompanyName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Company.CompanyName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Company.CompanyName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Company.CompanyName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }

                        if (field.FieldName == "Title")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Title.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Title.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Title.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Title.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Title.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Title.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.TableName == "customfield")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                    }

                    if (field.FieldName == "Prefix")
                    {
                        string[] prefixValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => prefixValues.Contains(x.Prefix));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => !prefixValues.Contains(x.Prefix));
                                break;
                        }
                    }

                    if (field.FieldName == "PreferredContact")
                    {
                        string[] preferredValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        List<int?> values = new List<int?>();
                        if (preferredValues.Contains("Primary Address")) values.Add(3);
                        if (preferredValues.Contains("Primary Email")) values.Add(1);
                        if (preferredValues.Contains("Primary Phone")) values.Add(2);
                        if (preferredValues.Contains("Primary Text")) values.Add(4);
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => values.Contains(x.PreferredContact));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => !values.Contains(x.PreferredContact));
                                break;
                        }
                    }

                    if (field.FieldName == "PhoneType")
                    {
                        string[] phoneValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phoneValues.Contains(phone.PhoneType) && phone.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.IsPrimary == 1) && !x.Phones.Any(phone => phoneValues.Contains(phone.PhoneType)));
                                break;
                        }
                    }

                    if (field.FieldName == "PhoneNumber")
                    {
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber == filter.Value && phone.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber != filter.Value && phone.IsPrimary == 1));
                                break;
                            case "CT":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber.Contains(filter.Value) && phone.IsPrimary == 1));
                                break;
                            case "NCT":
                                predicate = predicate.And(x => !x.Phones.Any(phone => phone.PhoneNumber.Contains(filter.Value) && phone.IsPrimary == 1));
                                break;
                            case "SW":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber.StartsWith(filter.Value) && phone.IsPrimary == 1));
                                break;
                            case "EW":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber.EndsWith(filter.Value) && phone.IsPrimary == 1));
                                break;
                        }
                    }

                    if (field.FieldName == "Gender")
                    {
                        string[] genderValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => genderValues.Contains(x.Gender));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => !genderValues.Contains(x.Gender));
                                break;
                        }
                    }
                    if (field.FieldName == "Designation")
                    {
                        string[] designationValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => designationValues.Contains(x.Designation));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => !designationValues.Contains(x.Designation));
                                break;
                        }
                    }

                    if (field.FieldName == "Roles")
                    {
                        string[] roles = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Entity.Entityroles.Any(role => roles.Contains(role.ContactRole.Name)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Entity.Entityroles.All(role => !roles.Contains(role.ContactRole.Name)));
                                break;
                        }
                    }

                    if (field.FieldName == "EmailAddressType")
                    {
                        string[] emailValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Emails.Any(email => emailValues.Contains(email.EmailAddressType) && email.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Emails.Any(email => email.IsPrimary == 1) && !x.Emails.Any(email => emailValues.Contains(email.EmailAddressType))); ;
                                break;
                        }
                    }

                    if (field.FieldName == "AddressType")
                    {
                        string[] addressValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && x.Addresses.Any(address => addressValues.Contains(address.AddressType)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && !x.Addresses.Any(address => addressValues.Contains(address.AddressType)));
                                break;
                        }
                    }
                    if (field.FieldName == "State")
                    {
                        string[] stateValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && x.Addresses.Any(address => stateValues.Contains(address.State)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && !x.Addresses.Any(address => stateValues.Contains(address.State)));
                                break;
                        }
                    }
                    if (field.FieldName == "Zip")
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip == filter.Value && address.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip != filter.Value && address.IsPrimary == 1));
                                break;
                            case "CT":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip.Contains(filter.Value) && address.IsPrimary == 1));
                                break;
                            case "NCT":
                                predicate = predicate.And(x => !x.Addresses.Any(address => address.Zip.Contains(filter.Value) && address.IsPrimary == 1));
                                break;
                            case "SW":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip.StartsWith(filter.Value) && address.IsPrimary == 1));
                                break;
                            case "EW":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip.EndsWith(filter.Value) && address.IsPrimary == 1));
                                break;
                        }
                    if (field.FieldName == "Country")
                    {
                        string[] countryValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && x.Addresses.Any(address => countryValues.Contains(address.Country)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && !x.Addresses.Any(address => countryValues.Contains(address.Country)));
                                break;
                        }
                    }
                    if (field.TableName == "customfield" && field.DisplayType == "Dropdown")
                    {
                        string[] customfieldValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Any(data => customfieldValues.Contains(data.Value.Trim('"'))));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => !x.Entity.Customfielddata.Any(data => customfieldValues.Contains(data.Value.Trim('"'))));
                                break;
                        }
                    }
                    if (field.TableName == "customfield" && field.DataType == "Int")
                    {
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() == Int32.Parse(filter.Value));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() != Int32.Parse(filter.Value));
                                break;
                            case "CT":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().Contains(filter.Value));
                                break;
                            case "NCT":
                                predicate = predicate.And(x => !x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().Contains(filter.Value));
                                break;
                            case "SW":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().StartsWith(filter.Value));
                                break;
                            case "EW":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().EndsWith(filter.Value));
                                break;
                            case "GT":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() > Int32.Parse(filter.Value));
                                break;
                            case "LT":
                                predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() < Int32.Parse(filter.Value));
                                break;
                        }
                    }
                }

            }

            var query = membermaxContext.People
                .Where(predicate)
                .Include(x => x.Entity)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Customfielddata)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Entityroles)
                        .ThenInclude(x => x.ContactRole)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.RelationEntities)
                         .ThenInclude(x => x.RelatedEntity)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.RelationRelatedEntities)
                         .ThenInclude(x => x.Entity)
                .Include(x => x.Company)
                .Include(x => x.Addresses)
                .Include(x => x.Emails)
                .Include(x => x.Phones)
                .AsNoTracking();

            return await query.ToListAsync();
        }



        public async Task<IEnumerable<Company>> GetAccountsReportData(Report report)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Company>();

            if (report.Reportfilters.Count > 0)
            {
                foreach (var filter in report.Reportfilters)
                {
                    var field = filter.Field;

                    if (field.DisplayType == "Text")
                    {
                        filter.Value = filter.Value.ToLower();
                        if (field.FieldName == "CompanyName")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.CompanyName.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.CompanyName.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.CompanyName.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.CompanyName.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.CompanyName.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.CompanyName.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.FieldName == "Website")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Website.ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Website.ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Website.ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Website.ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Website.ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Website.ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                        if (field.FieldName == "City")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower() == filter.Value && address.IsPrimary == 1));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower() != filter.Value && address.IsPrimary == 1));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower().Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Addresses.Any(address => address.City.ToLower().Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower().StartsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.City.ToLower().EndsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                            }
                        }
                        if (field.FieldName == "EmailAddress")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower() == filter.Value && email.IsPrimary == 1));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower() != filter.Value && email.IsPrimary == 1));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower().Contains(filter.Value) && email.IsPrimary == 1));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Emails.Any(email => email.EmailAddress.ToLower().Contains(filter.Value) && email.IsPrimary == 1));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower().StartsWith(filter.Value) && email.IsPrimary == 1));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Emails.Any(email => email.EmailAddress.ToLower().EndsWith(filter.Value) && email.IsPrimary == 1));
                                    break;
                            }
                        }
                        if (field.FieldName == "Address1")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower() == filter.Value && address.IsPrimary == 1));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Addresses.Any(address => address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower() != filter.Value && address.IsPrimary == 1));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).Contains(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).StartsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Addresses.Any(address => (address.Address1.ToLower() + " " + address.Address2.ToLower() + " " + address.Address3.ToLower()).EndsWith(filter.Value) && address.IsPrimary == 1));
                                    break;
                            }
                        }
                        if (field.TableName == "customfield")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower() == filter.Value);
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower() != filter.Value);
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().ToLower().EndsWith(filter.Value));
                                    break;
                            }
                        }
                    }

                    if (field.FieldName == "EmailAddressType")
                    {
                        string[] emailValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Emails.Any(email => emailValues.Contains(email.EmailAddressType) && email.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Emails.Any(email => email.IsPrimary == 1) && !x.Emails.Any(email => emailValues.Contains(email.EmailAddressType))); ;
                                break;
                        }
                    }

                    if (field.FieldName == "PhoneType")
                    {
                        string[] phoneValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phoneValues.Contains(phone.PhoneType) && phone.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.IsPrimary == 1) && !x.Phones.Any(phone => phoneValues.Contains(phone.PhoneType)));
                                break;
                        }
                    }

                    if (field.FieldName == "PhoneNumber")
                    {
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber == filter.Value));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber != filter.Value));
                                break;
                            case "CT":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber.Contains(filter.Value) && phone.IsPrimary == 1));
                                break;
                            case "NCT":
                                predicate = predicate.And(x => !x.Phones.Any(phone => phone.PhoneNumber.Contains(filter.Value) && phone.IsPrimary == 1));
                                break;
                            case "SW":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber.StartsWith(filter.Value) && phone.IsPrimary == 1));
                                break;
                            case "EW":
                                predicate = predicate.And(x => x.Phones.Any(phone => phone.PhoneNumber.EndsWith(filter.Value) && phone.IsPrimary == 1));
                                break;
                        }
                    }

                    if (field.FieldName == "AddressType")
                    {
                        string[] addressValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && x.Addresses.Any(address => addressValues.Contains(address.AddressType)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && !x.Addresses.Any(address => addressValues.Contains(address.AddressType)));
                                break;
                        }
                    }
                    if (field.FieldName == "State")
                    {
                        string[] stateValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => stateValues.Contains(address.State)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => !x.Addresses.Any(address => stateValues.Contains(address.State)));
                                break;
                        }
                    }
                    if (field.FieldName == "Zip")
                    {
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip == filter.Value && address.IsPrimary == 1));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip != filter.Value && address.IsPrimary == 1));
                                break;
                            case "CT":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip.Contains(filter.Value) && address.IsPrimary == 1));
                                break;
                            case "NCT":
                                predicate = predicate.And(x => !x.Addresses.Any(address => address.Zip.Contains(filter.Value) && address.IsPrimary == 1));
                                break;
                            case "SW":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip.StartsWith(filter.Value) && address.IsPrimary == 1));
                                break;
                            case "EW":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.Zip.EndsWith(filter.Value) && address.IsPrimary == 1));
                                break;
                        }
                    }
                    if (field.FieldName == "Country")
                    {
                        string[] countryValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                        switch (filter.Operator)
                        {
                            case "EQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && x.Addresses.Any(address => countryValues.Contains(address.Country)));
                                break;
                            case "NEQ":
                                predicate = predicate.And(x => x.Addresses.Any(address => address.IsPrimary == 1) && !x.Addresses.Any(address => countryValues.Contains(address.Country)));
                                break;
                        }
                    }
                    if (field.TableName == "customfield")
                    {
                        if (field.DisplayType == "Dropdowm")
                        {
                            string[] customfieldValues = filter.Value.Split(',').Select(value => value.Trim()).ToArray();
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Any(data => customfieldValues.Contains(data.Value.Trim('"'))));
                                    break;
                                case "NEQ":
                                    //predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).All(data => !values.Contains(data.Value)));
                                    break;
                            }
                        }
                        if (field.DisplayType == "Number")
                        {
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() == Int32.Parse(filter.Value));
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() != Int32.Parse(filter.Value));
                                    break;
                                case "CT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().Contains(filter.Value));
                                    break;
                                case "NCT":
                                    predicate = predicate.And(x => !x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().Contains(filter.Value));
                                    break;
                                case "SW":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().StartsWith(filter.Value));
                                    break;
                                case "EW":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => data.Value).FirstOrDefault().EndsWith(filter.Value));
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() > Int32.Parse(filter.Value));
                                    break;
                                case "LT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(data => data.CustomFieldId == field.CustomFieldId).Select(data => Int32.Parse(data.Value)).FirstOrDefault() < Int32.Parse(filter.Value));
                                    break;
                            }
                        }
                        if (field.DisplayType == "Calendar")
                        {
                            DateTime filterDate = DateTime.Parse(filter.Value);
                            switch (filter.Operator)
                            {
                                case "EQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) == filterDate).FirstOrDefault());
                                    break;
                                case "NEQ":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) != filterDate).FirstOrDefault());
                                    break;
                                case "GT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) > filterDate).FirstOrDefault());
                                    break; ;
                                case "LT":
                                    predicate = predicate.And(x => x.Entity.Customfielddata.Where(x => x.CustomFieldId == field.CustomFieldId).Select(x => DateTime.Parse(x.Value) < filterDate).FirstOrDefault());
                                    break; ;
                            }
                        }

                    }

                }

            }
            var query = membermaxContext.Companies
                .Where(predicate)
                .Include(x => x.Addresses)
                .Include(x => x.Emails)
                .Include(x => x.Phones)
                .Include(x => x.Entity)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Customfielddata)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.RelationEntities)
                         .ThenInclude(x => x.RelatedEntity)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.RelationRelatedEntities)
                         .ThenInclude(x => x.Entity)
                .AsNoTracking();

            return await query.ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
