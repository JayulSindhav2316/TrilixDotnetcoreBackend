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
using System.Threading.Tasks;

namespace Max.Import
{
    class Program_ncacc_image_import
    {
        public static void Main_image_import(string[] args)
        {

            MaindAsync(args).GetAwaiter().GetResult();

        }
        public static async Task MaindAsync(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=ncacc-demo;uid=root;password=Anit1066";
            string tenantConnectionString = "server=localhost;port=3306;database=maxtenant;uid=root;password=Anit1066";

            //string imageRootPath = @"D:\Projects\MemberMax\NCACC\Final-Import\ImageData\Commissioner";
            string imageRootPath = @"D:\Projects\MemberMax\NCACC\Final-Import\ImageData\County";

            int organizationId = 1;

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
            .AddSingleton<IDocumentService, DocumentService>()
            .AddSingleton<IEntityService, EntityService>()

            .BuildServiceProvider();

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();

            var dir = new DirectoryInfo(imageRootPath);

            Console.WriteLine("Uploading Profile Pictures");


            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);
            //var entityService = serviceProvider.GetService<IEntityService>();
            var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

            var currentPathName = string.Empty;
            var documentRoot = Path.Combine($"\\Documents\\b308eb4a-df61-11ed-b0ba-000d3a9cff9d");
            var roonFolderPath = Path.Combine($"D:/Projects/MemberMax/Repository/MaxApi/MaxAPI/Max.Api/Documents/b308eb4a-df61-11ed-b0ba-000d3a9cff9d");

            using var sourceConnection = new MySqlConnection(sourceConnectionString);
            sourceConnection.Open();

            using var command = new MySqlCommand("SELECT * FROM import_accountimages;", sourceConnection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string contactId = reader.GetValue(0).ToString();
                string fileName = reader.GetValue(1).ToString();
                string entityId = reader.GetValue(2).ToString();
               
                Console.WriteLine($"Uploading profile picture: {contactId} - {fileName} -{entityId}");

                var entity = await unitOfWork.Entities.GetByIdAsync(int.Parse(entityId));

                var personFolderPath = Path.Combine($"/EntityId-{entityId}");
                var mappedPath = Path.GetFullPath(roonFolderPath + personFolderPath);

              
                string imagFileName = Guid.NewGuid().ToString() + ".jpg";

                string absolutePath = Path.GetFullPath(Path.Combine(mappedPath, imagFileName));

                // If directory does not exist, create it. 
                if (!Directory.Exists(mappedPath))
                {
                    Directory.CreateDirectory(mappedPath);
                }

                var fileArray = fileName.Split('.');
                var compressedFile = $"{fileArray[0]}.jpg";
                string extention = Path.GetExtension(compressedFile);
                var inputfile = Path.Combine(imageRootPath, compressedFile);
                //change extension to jpg
                Stream fs = File.OpenRead(inputfile);

                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {

                    Document document = new Document();
                    document.EntityId = int.Parse(entityId);
                    document.FilePath = $"{documentRoot}{personFolderPath}";
                    document.OrganizationId = organizationId;
                    document.FileName = imagFileName;
                    document.DisplayFileName = compressedFile;
                    document.ContentType = GetContentType(extention);
                    document.Title = String.Empty;

                    await unitOfWork.Documents.AddAsync(document);

                    await unitOfWork.CommitAsync();
                    fs.CopyTo(stream);

                    entity.ProfilePictureId = document.DocumentId;
                    unitOfWork.Entities.Update(entity);
                    await unitOfWork.CommitAsync();
                }
            }

        }
        private static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}
