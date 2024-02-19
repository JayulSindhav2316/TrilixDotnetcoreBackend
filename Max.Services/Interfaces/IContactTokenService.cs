using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;


namespace Max.Services.Interfaces
{
    public interface IContactTokenService
    {
        Task<Contacttoken> CreateContactToken(string contactEmail, string ipAddress);
        Task<Contacttoken> GetContactTokenByEmailId(string email);
        Task<bool> ValidateContactToken(string email, string token);
        Task<bool> UpdateContactToken(string email, string token);
    }
}
