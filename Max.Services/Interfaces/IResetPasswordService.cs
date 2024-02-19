using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IResetPasswordService
    {
        Task<Resetpassword> GetResetRequestById(int id);
        Task<ResetPasswordModel> CreateResetRequest(ResetPasswordRequestModel model);
        Task<bool> ResetPassword(ResetPasswordRequestModel model);
        Task<bool> ValidateResetPasswordLink(ResetPasswordRequestModel model);
        Task<ResetPasswordModel> CreateMemberPasswordResetRequest(ResetPasswordRequestModel model);
        Task<bool> ResetMemberPassword(ResetPasswordRequestModel model);
    }
}
