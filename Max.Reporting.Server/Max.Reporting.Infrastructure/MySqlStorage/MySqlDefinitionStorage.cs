using Microsoft.Extensions.Configuration;
using Telerik.WebReportDesigner.Services;
using Telerik.WebReportDesigner.Services.Models;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    public class MySqlDefinitionStorage : MySqlDefinitionStorageBase, IDefinitionStorage,IAssetsStorage
    {      
        internal static readonly string[] reportFileExtensions = new string[3] { "*.trdx", "*.trdp", "*.trbp" };
        public new const string RootFolderName = "Reports";
        protected override string[] FileExtensions => reportFileExtensions;
        public MySqlDefinitionStorage(string baseDir, IConfiguration  configuration)
            : this(baseDir, new string[0], configuration)
        {            
        }

        public MySqlDefinitionStorage(string baseDir, string[] excludedFolders, IConfiguration configuration)
            : base(baseDir, new string[1] { "Reports" }, configuration)
        {
        }

        public override byte[] GetByUri(string uri)
        {
            return GetByUri<ReportNotFoundException>(uri);
        }

        public override Task<ResourceFileModel> RenameAsync(RenameResourceModel model)
        {
            return base.RenameAsync<InvalidReportNameException>(model);
        }

        public override Task<ResourceFileModel> SaveAsync(SaveResourceModel model, byte[] resource)
        {
            return base.SaveAsync<ReportNotFoundException>(model, resource);
        }
        protected override void ValidateDefinitionId(string definitionId)
        {
            definitionId.ValidateReportDefinitionId();
        }
    }
}
