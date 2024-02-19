using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<ReportModel>> GetAllReports();

        Task<ReportModel> GetReportById(int id);
        Task<List<ReportFieldModel>> GetReportFieldsByCategoryId(int id);
        Task<Reportcategory> GetReportCategoryIdByReportType(string reportType);
        Task<Reportfield> GetCustomFieldIdByReportFieldId(int id);
        Task<List<ReportFieldTableModel>> GetReportTableFields(int id);

        Task<Report> CreateReport(ReportConfigurationModel model);
        Task<ReportModel> GetReport(int id);
        Task<Report> UpdateFavoriteReports(ReportConfigurationModel model);
        Task<Report> UpdateReport(ReportConfigurationModel model);
        Task<ReportConfigurationModel> GetReportConfiguration(int id);
        Task<Report> CloneReport(int reportId, int userId);
        Task<bool> DeleteReport(int reportId);

        Task<List<ReportConfigurationModel>> GetMembershipCommunityReports();
        Task<List<ReportConfigurationModel>> GetMembershipSharedReportByUserId(int userId);
        Task<List<ReportConfigurationModel>> GetMembershipReportByUserId(int userId);

        Task<List<ReportConfigurationModel>> GetContactsAccountsReportByUserId(int userId);
        Task<List<ReportConfigurationModel>> GetContactsAccountsCommunityReports();
        Task<List<ReportConfigurationModel>> GetContactsAccountsSharedReportByUserId(int userId);

        Task<List<ReportConfigurationModel>> GetEventReportByUserId(int userId);

        Task<List<ReportConfigurationModel>> GetOpportunityReportByUserId(int userId);
        Task<List<ReportConfigurationModel>> GetOpportunitiesCommunityReports();
        Task<List<ReportConfigurationModel>> GetOpportunitiesSharedReportByUserId(int userId);
    }
}
