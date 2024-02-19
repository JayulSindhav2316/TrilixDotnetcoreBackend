using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IAzureStorageService
    {
        Task<bool> CreateContainer(string name, int organizationId);
        Task<long> CreateBlob(int organizationId, string containerName, string fileName, Stream blob);
        Task<string> DownloadBlob(int organizationId, string containerName, string fileName, string localFilePath);
        Task<Boolean> DeleteBlob(int organizationId, string containerName, string fileName);
    }
}
