using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using System.Security.Cryptography;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Max.Data.Interfaces;
using System;
using Microsoft.AspNetCore.Http;

namespace Max.Services
{
    public class LinkEventFeeTypeService : ILinkEventFeeTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LinkEventFeeTypeService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper; ;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<LinkEventFeeTypeModel>> GetLinkedFeesByEventId(int eventId)
        {  
            var feeList = await _unitOfWork.LinkEventFeeTypes.GetFeeTypesByEventIdAsync(eventId);
            List<LinkEventFeeTypeModel> feeListModel = new List<LinkEventFeeTypeModel>();
            foreach (var fee in feeList)
            {
                LinkEventFeeTypeModel linkEventFeeTypeModel = new LinkEventFeeTypeModel();
                linkEventFeeTypeModel.EventId = fee.EventId??0;
                linkEventFeeTypeModel.RegistrationFeeTypeId = fee.RegistrationFeeTypeId??0;
                linkEventFeeTypeModel.RegistrationFeeTypeName = fee.RegistrationFeeType.RegistrationFeeTypeName;
                feeListModel.Add(linkEventFeeTypeModel);
            }

            return feeListModel;
        }
    }
}
