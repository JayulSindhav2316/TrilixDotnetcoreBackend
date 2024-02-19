using Max.Core.Helpers;
using Max.Core.Models;
using Max.Data;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SolrNet;

namespace Max.Billing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
             .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
             .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
             .Build();
            // Create the logger
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .ReadFrom.Configuration(configuration)
               .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);

            // Add this line to be able to run as a windows service
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-3.1&tabs=visual-studio
            host.UseWindowsService();
            host.UseSerilog();

            host.ConfigureAppConfiguration(
                  (hostContext, config) =>
                  {
                      config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                      config.AddJsonFile("appsettings.json", false, true);
                      config.AddCommandLine(args);
                  }
            );



            host.ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

                services.AddHostedService<Worker>();
                services.AddApplicationInsightsTelemetryWorkerService();

                //services.AddDbContext<membermaxContext>(options => options
                //              .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
                //              .EnableSensitiveDataLogging(true)
                //              .UseMySql(hostContext.Configuration.GetConnectionString("Default"), new MySqlServerVersion(new Version(5, 3, 34))));

                string tenantConnectionString = hostContext.Configuration.GetConnectionString("Tenant");

                services.AddDbContext<maxtenantContext>(options => options
                             .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
                             .EnableSensitiveDataLogging(true)
                             .EnableDetailedErrors(true)
                             .UseMySql(tenantConnectionString, new MySqlServerVersion(new Version(5, 3, 34))));

                services.AddDbContext<membermaxContext>();
                services.AddMemoryCache();
                // Use the following definition in production
                //services.AddDbContext<membermaxContext>();
                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<ITenantUnitOfWork, TenantUnitOfWork>();
                services.AddScoped<IMembershipTypeService, MembershipTypeService>();
                services.AddScoped<IMembershipFeeService, MembershipFeeService>();
                services.AddScoped<IMembershipPeriodService, MembershipPeriodService>();
                services.AddScoped<IMembershipCategoryService, MembershipCategoryService>();
                services.AddScoped<IGlAccountTypeService, GlAccountTypeService>();
                services.AddScoped<IGlAccountService, GlAccountService>();
                services.AddScoped<IPersonService, PersonService>();
                services.AddScoped<IInvoiceService, InvoiceService>();
                services.AddScoped<IInvoiceDetailService, InvoiceDetailService>();
                services.AddScoped<IMembershipService, MembershipService>();
                services.AddScoped<IMembershipHistoryService, MembershipHistoryService>();
                services.AddScoped<IAuthNetService, AuthNetService>();
                services.AddScoped<IShoppingCartService, ShoppingCartService>();
                services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
                services.AddScoped<IPaymentProcessorService, PaymentProcessorService>();
                services.AddScoped<IPaymentProfileService, PaymentProfileService>();
                services.AddScoped<IReceiptHeaderService, ReceiptHeaderService>();
                services.AddScoped<IAutoBillingService, AutoBillingService>();
                services.AddScoped<IAuthNetService, AuthNetService>();
                services.AddScoped<IAutoBillingDraftService, AutoBillingDraftService>();
                services.AddScoped<IBillingDocumentsService, BillingDocumentsService>();
                services.AddScoped<IBillingService, BillingService>();
                services.AddScoped<IPaperInvoiceService, PaperInvoiceService>();
                services.AddScoped<ITransactionService, TransactionService>();
                services.AddScoped<IAuthNetDraftService, AuthNetDraftService>();
                services.AddScoped<IPaymentProcessorService, PaymentProcessorService>();
                services.AddScoped<IAutoBillingNotificationService, AutoBillingNotificationService>();
                services.AddScoped<IAutoBillingSettingsService, AutoBillingSettingsService>();
                services.AddScoped<INotificationService, NotificationService>();
                services.AddScoped<IDocumentService, DocumentService>();
                services.AddScoped<IEmailService, EmailService>();
                services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();
                services.AddScoped<INotificationService, NotificationService>();
                services.AddScoped<IReceiptDetailService, ReceiptDetailService>();
                services.AddScoped<IRefundService, RefundService>();
                services.AddScoped<IRelationService, RelationService>();
                services.AddScoped<ITenantService, TenantService>();
                services.AddScoped<IPromoCodeService, PromoCodeService>();
                services.AddScoped<IRelationshipService, RelationshipService>();
                services.AddScoped<IReportService, ReportService>();
                services.AddScoped<IItemService, ItemService>();
                services.AddScoped<ICompanyService, CompanyService>();
                services.AddScoped<IEntityService, EntityService>();
                services.AddScoped<IOrganizationService, OrganizationService>();
                services.AddScoped<IDocumentContainerService, DocumentContainerService>();
                services.AddScoped<ISociableService, SociableService>();
                services.AddScoped<IStaffUserService, StaffUserService>();
                services.AddScoped<IAzureStorageService, AzureStorageService>();
                services.AddScoped<IStaffUserService, StaffUserService>();
                services.AddSolrNet<SolrDocumentModel>($"http://40.117.81.255:8983/solr/usccb_demo");
                services.AddScoped<ISolrIndexService<SolrDocumentModel>, SolrIndexService<SolrDocumentModel, ISolrOperations<SolrDocumentModel>>>();
                services.AddScoped<ISociableGroupService, SociableGroupService>();
                services.AddScoped<IEntityRoleService, EntityRoleService>();
                services.AddScoped<ICountryService, CountryService>();
                services.AddScoped<IStateService, StateService>();
                services.AddScoped<IContactRoleService, ContactRoleService>();
                services.AddScoped<IEntityRoleService, EntityRoleService>();
                services.AddScoped<IContactActivityService, ContactActivityService>();
                services.AddScoped<ICustomFieldService, CustomFieldService>();
                services.AddScoped<IContactTokenService, ContactTokenService>();
                //services.AddScoped<IAuditHistoryService, AuditHistoryService>();
                services.AddScoped<ISociableGroupService, SociableGroupService>();
                services.AddScoped<IOpportunityService, OpportunityService>();
                services.AddScoped<IBillingCycleNotificationService, BillingCycleNotificationService>();
                var config = new AutoMapper.MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new MappingProfile());
                });
                var mapper = config.CreateMapper();
                services.AddSingleton(mapper);
                services.AddOptions();
            });

            return host;
        }
    }
}
