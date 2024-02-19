using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IMultiFactorCodeRepository : IRepository<Multifactorcode>
    {
        Task<Multifactorcode> GetByUserIdAsync(int userId);
        Task<Multifactorcode> GetByUserNameAsync(string userName);
        Task<Multifactorcode> GetByAccessCodeAsync(string accessCode);
        Task<Multifactorcode> GetByOtpCodeAsync(string otpCode);
        Task<Multifactorcode> GetByEntityIdAsync(int entityId);

    }
}
