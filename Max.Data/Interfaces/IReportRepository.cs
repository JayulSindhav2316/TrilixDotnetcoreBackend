using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<IEnumerable<Report>> GetAllReportsAsync();
        Task<Report> GetReportByIdAsync(int id);
        Task<Report> GetReportByTitleAsync(string title);
        Task<IEnumerable<Reportfield>> GetReportFieldsByCategoryId(int id);
        Task<IEnumerable<Reportfield>> GetReportCustomFieldsByCategoryId(int id);
        Task<Reportfield> GetReportFieldById(int id);
        Task<Reportcategory> GetReportCategoryIdByReportType(string reportType);
        Task<Reportfield> GetCustomFieldIdByReportFieldId(int id);

        Task<IEnumerable<Report>> GetPreviewModeReportsByUserAsync(int id);
        Task<IEnumerable<Report>> GetReportByTitleAndUserIdAsync(int userId, string title);

        //Membership reports
        Task<IEnumerable<Membershipconnection>> GetMembershipReportData(Report report);
        Task<IEnumerable<Report>> GetMembershipReportsByUserAsync(int id);
        Task<IEnumerable<Report>> GetMembershipCommunityReportsAsync();
        Task<IEnumerable<Report>> GetMembershipSharedReportsByUserAsync(int id);

        // Contacts Accounts reports
        Task<IEnumerable<Person>> GetContactsReportData(Report report);
        Task<IEnumerable<Company>> GetAccountsReportData(Report report);
        Task<IEnumerable<Report>> GetContactsAccountsReportsByUserAsync(int id);
        Task<IEnumerable<Report>> GetContactsAccountsCommunityReportsAsync();
        Task<IEnumerable<Report>> GetContactsAccountsSharedReportsByUserAsync(int id);

        // Event reports
        Task<IEnumerable<Report>> GetEventReportsByUserAsync(int id);

        // Opportunity reports
        Task<IEnumerable<Report>> GetOpportunityReportsByUserAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunityReportData(Report report);
        Task<IEnumerable<Report>> GetOpportunitiesCommunityReportsAsync();
        Task<IEnumerable<Report>> GetOpportunitiesSharedReportsByUserAsync(int id);
    }
}
