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
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryService> _logger;

        public CountryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CountryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CountryModel>> GetAllCountriesAsync()
        {
            var countries = await _unitOfWork.CountryRepository.GetAllAsync();

            return _mapper.Map<List<CountryModel>>(countries);
        }
    }
}
