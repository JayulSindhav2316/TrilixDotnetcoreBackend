using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;


namespace Max.Services.Interfaces
{
    public interface IContactActivityService
    {
        Task<List<ContactActivityModel>> GetAllContactActivities();
        Task<ContactActivityModel> GetContactActivityById(int id);
        Task<Contactactivity> CreateContactActivity(ContactActivityInputModel model);
        Task<List<ContactActivityOutputModel>> GetContactActivityByEntityId(int id, int? recordsCount = 0);
        Task<List<ContactActivityOutputModel>> GetRoleActivityByEntityId(int id, int roleId);
        Task<List<ContactActivityOutputModel>> GetContactActivityByAccountIdAndEntityId(int accountId, int entityId);
        Task<bool> UpdateContactActivity(ContactActivityInputModel model);
        Task<bool> DeleteContactActivity(int id);
        Task<IEnumerable<ContactActivityOutputModel>> GetContactActivityBySearchCondition(int entityId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId);
        Task<IEnumerable<ContactActivityOutputModel>> GetRoleActivityBySearchCondition(int entityId,
            DateTime? fromDate, DateTime? toDate, int? interactionType, int? interactionEntityId, int roleId);
        Task<IEnumerable<ContactActivityModel>> GetContactActivityByActivityDate(int entityId, DateTime activityDate);
        Task<IEnumerable<ContactActivityModel>> GetContactActivityByRoleIdAndActivityDateAsync(int entityId,
            int roleId, DateTime activityDate);
        Task<bool> CreateAccountChangeContactActivity(ContactActivityInputModel model, bool isAccountAssigned);
    }
}
