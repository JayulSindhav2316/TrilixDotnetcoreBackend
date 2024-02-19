using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;


namespace Max.Services.Interfaces
{
    public interface IGrouphistoryService
    {
        Task<Grouphistory> AddGroupHistoryRecord(GrouphistoryModel grouphistoryModel);
        Task<List<GrouphistoryModel>> GetGrouphistoryByEntityId(int entityId);
        Task<List<GrouphistoryModel>> GetGrouphistoryByGroupId(int groupId);
    }
}
