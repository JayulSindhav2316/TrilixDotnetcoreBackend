using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IContactActivityRepository : IRepository<Contactactivity>
    {
        Task<IEnumerable<Contactactivity>> GetAllContactActivitiesAsync();
        Task<IEnumerable<Contactactivity>> GetContactActivityByEntityIdAsync(int id);
        Task<IEnumerable<Contactactivity>> GetEntityRoleActivityByEntityIdAsync(int id,
            List<int> selectedRoleContactsEntity, List<int> selectedRoleHistoryEntity, string roleName, int roleId);
        Task<IEnumerable<Contactactivity>> GetContactActivitiesByAccountIdAsync(int accountId);
        Task<IEnumerable<Contactactivity>> GetContactActivityByAccountAndEntityIdAsync(int accountId, int entityId);
        Task<Contactactivity> GetContactActivityByIdAsync(int id);
        Task<IEnumerable<Contactactivity>> GetContactActivityBySearchConditionAsync(int entityId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId);
        Task<IEnumerable<Contactactivity>> GetAccountActivityBySearchConditionAsync(int accountId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId);
        Task<IEnumerable<Contactactivity>> GetEntityRoleActivityBySearchConditionAsync(int accountId,
            DateTime? fromDate, DateTime? toDate, int? interactionType,
            int? interactionEntityId, List<int> selectedRoleContactsEntity, List<int> selectedRoleHistoryEntity,
            string roleName, int roleId);
        Task<IEnumerable<Contactactivity>> GetContactActivityByActivityDateAsync(int entityId, DateTime activityDate);
        Task<IEnumerable<Contactactivity>> GetContactActivityByRoleIdAndActivityDateAsync(int entityId,
            int roleId, DateTime activityDate);
        Task<IEnumerable<Contactactivity>> GetContactActivityByEffectiveDateAndEndDateAsync(int entityId,
            int accountId, int contactRoleId, DateTime? effectiveDate, DateTime? endDate);
        Task<IEnumerable<Contactactivity>> GetAccountActivityByEntityAccountDateAndRole(int accountId,
            int entityId, string roleName, DateTime date, bool isAssign);
        Task<IEnumerable<Contactactivity>> GetRoleAssignmentActivities(int entityId, int accountId,  int roleId);
    }
}
