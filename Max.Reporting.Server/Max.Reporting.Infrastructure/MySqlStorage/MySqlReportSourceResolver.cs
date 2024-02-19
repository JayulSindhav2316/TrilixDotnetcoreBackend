using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Telerik.Reporting;
using Telerik.Reporting.Services;

namespace Max.Reporting.Infrastructure.MySqlStorage
{
    public class MySqlReportSourceResolver : IReportSourceResolver
    {
        private readonly string repositoryDirectory;
        private readonly MySqlServerStorage _storage;
        private readonly IDataProtectorExtention _dataProtector;
        private readonly IMemoryCache _memoryCache;

        public MySqlReportSourceResolver(IConfiguration configuration, IMemoryCache memoryCache, IDataProtectorExtention dataProtector)            
        {
            _dataProtector = dataProtector;
            _storage = new MySqlServerStorage(configuration);
            _memoryCache = memoryCache;
        }

        public ReportSource Resolve(string report, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
        {            
            if (IsValidReportPath(report))
            {                
                if (_storage.ReportExists(Path.GetFileNameWithoutExtension(report)))
                {

                    var tenantId = _memoryCache.Get<string>("TenantId");

                    if (!string.IsNullOrEmpty(tenantId))
                    {
                        var tenantRCN = _memoryCache.Get<string>($"RCN-{tenantId}");
                        if (!string.IsNullOrEmpty(tenantRCN))
                        {
                            var reportDefinition = _storage.GetReportBytes(Path.GetFileNameWithoutExtension(report));
                            if (!string.IsNullOrEmpty(reportDefinition))
                            {
                                var connectionStringHandler = new ReportConnectionStringManager(_dataProtector.Decrypt(tenantRCN));
                                var sourceReportSource = new XmlReportSource { Xml = reportDefinition };
                                var reportSource = connectionStringHandler.UpdateReportSource(sourceReportSource);
                                return reportSource;
                            }
                            //return new XmlReportSource
                            //{
                            //    Xml = reportDefinition.Replace("﻿<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("DefaultReport", _dataProtector.Decrypt(tenantRCN))
                            //};
                        }
                    }                    
                }
            }

            return null;
        }
        private bool IsValidReportPath(string path)
        {
            return ReportDocumentUtils.IsSupportedReportDocument(path);
        }

        private string MapPath(string path)
        {
            if (!string.IsNullOrEmpty(repositoryDirectory))
            {
                return Path.Combine(repositoryDirectory, path);
            }

            return path;
        }
    }
}
