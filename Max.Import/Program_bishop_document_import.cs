using Max.Core;
using Max.Core.Models;
using Max.Data;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services;
using Max.Services.Interfaces;
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

namespace Max.Import
{
    class Programd
    {
        static async System.Threading.Tasks.Task Maind(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=trilix_bishop;uid=root;password=Anit1066";
            string tenantConnectionString = "server=localhost;port=3306;database=maxtenant;uid=root;password=Anit1066";
            string rootPath = Path.Combine(@"E:/Data/Bishops/Bishopsonly");
            var confidentialOntainer = new DocumentContainerModel();
            var nonConfidentialOntainer = new DocumentContainerModel();

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
            .AddSolrNet<SolrDocumentModel>($"http://40.117.81.255:8983/solr/usccb")
            .AddSingleton<ISolrIndexService<SolrDocumentModel>, SolrIndexService<SolrDocumentModel, ISolrOperations<SolrDocumentModel>>>()
           
            .BuildServiceProvider();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();

            var containerService = serviceProvider.GetService<IDocumentContainerService>();


            var dir = new DirectoryInfo(rootPath);

            string[] containers = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);

            //Create a confidential container

            var containerRequest = new ContainerRequestModel();
            //containerRequest.Name = "confidential"; 
            //containerRequest.Description = "Contains all classified documents";
            //containerRequest.UserId = 1;
            //confidentialOntainer = await containerService.CreateDocumentContainer(containerRequest);

            //Create a non-confidential container

            containerRequest = new ContainerRequestModel();
            containerRequest.Name = "non-confidential";
            containerRequest.Description = "Contains all non-classified documents";
            containerRequest.UserId = 1;
            nonConfidentialOntainer = await containerService.CreateDocumentContainer(containerRequest);

            Console.WriteLine("Uploading confidential documents");

            ////Create Sub folder and upload files.

            //var conatinerPath = rootPath + "\\confidential";

            //var containerDirectory = new DirectoryInfo(conatinerPath);

            //FileInfo[] files = containerDirectory.GetFiles("*", SearchOption.AllDirectories);

            //var currentPathName = string.Empty;

            //foreach (var file in files)
            //{
            //    //Get folder name

            //    var folderName = file.DirectoryName.ReplaceFirst(rootPath + "\\", "").Replace("\\", "/");
            //    if (folderName == "confidential")
            //    {
            //        //Upload file in root folder
            //        currentPathName = "confidential";
            //    }
            //    else
            //    {
            //        Console.WriteLine($"\t{folderName}");
            //        //check if folder exists

            //        //Seprate Path & File Name

            //        string[] paths = folderName.Split("/");

            //        var pathNameArray = paths.SkipLast(1).ToArray();
            //        var pathName = String.Join("/", pathNameArray);
            //        var fileName = paths[paths.Length - 1];

            //        var folders = await containerService.GetFoldersByContainerAndPath(confidentialOntainer.ContainerId, pathName);

            //        if (folders.Count == 0 || !folders.Any(x => x.FileName == fileName))
            //        {
            //            Console.WriteLine($"\t Creating Folder :{fileName}");
            //            var folderObject = new DocumentObjectModel();
            //            folderObject.PathName = pathName;
            //            folderObject.FileName = fileName;
            //            folderObject.FileType = 0;
            //            folderObject.CreatedBy = 1;
            //            folderObject.CreatedDate = DateTime.Now;
            //            folderObject.ContainerId = confidentialOntainer.ContainerId;

            //            try
            //            {
            //                var currentFolder = await containerService.CreateFolder(folderObject);
            //                currentPathName = pathName + "/" + fileName;
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine($"\t Could create folder :{fileName}");
            //            }

            //        }

            //    }

            //    if(file.Extension.ToUpper() == ".SHTML" || file.Extension.ToUpper() == ".HTML")
            //    {
            //        Console.WriteLine($"\t\t Skipping  {file.FullName} - {file.Length}");
            //        continue;
            //    }
            //    Console.WriteLine($"\t\t Uploading {file.FullName} - {file.Length}");
            //    var documentObject = new DocumentObjectModel();
            //    documentObject.ContainerId = confidentialOntainer.ContainerId;
            //    documentObject.FileName = file.Name;
            //    documentObject.PathName = currentPathName;
            //    documentObject.FileType = 1;
            //    documentObject.CreatedBy = 1;
            //    documentObject.CreatedDate = DateTime.Now;
            //    documentObject.OrganizationId = 1;
            //    documentObject.SelectedTags = new List<SelectListModel>();
            //    try
            //    {
            //        var document = await containerService.UploadLocalFile(rootPath, file.FullName, documentObject);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"\t Could not upload file :{file.FullName}");
            //    }

            //}

            // Upload all non confidential files 

            var conatinerPath = rootPath + "\\non-confidential";

            var containerDirectory = new DirectoryInfo(conatinerPath);

            var files = containerDirectory.GetFiles("*", SearchOption.AllDirectories);

            var currentPathName = string.Empty;

            foreach (var file in files)
            {
                //Get folder name

                var folderName = file.DirectoryName.ReplaceFirst(rootPath + "\\", "").Replace("\\", "/");
                if (folderName == "non-confidential")
                {
                    //Upload file in root folder
                    currentPathName = "non-confidential";
                }
                else
                {
                    Console.WriteLine($"\t{folderName}");
                    //check if folder exists

                    //Seprate Path & File Name

                    string[] paths = folderName.Split("/");

                    var pathNameArray = paths.SkipLast(1).ToArray();
                    var pathName = String.Join("/", pathNameArray);
                    var fileName = paths[paths.Length - 1];

                    Console.WriteLine($"\t{pathName}-{fileName}");

                    var folders = await containerService.GetFoldersByContainerAndPath(nonConfidentialOntainer.ContainerId, pathName);

                    if (folders.Count == 0 || !folders.Any(x => x.FileName == fileName))
                    {
                        Console.WriteLine($"\t Creating Folder :{fileName}");
                        var folderObject = new DocumentObjectModel();
                        folderObject.PathName = pathName;
                        folderObject.FileName = fileName;
                        folderObject.FileType = 0;
                        folderObject.CreatedBy = 1;
                        folderObject.CreatedDate = DateTime.Now;
                        folderObject.ContainerId = nonConfidentialOntainer.ContainerId;

                        try
                        {
                            var currentFolder = await containerService.CreateFolder(folderObject);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\t Could not create Folder :{fileName}");
                        }
                        currentPathName = Path.Combine(pathName , fileName);

                    }

                }
                if (file.Extension.ToUpper() == ".SHTML" || file.Extension.ToUpper() == ".HTML")
                {
                    Console.WriteLine($"\t\t Skipping  {file.FullName} - {file.Length}");
                    continue;
                }
                Console.WriteLine($"\t\t Uploading {file.FullName} - {file.Length}");
                var documentObject = new DocumentObjectModel();
                documentObject.ContainerId = nonConfidentialOntainer.ContainerId;
                documentObject.FileName = file.Name;
                documentObject.PathName = currentPathName;
                documentObject.FileType = 1;
                documentObject.CreatedBy = 1;
                documentObject.CreatedDate = file.CreationTime;
                documentObject.OrganizationId = 1;
                documentObject.SelectedTags = new List<SelectListModel>();
                try
                {
                    var document = await containerService.UploadLocalFile(rootPath, file.FullName, documentObject);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\t Could not upload file :{file.FullName}");
                }
            }

        }

    }
}
