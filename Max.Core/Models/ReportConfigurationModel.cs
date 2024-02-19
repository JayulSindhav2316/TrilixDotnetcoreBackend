using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReportConfigurationModel
    {
        public ReportConfigurationModel()
        {
            MembershipReports = new List<MembershipReportConfigurationModel>();
            EventReports = new List<EventReportConfigurationModel>();
            OpportunityReports = new List<OpportunityReportConfigurationModel>();

            ReportFilters = new List<ReportFilterModel>();
            ReportShares = new List<ReportShareModel>();
            ReportSortOrders = new List<ReportSortOrderModel>();
        }

        public int ReportId { get; set; }
        public int OrganizationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string Fields { get; set; }
        public string ReportType { get; set; }
        public int ReportCategoryId { get; set; }
        public int isCommunity { get; set; }
        public int isFavorite { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int PreviewMode { get; set; }
        public string RemovedUsers { get; set; }
        public string Users { get; set; }
        public string Categories { get; set; }
        public string MembershipTypes { get; set; }
        public string Event { get; set; }
        public string Sessions { get; set; }
        public string Status { get; set; }
        public string Pipeline { get; set; }
        public string Stages { get; set; }
        public string Products { get; set; }
        public StaffUserModel User { get; set; }

        public List<ReportFilterModel> ReportFilters { get; set; }
        public List<ReportShareModel> ReportShares { get; set; }
        public List<ReportSortOrderModel> ReportSortOrders { get; set; }

        public List<MembershipReportConfigurationModel> MembershipReports { get; set; }
        public List<EventReportConfigurationModel> EventReports { get; set; }
        public List<OpportunityReportConfigurationModel> OpportunityReports { get; set; }

        //public List<ReportArgumentModel> ReportArguments { get; set; }
    }
}
