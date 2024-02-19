using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using AutoMapper;

namespace Max.Services
{
    public class MembershipConnectionService : IMembershipConnectionService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MembershipConnectionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        } 
        public async Task<Membershipconnection> CreateMembershipConnection(MembershipConnectionModel model) 
        {
            Membershipconnection MembershipConnection = new Membershipconnection();

            //MembershipConnection.MembershipConnectionId = model.MembershipConnectionId;
            MembershipConnection.MembershipId = model.MembershipId;
            MembershipConnection.EntityId = model.EntityId;
            MembershipConnection.Status = model.Status;

             await _unitOfWork.MembershipConnections.AddAsync(MembershipConnection);
             await _unitOfWork.CommitAsync();
            
            return MembershipConnection;
        }

        public async Task<IEnumerable<Membershipconnection>> GetMembershipConnectionsByMembershipId(int membershipId)
        {
            var membershipConnection = await _unitOfWork.MembershipConnections.GetMembershipConnectionsByMembershipIdAsync(membershipId);

            return membershipConnection;
        }

    }
}
