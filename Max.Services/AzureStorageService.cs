using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Max.Core;
using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace Max.Services
{

    public class AzureStorageService : IAzureStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AzureStorageService> _logger;
        private readonly IMapper _mapper;
        private readonly ITenantService _tenantService;
        public AzureStorageService(IUnitOfWork unitOfWork, IMapper mapper, ITenantService tenantService,ILogger<AzureStorageService> logger)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._tenantService = tenantService;
            this._logger = logger;
        }

        public async Task<bool> CreateContainer(string containerName, int organizationId)
        {

            _logger.LogInformation("Creating conatiner");

            var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);

            string connectionString = tenant.StorageConnectionString;

            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName.ToLower());
                var container = await containerClient.CreateAsync();
                _logger.LogInformation($"createContainer(): {container.ToString()}");
                return true;
            }
            catch( Exception ex)
            {
                _logger.LogError($"createContainer(): {ex.Message}");
            }
            return false;
        }

        public async Task<long> CreateBlob(int organizationId, string containerName, string fileName, Stream fileBlob)
        {
            var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(tenant.StorageConnectionString, containerName);
                containerClient.CreateIfNotExists();
                // Get a reference to a blob
                BlobClient blob = containerClient.GetBlobClient(fileName);

                // Upload file data
                await blob.UploadAsync(fileBlob);

                // Verify we uploaded some content
                BlobProperties properties = await blob.GetPropertiesAsync();
               
                return(properties.ContentLength);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreateBlob(): {ex.Message}");
                throw new Exception("failed to upload the file.");
            }
        }

        public async Task<string> DownloadBlob(int organizationId, string containerName, string fileName, string localFilePath)
        {
            var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(tenant.StorageConnectionString, containerName);
                containerClient.CreateIfNotExists();
                // Get a reference to a blob
                BlobClient blob = containerClient.GetBlobClient(fileName);

                await blob.DownloadToAsync(localFilePath);

                //Validate file size??

                return (localFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"DonwloadBlob(): {ex.Message}");
                throw new Exception("failed to Download  the file.");
            }
        }

        public async Task<Boolean> DeleteBlob(int organizationId, string containerName, string fileName)
        {
            var organization = await _unitOfWork.Organizations.GetOrganizationByIdAsync(organizationId);
            var tenant = await _tenantService.GetTenantByOrganizationName(organization.Name);
            try
            {
                BlobContainerClient containerClient = new BlobContainerClient(tenant.StorageConnectionString, containerName);
                
                if(containerClient != null)
                {
                    BlobClient blob = containerClient.GetBlobClient(fileName);
                    blob.DeleteIfExists();
                    return true;
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete(): {ex.Message}");
                throw new Exception("failed to Delete the file.");
            }
            return false;
        }
    }
}