using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface ILookupService
    {
        Task<IEnumerable<Lookup>> GetAllLookups();

        Task<string> GetLookupValueByGroupName(string name);
        Task<Lookup> GetLookupById(int id);
        Task<Lookup> CreateLookup(LookupModel LookupModel);
        Task<bool> UpdateLookup(LookupModel LookupModel);
        Task<bool> DeleteLookup(int LookupId);
        Task<IEnumerable<Answertypelookup>> GetAnswerTypeLookup();
        Task<IEnumerable<Eventtype>> GetEventTypeLookup();
        Task<IEnumerable<Timezone>> GetTimezoneLookup();
        Task<IEnumerable<Registrationfeetype>> GetRegistrationFeeTypes();
    }
}