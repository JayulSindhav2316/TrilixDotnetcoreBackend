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
using Serilog;

namespace Max.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        static readonly ILogger _logger = Serilog.Log.ForContext<ConfigurationService>();
        public ConfigurationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;

        }

        public async Task<Configuration> GetConfigurationByOrganizationId(int organizationId)
        {
            return await _unitOfWork.Configurations.GetConfigurationByOrganizationIdAsync(organizationId);
        }

        public async Task<ConfigurationModel> UpdateConfiguration(ConfigurationModel model)
        {
            var configuration = new Configuration();

            if(model.ConfigurationId > 0)
            {
                configuration = await _unitOfWork.Configurations.GetByIdAsync(model.ConfigurationId);

                configuration.OrganizationId = model.OrganizationId;
                configuration.DocumentAccessControl = model.DocumentAccessControl;
                configuration.ContactDisplayTabs = model.ContactDisplayTabs;
                configuration.ContactDisplayMembership = model.ContactDisplayMembership;
                _unitOfWork.Configurations.Update(configuration);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                configuration.OrganizationId = model.OrganizationId;
                configuration.DocumentAccessControl = model.DocumentAccessControl;
                configuration.ContactDisplayTabs = model.ContactDisplayTabs;
                configuration.ContactDisplayMembership = model.ContactDisplayMembership;
                await _unitOfWork.Configurations.AddAsync(configuration);
                await _unitOfWork.CommitAsync();
            }

            var configurationModel = _mapper.Map<ConfigurationModel>(configuration);
            return configurationModel;
        }

    }
}
