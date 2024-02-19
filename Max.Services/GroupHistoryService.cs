using AutoMapper;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class GrouphistoryService : IGrouphistoryService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GrouphistoryService> _logger;
        public GrouphistoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GrouphistoryService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
        }
        public async Task<Grouphistory> AddGroupHistoryRecord(GrouphistoryModel grouphistoryModel)
        {

            Grouphistory groupHistory = new Grouphistory
            {
                GroupId = grouphistoryModel.GroupId,
                ActivityDate = DateTime.Now,
                ActivityType = grouphistoryModel.ActivityType,
                ActivityStaffId = 1,
                ActivityDescription = grouphistoryModel.ActivityDescription
            };

            try
            {
                await _unitOfWork.GroupHistories.AddAsync(groupHistory);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                            ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return groupHistory;
        }

        public async Task<Grouphistory> AddGroupHistoryForNewGroupMemberRecord(GrouphistoryModel grouphistoryModel)
        {

            Grouphistory groupHistory = new Grouphistory
            {
                GroupId = grouphistoryModel.GroupId,
                ActivityDate = DateTime.Now,
                ActivityType = grouphistoryModel.ActivityType,
                ActivityStaffId = 1,
                ActivityDescription = grouphistoryModel.ActivityDescription
            };

            try
            {
                await _unitOfWork.GroupHistories.AddAsync(groupHistory);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                            ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return groupHistory;
        }

        public async Task<List<GrouphistoryModel>> GetGrouphistoryByEntityId(int entityId)
        {
            var list = await _unitOfWork.GroupHistories.GetGrouphistoryByEntityIdAsync(entityId);
            return _mapper.Map<List<GrouphistoryModel>>(list);
        }

        public async Task<List<GrouphistoryModel>> GetGrouphistoryByGroupId(int groupId)
        {
            var list = await _unitOfWork.GroupHistories.GetGrouphistoryByGroupIdAsync(groupId);
            return _mapper.Map<List<GrouphistoryModel>>(list);
        }
    }
}
