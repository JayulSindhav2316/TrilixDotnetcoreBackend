using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IStateService
    {
        Task<IEnumerable<StateModel>> GetStatesByCountryIdAsync(int countryId);
    }
}
