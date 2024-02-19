using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;


namespace Max.Services.Interfaces
{
    public interface IEntityRoleService
    {
        Task<List<Entityrole>> GetAllEntityRoles();
        Task<Entityrole> GetEntityRoleById(int id);
        Task<Entityrole> CreateEntityRole(EntityRoleModel model);
        Task<bool> UpdateEntityRole(EntityRoleModel model);
        Task<List<Entityrole>> GetAllEntityRolesByEntityId(int entityId);
        Task<List<Entityrole>> GetActiveEntityRolesByEntityId(int entityId);
        Task<List<SelectListModel>> GetActiveEntityRoleListByEntityId(int entityId);
        Task<List<ContactRoleModel>> GetEntityRolesByCompanyId(int companyId);
        Task<List<AccountContactRoleModel>> GetActiveEntityRolesByCompanyId(int companyId);
        Task<List<SelectListModel>> GetActiveEntityRoleListByCompanyId(int companyId);
        Task<List<AccountContactRoleModel>> GetActiveAccountContactsByEntityId(int entityId);
        Task<List<AccountContactRoleModel>> GetAccountContactsByEntityId(int entityId);
        Task<List<AccountContactModel>> GetContactsByFirstAndLastName(string firstName, string lastName, int companyId);
        Task<List<AccountContactRoleModel>> GetContactsByRoleAndEntityId(int roleId, int entityId);
        Task<List<AccountContactRoleModel>> GetContactsByRoleAndCompanyId(int roleId, int companyId);
        Task<bool> UnassignEntityRole(EntityRoleModel model);
        Task<bool> DeleteAssignment(EntityRoleModel model);
        Task<List<AccountContactRoleHistoryModel>> GetContactRoleHistoryByEntityId(int entityId);
        Task<List<AccountContactRoleHistoryModel>> GetActiveContactRolesByEntityId(int entityId);
        Task<List<AccountContactRoleModel>> GetContactsByCompanyId(int companyId);
        Task<bool> UpdateEntityRoleEffectiveDates(EntityRoleModel entityRoleModel);
        Task<bool> AccountChangeRoleChangeOperation(List<EntityRoleModel> entityRoleModels);
        Task<List<AccountContactModel>> GetContactsByName(string name, int companyId);
        Task<List<SelectListModel>> GetContactSelectList(int accountId);

    }
}
