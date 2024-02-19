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
    class Program_pic
    {
        public static void Main_pic(string[] args)
        {

            MaindAsync(args).GetAwaiter().GetResult();

        }
        public static async Task MaindAsync(string[] args)
        {
            string sourceConnectionString = "server=localhost;port=3306;database=trilix_bishop;uid=root;password=Anit1066";
            string tenantConnectionString = "server=localhost;port=3306;database=maxtenant;uid=root;password=Anit1066";

            string imageRootPath = @"D:\Projects\MemberMax\Bishops\Photos";

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
            var documentRoot = Path.Combine($"/Documents/d3930c2d-4ac4-11ed-8023-38f3ab158646");
            var roonFolderPath = Path.Combine($"D:/Projects/MemberMax/Repository/MaxApi/MaxAPI/Max.Api/Documents/d3930c2d-4ac4-11ed-8023-38f3ab158646");
            foreach (var file in files)
            {
                Console.WriteLine($"Uploading profile picture: {file.Name} - {file.FullName}");

                //Get entityDetails
                
                var entityId = file.Name.Split(".")[0];
                Console.WriteLine($"EntityId: {entityId}");

                var entity = await unitOfWork.Entities.GetByIdAsync(int.Parse(entityId));

                if (entity != null)
                {

                }

                var personFolderPath = Path.Combine($"/EntityId-{entityId}");
                var mappedPath = Path.GetFullPath(roonFolderPath + personFolderPath);

                string extention = Path.GetExtension(file.Name);
                string fileName = Guid.NewGuid().ToString().Replace("-", "") + extention;

                string absolutePath = Path.GetFullPath(Path.Combine(mappedPath, fileName));

                // If directory does not exist, create it. 
                if (!Directory.Exists(mappedPath))
                {
                    Directory.CreateDirectory(mappedPath);
                }

                Stream fs = File.OpenRead(file.FullName);

                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {

                    Document document = new Document();
                    document.EntityId = int.Parse(entityId);
                    document.FilePath = $"{documentRoot}{personFolderPath}";
                    document.OrganizationId = organizationId;
                    document.FileName = fileName;
                    document.DisplayFileName = file.Name;
                    document.ContentType = GetContentType(extention);
                    document.Title = String.Empty;

                    await unitOfWork.Documents.AddAsync(document);

                    await unitOfWork.CommitAsync();
                    fs.CopyTo(stream);

                    entity.ProfilePictureId = document.DocumentId;
                    unitOfWork.Entities.Update(entity);
                    await unitOfWork .CommitAsync();
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
