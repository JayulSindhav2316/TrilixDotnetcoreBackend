using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IResetPasswordRepository : IRepository<Resetpassword>
    {
        Task<Resetpassword> GetResetRequestByIdAsync(int id);
        Task<Resetpassword> GetResetRequestByTokenAsync(string token);
    }
}
