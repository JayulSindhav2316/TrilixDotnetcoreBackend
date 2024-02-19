using Max.Core;
using Max.Core.Models;
using Max.Data;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Import;
using Max.Services;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Serilog;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

string sourceConnectionString = "server=localhost;port=3306;database=trilix_bishop_live;uid=root;password=Anit1066";
string targetConnectionString = "server=localhost;port=3306;database=trilix_bishop_live;uid=root;password=Anit1066";
string tenantConnectionString = "server=localhost;port=3306;database=maxtenant;uid=root;password=Anit1066";

var serviceProvider = new ServiceCollection()
.AddDbContext<membermaxContext>(options => options
              .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
              .EnableSensitiveDataLogging(true)
              .EnableDetailedErrors(true)
              .UseMySql(sourceConnectionString, new MySqlServerVersion(new Version(5, 3, 34))))


.AddDbContext<maxtenantContext>(options => options
             .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
             .EnableSensitiveDataLogging(true)
             .EnableDetailedErrors(true)
             .UseMySql(tenantConnectionString, new MySqlServerVersion(new Version(5, 3, 34))))
.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
.AddSingleton<IUnitOfWork, UnitOfWork>()
.AddSingleton<ITenantUnitOfWork, TenantUnitOfWork>()
.AddSingleton<ITenantService, TenantService>()
.AddSingleton<IDocumentContainerService, DocumentContainerService>()
.AddSingleton<IConfigurationService, ConfigurationService>()
.AddSingleton<IAzureStorageService, AzureStorageService>()
.AddSingleton<ITagService, TagService>()
.AddMemoryCache()

.AddSolrNet<SolrDocumentModel>($"http://52.255.197.94:8983/solr/usccb_live")
.AddSingleton<ISolrIndexService<SolrDocumentModel>, SolrIndexService<SolrDocumentModel, ISolrOperations<SolrDocumentModel>>>()
.BuildServiceProvider();



var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
});
var mapper = config.CreateMapper();

var containerService = serviceProvider.GetService<IDocumentContainerService>();

using var sourceConnection = new MySqlConnection(sourceConnectionString);
sourceConnection.Open();

var tenantId = "83af4055-59b1-11ed-810a-000d3a1136df";

using var command = new MySqlCommand("SELECT * FROM trilix_bishop_live.solrexport where uploaded=0;", sourceConnection);
using var reader = command.ExecuteReader();

while (reader.Read())
{
    string solrId = reader.GetValue(0).ToString();
    string createdBy = reader.GetValue(1).ToString();
    string createdDate = reader.GetValue(2).ToString();
    string fileName = reader.GetValue(3).ToString();
    string text = reader.GetValue(4).ToString();

    Console.WriteLine($"\t\t Uploading {solrId} - {fileName}");
    var solrDocument = new SolrDocumentModel();

    solrDocument.Id = $"{tenantId}-{solrId}";
    solrDocument.TenantId = tenantId;
    solrDocument.CreatedBy = createdBy;
    solrDocument.FileName = fileName;
    solrDocument.CreatedDate = DateTime.Parse(createdDate);
    solrDocument.Text = text;
    try
    {
        if (containerService.UploadSolrDocument(solrDocument))
        {
            using var targetConnection = new MySqlConnection(targetConnectionString);
            targetConnection.Open();

            string updateSql = $"UPDATE solrexport Set Uploaded=1 WHERE SolrId={solrId};";
            MySqlCommand cmdUpdateEntity = new MySqlCommand(updateSql, targetConnection);
            cmdUpdateEntity.ExecuteNonQuery();
            targetConnection.Close();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\t Could not upload file :{solrId} - {fileName}");
    }
}
