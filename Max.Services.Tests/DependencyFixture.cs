using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Data;
using Max.Services.Interfaces;
using Max.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Serilog;
using Max.Core.Helpers;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Max.Services.Tests{
    
    public class DependencySetupFixture
    {
        private readonly IConfiguration Configuration;
        public DependencySetupFixture()
        {
            var serviceCollection = new ServiceCollection();
            //serviceCollection.AddDbContext<membermaxContext>(options => options.UseInMemoryDatabase(databaseName: "TestDatabase"));
            serviceCollection.AddDbContext<membermaxContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));            
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddTransient<IStaffUserService, StaffUserService>();
            serviceCollection.AddTransient<IRoleService, RoleService>();
            serviceCollection.AddTransient<IStaffRoleService, StaffRoleService>();
            serviceCollection.AddTransient<IAuthenticationService, AuthenticationService>();
            serviceCollection.AddTransient<IMembershipTypeService, MembershipTypeService>();
            serviceCollection.AddTransient<IMembershipFeeService, MembershipFeeService>();
            serviceCollection.AddTransient<IMembershipPeriodService, MembershipPeriodService>();
            serviceCollection.AddTransient<IMembershipCategoryService, MembershipCategoryService>();
            serviceCollection.AddTransient<IDepartmentService, DepartmentService>();
            serviceCollection.AddTransient<IGlAccountTypeService, GlAccountTypeService>();
            serviceCollection.AddTransient<IGlAccountService, GlAccountService>();
            serviceCollection.AddTransient<IPersonService, PersonService>();
            serviceCollection.AddScoped<ILookupService, LookupService>();
            serviceCollection.AddScoped<ICommunicationService, CommunicationService>();
            serviceCollection.AddScoped<IInvoiceService, InvoiceService>();
            serviceCollection.AddScoped<IInvoiceDetailService, InvoiceDetailService>();
            serviceCollection.AddScoped<IMembershipService, MembershipService>();
            serviceCollection.AddScoped<IMembershipHistoryService, MembershipHistoryService>();
            serviceCollection.AddScoped<IRelationService, RelationService>();
            serviceCollection.AddScoped<IRoleMenuService, RoleMenuService>();
            serviceCollection.AddScoped<IMenuService, MenuService>();
            serviceCollection.AddScoped<IShoppingCartService, ShoppingCartService>();
            serviceCollection.AddScoped<IAutoBillingDraftService, AutoBillingDraftService>();
            serviceCollection.AddScoped<IOrganizationService, OrganizationService>();
            serviceCollection.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
            serviceCollection.AddScoped<IAutoBillingNotificationService, AutoBillingNotificationService>();
            serviceCollection.AddScoped<IAutoBillingProcessingDatesService, AutoBillingProcessingDatesService>();
            serviceCollection.AddScoped<IAutoBillingService, AutoBillingService>();
            serviceCollection.AddScoped<IAutoBillingProcessingDatesService, AutoBillingProcessingDatesService>();
            serviceCollection.AddScoped<IAutoBillingSettingsService, AutoBillingSettingsService>();
            serviceCollection.AddScoped<IPaymentProfileService, PaymentProfileService>();
            serviceCollection.AddScoped<IReceiptDetailService, ReceiptDetailService>();
            serviceCollection.AddScoped<IReceiptHeaderService, ReceiptHeaderService>();
            serviceCollection.AddScoped<IBillingDocumentsService, BillingDocumentsService>();
            serviceCollection.AddScoped<INoteservice, NoteService>();
            serviceCollection.AddScoped<ITransactionService, TransactionService>();
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IDocumentService, DocumentService>();
            serviceCollection.AddScoped<INotificationService, NotificationService>();
            serviceCollection.AddScoped<IBillingService, BillingService>();
            serviceCollection.AddScoped<ICompanyService, CompanyService>();
            serviceCollection.AddScoped<IEntityService, EntityService>();
            serviceCollection.AddScoped<IReportService, ReportService>();
            serviceCollection.AddScoped<IReportParameterService, ReportparameterService>();
            serviceCollection.AddScoped<IReportSortOrderService, ReportSortOrderService>();
            serviceCollection.AddScoped<IReportFieldService, ReportFieldService>();
            serviceCollection.AddScoped<IMembershipConnectionService, MembershipConnectionService>();
            serviceCollection.AddScoped<IBillingFeeService, BillingFeeService>();
            serviceCollection.AddScoped<IPromoCodeService, PromoCodeService>();
            serviceCollection.AddScoped<ICookieService, CookieService>();
            serviceCollection.AddOptions();
            serviceCollection.AddScoped<CookieOptions>();
            serviceCollection.AddSingleton<IHostEnvironment, HostingEnvironment>();
            serviceCollection.AddTransient<IStaffSearchHistoryService, StaffSearchHistoryService>();
            serviceCollection.AddTransient<IGroupRegistrationService, GroupRegistrationService>();
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


            var configuration = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();

            Configuration = configuration;

            serviceCollection.Configure<AppSettings>(Configuration.GetSection("AppSettings"));


            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();
            serviceCollection.AddSingleton(mapper);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}
