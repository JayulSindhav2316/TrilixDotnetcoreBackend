using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Max.Services
{
    public class GroupRegistrationService : IGroupRegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupRegistrationService> _logger;
        public GroupRegistrationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GroupRegistrationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> RegisterGroup(RegistrationGroupModel group)
        {
            var response = false;
            bool isValid = await ValidateGroupName(group);
            if(isValid)
            {
                try
                {
                    var model = _mapper.Map<Registrationgroup>(group);
                    var res = _unitOfWork.GroupRegistrations.RegisterGroup(model);
                    await _unitOfWork.CommitAsync();
                    response = true;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return response;
           
        }

        public async Task<List<Registrationgroup>> GetRegisterGroups(string searchText)
        {
            try
            {
                var res = await _unitOfWork.GroupRegistrations.GetAll(searchText);
                return res.OrderByDescending(x=>x.RegistrationGroupId).ToList();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> DeleteGroup(int id)
        {
            //var linkedTypes=_unitOfWork.GroupRegistrations.
            var linkedEntity = await _unitOfWork.GroupRegistrations.GetLinkTypesByGroupId(id);
            linkedEntity.ForEach(async item =>
            {
                await _unitOfWork.GroupRegistrations.DeleteLink(item.RegistrationGroupMembershipLinkId);
            });

            var entity = _unitOfWork.GroupRegistrations.Find(x => x.RegistrationGroupId == id).FirstOrDefault();
           _unitOfWork.GroupRegistrations.Remove(entity);
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> DeleteLink(int linkId)
        {
           await _unitOfWork.GroupRegistrations.DeleteLink(linkId);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> UpdateGroup(RegistrationGroupModel group)
        {
            var response = false;
            bool isValid = await ValidateGroupName(group);
            if (isValid)
            {
                var entity = _unitOfWork.GroupRegistrations.Find(x => x.RegistrationGroupId == group.RegistrationGroupId).FirstOrDefault();
                var types = await _unitOfWork.GroupRegistrations.GetLinkTypesByGroupId(group.RegistrationGroupId);
                if (types.Count() > 0 && group.Type != "Membership")
                {
                    entity.Registrationgroupmembershiplinks.ToList().ForEach(item =>
                    {
                        _unitOfWork.GroupRegistrations.DeleteLink(item.RegistrationGroupMembershipLinkId);
                    });
                }
                entity.Status = group.Status;
                entity.Name = group.Name;
                entity.Type = group.Type;
                _unitOfWork.GroupRegistrations.Update(entity);
                await _unitOfWork.CommitAsync();
                response = true;
            }
            return response;
        }

        public async Task<bool> LinkMembership(RegistrationGroupModel group)
        {
            try
            {
                if (group.MembershipTypeIds.Count() > 0)
                {
                    var linkedEntity = await _unitOfWork.GroupRegistrations.GetLinkTypesByGroupId(group.RegistrationGroupId);
                    linkedEntity.ForEach(async item =>
                    {
                        await _unitOfWork.GroupRegistrations.DeleteLink(item.RegistrationGroupMembershipLinkId);
                    });
                    var list = new List<Registrationgroupmembershiplink>();
                    foreach (var membershipTypeId in group.MembershipTypeIds)
                    {
                        var model = new Registrationgroupmembershiplink();
                        model.RegistrationGroupId = group.RegistrationGroupId;
                        model.MembershipId = membershipTypeId;
                        list.Add(model);  
                        await _unitOfWork.GroupRegistrations.LinkMembership(model);
                        await _unitOfWork.CommitAsync();
                    }
                    //await _unitOfWork.GroupRegistrations.LinkMembership(list);
                    //await _unitOfWork.CommitAsync();
                }
                else
                {
                    var linkedEntity = await _unitOfWork.GroupRegistrations.GetLinkTypesByGroupId(group.RegistrationGroupId);
                    linkedEntity.ForEach(async item =>
                    {
                        await _unitOfWork.GroupRegistrations.DeleteLink(item.RegistrationGroupMembershipLinkId);
                    });
                    await _unitOfWork.CommitAsync();
                }

                return true;
            }
            catch (Exception ex)
            {

                throw;
            }
     
        }
        public async Task<IEnumerable<LinkEventGroupModel>> GetLinkEventModelForAllRegisterGroups(int eventId)
        {
            List<LinkEventGroupModel> linkEventGroupModelList = new List<LinkEventGroupModel>();
            try
            {
                List<Registrationgroup> registrationGroupList = new List<Registrationgroup>();
                registrationGroupList.AddRange(await GetRegisterGroups(""));
                var eventDetails = await _unitOfWork.Events.GetByIdAsync(eventId);

                foreach (var group in registrationGroupList)
                {
                    var linkEventGroup = await _unitOfWork.LinkEventGroups.GetLinkEventGroupByRegistrationGroupIdAndEventIdAsync(group.RegistrationGroupId, eventId);

                    LinkEventGroupModel linkEventGroupModel = new LinkEventGroupModel();
                    linkEventGroupModel.LinkEventGroupId = linkEventGroup != null ? linkEventGroup.LinkEventGroupId : 0;
                    linkEventGroupModel.EventId = eventId;
                    linkEventGroupModel.RegistrationGroupId = group.RegistrationGroupId;
                    linkEventGroupModel.EnableOnlineRegistration = linkEventGroup != null ? linkEventGroup.EnableOnlineRegistration : 0;
                    linkEventGroupModel.GroupName = group.Name;
                    linkEventGroupModel.IsGroupLinked = linkEventGroup != null ? 1 : 0;

                    //var registrationFeeTypes = await _unitOfWork.RegistrationFeeTypes.GetAllAsync();
                    if (linkEventGroup !=null && linkEventGroup.Linkregistrationgroupfees.Count > 0)
                    {                       
                        foreach (var fee in linkEventGroup.Linkregistrationgroupfees.OrderBy(x => x.RegistrationFeeTypeId))
                        {
                            LinkGroupRegistrationDatePriorityModel linkGroupRegistrationFeeModel = new LinkGroupRegistrationDatePriorityModel();
                            linkGroupRegistrationFeeModel.LinkRegistrationGroupFeeId = fee.LinkRegistrationGroupFeeId;
                            linkGroupRegistrationFeeModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                            linkGroupRegistrationFeeModel.RegistrationGroupId = group.RegistrationGroupId;
                            linkGroupRegistrationFeeModel.RegistrationGroupDateTime = fee.RegistrationGroupDateTime;
                            linkGroupRegistrationFeeModel.RegistrationGroupEndDateTime = fee.RegistrationGroupEndDateTime;
                            linkGroupRegistrationFeeModel.LinkEventGroupId = linkEventGroup.LinkEventGroupId;
                            linkEventGroupModel.GroupPriorityDateSettings.Add(linkGroupRegistrationFeeModel);
                        }
                    }
                    //else
                    //{
                    //    var registrationFees = await _unitOfWork.RegistrationFeeTypes.GetAllAsync();
                    //    foreach (var fee in registrationFees.Where(x => x.RegistrationFeeTypeName=="Regular"))
                    //    {
                    //        LinkGroupRegistrationDatePriorityModel linkGroupRegistrationFeeModel = new LinkGroupRegistrationDatePriorityModel();
                    //        linkGroupRegistrationFeeModel.LinkRegistrationGroupFeeId = 0;
                    //        linkGroupRegistrationFeeModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId;
                    //        linkGroupRegistrationFeeModel.RegistrationGroupId = group.RegistrationGroupId;
                    //        linkGroupRegistrationFeeModel.RegistrationGroupDateTime = eventDetails.FromDate != null ? eventDetails.FromDate : DateTime.Now;
                    //        linkGroupRegistrationFeeModel.RegistrationGroupEndDateTime = eventDetails.ToDate != null ? eventDetails.ToDate : DateTime.Now;
                    //        linkGroupRegistrationFeeModel.LinkEventGroupId = 0;
                    //        linkEventGroupModel.GroupPriorityDateSettings.Add(linkGroupRegistrationFeeModel);
                    //    }
                    //}

                    if(linkEventGroup!=null)
                    {
                        linkEventGroupModelList.Add(linkEventGroupModel);
                    }
                    else if(group.Status!=0)
                    { 
                        linkEventGroupModelList.Add(linkEventGroupModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return linkEventGroupModelList;
        }

        public async Task<bool> ValidateGroupName(RegistrationGroupModel groupModel)
        {
            if(groupModel.Name.Trim().ToUpper() == "MEMBER" || groupModel.Name.Trim().ToUpper() == "NON-MEMBER")
            {
                throw new InvalidOperationException($"'Member' and 'Non-Member' are reserved group names. Please use different name.");
            }
            var group = await _unitOfWork.GroupRegistrations.GetGroupByNameAsync(groupModel.Name);

            if (group != null)
            {
                //check if code already exists
                if (group.RegistrationGroupId != groupModel.RegistrationGroupId)
                {
                    if (group.Name.Trim() == groupModel.Name.Trim())
                    {
                        throw new InvalidOperationException($"Duplicate group name.");
                    }
                }
            }
            return true;
        }
    }
}
