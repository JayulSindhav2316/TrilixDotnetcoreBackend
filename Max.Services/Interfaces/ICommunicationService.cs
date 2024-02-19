using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface ICommunicationService
    {
        Task<IEnumerable<Communication>> GetAllCommunications();
        Task<Communication> GetCommunicationById(int id);
        Task<Communication> CreateCommunication(CommunicationModel communicationModel);
        Task<IEnumerable<Communication>> GetAllCommunicationsByEntityIdId(int  personId);
        Task<Communication> UpdateCommunication(CommunicationModel communicationModel);
        Task<bool> DeleteCommunication(int communicationId);
    }
}
