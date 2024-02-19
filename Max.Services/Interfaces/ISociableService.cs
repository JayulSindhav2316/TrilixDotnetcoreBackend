using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ISociableService
    {
        Task<int> CreatePerson(PersonModel model, CompanyModel companyModel, int organizationId);
        Task<bool> UpdatePerson(int sociableUserId, string loginName, string password, string primaryEmail, int organizationId, bool isBillableManager, bool isSoicableManager, bool isUpdateRoles);

        Task<int> CreatePersonProfile(PersonModel model, CompanyModel companyModel, int organizationId);
        Task<int> UpdatePersonProfile(PersonModel model, CompanyModel companyModel, int profileId, int organizationId);
        Task<string> GetUserById(int userId, int organizationId);
        Task<SociableApiResponseModel> CreateStaffUser(StaffUserModel staffUser, int organizationId, bool isContentManager);
        Task<SociableApiResponseModel> UpdateStaffUser(int sociableUserId, string loginName, string password, string primaryEmail, int organizationId, bool isContentManager, bool isNewContentManager);
        Task<SociableApiResponseModel> UpdateUserProfile(StaffUserModel model, int profileId, int organizationId);
        Task<bool> UpdateProfileImage(DocumentModel documentModel);
        Task<bool> UpdatePersonMembership(string membershipType, int membershipStatus, int profileId, int organizationId);
        Task<bool> UpdateCustomFields(string fieldName, string fieldvalue, int profileId, int organizationId);
        Task<bool> RemoveCompanyForUser(int profileId, int organizationId);
    }
}
