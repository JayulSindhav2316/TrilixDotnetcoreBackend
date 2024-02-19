using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<CountryModel>> GetAllCountriesAsync();
    }
}
