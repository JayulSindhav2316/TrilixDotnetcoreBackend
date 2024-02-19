using AutoMapper;
using Max.Core.Models;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class StateService : IStateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<StateService> _logger;

        public StateService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<StateService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<StateModel>> GetStatesByCountryIdAsync(int countryId)
        {
            var states = await _unitOfWork.StateRepository.GetStatesByCountryIdAsync(countryId);

            return _mapper.Map<List<StateModel>>(states);
        }
    }
}
