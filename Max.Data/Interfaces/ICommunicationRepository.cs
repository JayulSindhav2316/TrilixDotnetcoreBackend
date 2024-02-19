﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ICommunicationRepository : IRepository<Communication>
    {
        Task<IEnumerable<Communication>> GetAllCommunicationsAsync();
        Task<Communication> GetCommunicationByIdAsync(int id);
        Task<IEnumerable<Communication>> GetAllCommunicationsByEntityIdAsync(int entityId);
    }
}