using Microsoft.Extensions.Configuration;
using System.Text;
using Telerik.WebReportDesigner.Services;
using Telerik.WebReportDesigner.Services.Models;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    public abstract class MySqlDefinitionStorageBase : ResourceStorage
    {
        protected abstract string[] FileExtensions { get; }
        private readonly MySqlServerStorage _storage;
        protected abstract void ValidateDefinitionId(string definitionId);

        public MySqlDefinitionStorageBase(string baseDir, string[] definitionFolders, IConfiguration configuration)
            : this(baseDir, definitionFolders, new string[0],configuration)
        {
        }

        public MySqlDefinitionStorageBase(string baseDir, string[] definitionFolders, string[] excludedFolders, IConfiguration configuration)
            : base(baseDir, definitionFolders.Select((string s) => new CreateFolderModel
            {
                Name = s
            }).ToArray(), excludedFolders)
        {
            _storage = new MySqlServerStorage(configuration);
        }

        public override IEnumerable<ResourceModelBase> GetFolderContents(string uri, string[] searchPattern)
        {
            return base.GetFolderContents(uri, FileExtensions);
        }

        protected virtual byte[] GetByUri<TDefinitionNotFoundException>(string uri) where TDefinitionNotFoundException : Exception
        {            
            ValidateDefinitionId(uri);
            var reportData = _storage.GetReportBytes(Path.GetFileNameWithoutExtension(uri));
            byte[] dataBuffer = Encoding.UTF8.GetBytes(reportData.Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", ""));
            return WrapException<byte[], TDefinitionNotFoundException, ResourceNotFoundException>(() => dataBuffer.ToArray());
        }

        protected virtual Task<ResourceFileModel> RenameAsync<TInvalidDefinitionNameException>(RenameResourceModel model) where TInvalidDefinitionNameException : Exception
        {
            return Task.FromResult(WrapException<ResourceFileModel, TInvalidDefinitionNameException, InvalidResourceNameException>(() => base.RenameAsync(model).Result));
        }

        protected virtual Task<ResourceFileModel> SaveAsync<TDefinitionNotFoundException>(SaveResourceModel model, byte[] resource) where TDefinitionNotFoundException : Exception
        {
            string resourceData = Encoding.UTF8.GetString(resource);
            _storage.SetReportDefinition(Path.GetFileNameWithoutExtension(model.Name), resourceData.Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", ""));
            return Task.FromResult(WrapException<ResourceFileModel, TDefinitionNotFoundException, ResourceNotFoundException>(() => Save(model, resource, overwrite: true, forcePath: true)));
        }

        public override async Task<ResourceFolderModel> GetFolderAsync(string uri)
        {
            ResourceFolderModel result = default(ResourceFolderModel);
            int num;
            try
            {
                result = await base.GetFolderAsync(uri);
                return result;
            }
            catch (ResourceFolderNotFoundException)
            {
                num = 1;
            }

            if (num != 1)
            {
                return result;
            }

            return await Task.FromResult<ResourceFolderModel>(null);
        }

        public override async Task<ResourceFileModel> GetModelAsync(string uri)
        {
            ResourceFileModel result = default(ResourceFileModel);
            int num;
            try
            {
                result = await base.GetModelAsync(uri);
                return result;
            }
            catch (ResourceNotFoundException)
            {
                num = 1;
            }

            if (num != 1)
            {
                return result;
            }

            return await Task.FromResult<ResourceFileModel>(null);
        }

        public override async Task DeleteAsync(string uri)
        {
            string[] pathParts;
            //string definitionId = UtilsFacade.FileSystemUtils.ExtractResourceName(uri, out pathParts);
            ValidateDefinitionId(uri);
            try
            {
                await base.DeleteAsync(uri);
            }
            catch (ResourceNotFoundException)
            {
            }

            await Task.CompletedTask;
        }

        public ResourceFileModel Save(SaveResourceModel model, byte[] resource, bool overwrite, bool forcePath)
        {
            ValidateDefinitionId(model.Name);
            if (overwrite)
            {
                string text = Path.Combine(model.ParentUri, model.Name);
                if (ResourceExists(text))
                {
                    OverwriteResourceModel model2 = new OverwriteResourceModel
                    {
                        Uri = text
                    };
                    return base.Overwrite(model2, resource);
                }
            }

            return base.SaveAsync(model, resource, forcePath).Result;
        }

        protected TResult WrapException<TResult, TDefinitionException, TResourceException>(Func<TResult> callback) where TDefinitionException : Exception where TResourceException : Exception
        {
            try
            {
                return callback();
            }
            catch (TResourceException val)
            {
                throw (TDefinitionException)Activator.CreateInstance(typeof(TDefinitionException), val.Message, val);
            }
        }
    }
}
