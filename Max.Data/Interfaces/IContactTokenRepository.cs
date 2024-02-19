using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IContactTokenRepository : IRepository<Contacttoken>
    {
        Task<Contacttoken> GetTokenRequestByIdAsync(int id);
        Task<Contacttoken> GetTokenRequestByTokenAsync(string token);
        Task<Contacttoken> GetTokenRequestByEmailAsync(string emailAddress);
        
    }
}
