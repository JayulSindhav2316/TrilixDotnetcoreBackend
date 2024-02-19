using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IStaffUserService
    {
        Task<List<StaffUserModel>> GetAllStaffUsers();
        Task<Staffuser> GetStaffUserById(int id);
        Task<Staffuser> GetStaffUserByName(string userName);
        Task<Staffuser> CreateStaffUser(StaffUserModel  staffUser);
        Task<bool> UpdateStaffUser(StaffUserModel staffUser);
        Task<bool> DeleteStaffUser(int staffUserId);
        Task<bool> ResetPassword(int staffUserId, string password);
        Task<bool> AssignRole(int staffUserId, int roleId);
        Task<int> UpdateLoginStatus(Multifactorcode code,int remeberDevice,int status);
        Task<int> UpdateLoginStatus(int userId, int status);
        Task<List<StaffUserModel>> GetAllStaffUsersByNameAndUserNameAndEmail(string value);
        Task<List<SelectListModel>> GetStaffUserSelectList();
    }
}   