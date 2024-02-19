using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IEntityService
    {
        Task<List<EntityMembershipHistoryModel>> GetMembershipHistoryByEntityId(int id);
        Task<List<EntityBillingModel>> GetScheduledBillingByEntityId(int id);
        Task<EntitySummaryModel> GetEntitySummaryById(int id);
        Task<EntityModel> GetEntityProfileById(int id);
        Task<EntityMembershipProfileModel> GetMembershipProfileById(int entityId);
        Task<decimal> GetCreditBalanceById(int entityId);
        Task<Entity> CreateEntity(EntityModel entity);
        Task<EntityModel> GetEntityById(int id);
        Task<List<Entity>> GetEntitiesByName(string name);
        Task<BillingAddressModel> GetBillingAddressByEntityId(int id);
        Task<bool> AddBillableContact(int entityId, int billingContactId);
        Task<IEnumerable<EntityDisplayModel>> GetEntitiesByEntityIds(string ids);
        Task<bool> UpdateWebLoginPasword(WebLoginModel model);
        Task<bool> UpdateBillingNotification(BillingNotificationModel model);
        Task<int> UpdateLoginStatus(int entityId, int status);
        Task<int> UpdateLoginStatus(Multifactorcode code, int rememberDevice, int status);
        Task<EntitySociableModel> GetSociableEntityDetailsById(int entityId);
        Task<Entity> GetEntityByWebLogin(string webLogin);
    }

}