using Max.Api.Helpers;
using Max.Core.Helpers;
using Max.Data;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using SolrNet;
using Max.Core.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;

namespace Max.Api
{
    public class Startup
    {
        readonly string allowSpecificOrigins = "_allowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
           
            string mySqlConnectionString = Configuration.GetConnectionString("Default");
            string tenantConnectionString = Configuration.GetConnectionString("Tenant");

            services.AddDbContext<maxtenantContext>(options => options
                         .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
                         .EnableSensitiveDataLogging(true)
                         .EnableDetailedErrors(true)
                         .UseMySql(tenantConnectionString, ServerVersion.AutoDetect(tenantConnectionString)));

            // Not required but has been configured to all swagger API to work. Shall be removed from Production
            // as it is being dynamically set through host headers
            services.AddDbContext<membermaxContext>(options => options
                          .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()))
                          .EnableSensitiveDataLogging(true)
                          .EnableDetailedErrors(true)
                          .UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString),
                                options => options.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: System.TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null)));

            // Use the following definition in production
            //services.AddDbContext<membermaxContext>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



            services.AddCors(options =>
            {
                options.AddPolicy(allowSpecificOrigins,
                        builder =>
                        {
                            builder.WithOrigins("https://localhost:4200", "http://localhost:4200", "https://localhost:44332",
                                                  "https://lightingbolt.azurewebsites.net", "https://maxweb.membermax.com",
                                                  "https://lightingbolt.membermax.com",
                                                  "https://trilixqa.membermax.com",
                                                  "https://trilixpp.membermax.com",
                                                  "https://trilix.membermax.com",
                                                  "https://trilix-qa-reportapi.membermax.com")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .SetIsOriginAllowed(origin => true)
                                    .AllowCredentials();
                        });
            });
            const int maxRequestLimit = 209715200;
            // If using IIS
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = maxRequestLimit;
            });
            // If using Kestrel
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = maxRequestLimit;
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = maxRequestLimit;
                x.MultipartBodyLengthLimit = maxRequestLimit;
                x.MultipartHeadersLengthLimit = maxRequestLimit;
            });
            services.AddControllers().AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Documents")));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddOptions();
            services.AddSolrNet<SolrDocumentModel>($"http://40.117.81.255:8983/solr/trilixcore");
            services.AddScoped<ISolrIndexService<SolrDocumentModel>, SolrIndexService<SolrDocumentModel, ISolrOperations<SolrDocumentModel>>>();
            services.AddScoped<CookieOptions>();
           
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITenantUnitOfWork, TenantUnitOfWork>();
            services.AddScoped<IStaffUserService, StaffUserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IMembershipTypeService, MembershipTypeService>();
            services.AddScoped<IMembershipFeeService, MembershipFeeService>();
            services.AddScoped<IMembershipPeriodService, MembershipPeriodService>();
            services.AddScoped<IMembershipCategoryService, MembershipCategoryService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IGlAccountTypeService, GlAccountTypeService>();
            services.AddScoped<IGlAccountService, GlAccountService>();
            services.AddScoped<IRoleMenuService, RoleMenuService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<ICommunicationService, CommunicationService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IInvoiceDetailService, InvoiceDetailService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<IMembershipHistoryService, MembershipHistoryService>();
            services.AddScoped<IRelationService, RelationService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<IAuthNetService, AuthNetService>();
            services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();
            services.AddScoped<IPaymentProcessorService, PaymentProcessorService>();
            services.AddScoped<IReceiptHeaderService, ReceiptHeaderService>();
            services.AddScoped<IAutoBillingProcessingDatesService, AutoBillingProcessingDatesService>();
            services.AddScoped<IAutoBillingSettingsService, AutoBillingSettingsService>();
            services.AddScoped<IPaymentProfileService, PaymentProfileService>();
            services.AddScoped<IAutoBillingDraftService, AutoBillingDraftService>();
            services.AddScoped<IAutoBillingNotificationService, AutoBillingNotificationService>();
            services.AddScoped<IAutoBillingService, AutoBillingService>();
            services.AddScoped<IBillingDocumentsService, BillingDocumentsService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IPaperInvoiceService, PaperInvoiceService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<IReceiptDetailService, ReceiptDetailService>();
            services.AddScoped<IRefundService, RefundService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<INoteservice, NoteService>();
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IPromoCodeService, PromoCodeService>();
            services.AddScoped<IRelationshipService, RelationshipService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IEntityService, EntityService>();
            services.AddScoped<IMembershipConnectionService, MembershipConnectionService>();
            services.AddScoped<IResetPasswordService, ResetPasswordService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupRoleService, GroupRoleService>();
            services.AddScoped<ILinkGroupRoleService, LinkGroupRoleService>();
            services.AddScoped<IGroupMemberService, GroupMemberService>();
            services.AddScoped<IGroupMemberRoleService, GroupMemberRoleService>();
            services.AddScoped<IBillingFeeService, BillingFeeService>();
            services.AddScoped<IVoidService, VoidService>();
            services.AddScoped<IWriteOffService, WriteOffService>();
            services.AddScoped<IGrouphistoryService, GrouphistoryService>();
            services.AddScoped<IStaffSearchHistoryService, StaffSearchHistoryService>();
            services.AddScoped<IClientLogService, ClientLogService>();
            services.AddScoped<IDocumentContainerService, DocumentContainerService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IQuestionBankService, QuestionBankService>();
            services.AddScoped<IAnswerOptionService, AnswerOptionService>();
            services.AddScoped<IGroupRegistrationService, GroupRegistrationService>();
            services.AddScoped<ISociableService, SociableService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IQuestionLinkService, QuestionLinkService>();
            services.AddScoped<ILinkEventFeeTypeService, LinkEventFeeTypeService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IStateService, StateService>();
            services.AddScoped<IContactRoleService, ContactRoleService>();
            services.AddScoped<IEntityRoleService, EntityRoleService>();
            services.AddScoped<IContactActivityService, ContactActivityService>();
            services.AddScoped<ICustomFieldService, CustomFieldService>();
            services.AddScoped<IContactTokenService, ContactTokenService>();
            //services.AddScoped<IAuditHistoryService, AuditHistoryService>();
            services.AddScoped<ISociableGroupService, SociableGroupService>();
            services.AddScoped<IOpportunityService, OpportunityService>();            services.AddScoped<IBillingCycleNotificationService, BillingCycleNotificationService>();
            services.AddScoped<ICommanService, CommanService>();

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MaxAPI", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                            {
                                new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"
                                        }
                                    },
                                    new string[] {}

                            }
                });
            });
            // Add exception logging on model binding exceptions
            ConfigureModelBindingExceptionHandling(services);

            // Add JWT Token Authentication & Authorization
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Secret").Value)),

                };
            });
            services.AddMvc()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
          
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Images")),
                RequestPath = new PathString("/app-images")
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Documents")),
                RequestPath = new PathString("/documents")
            });
            app.UseHttpsRedirection();
            //app.UseMiddleware<OptionsMiddleware>();
            app.UseRouting();
            app.UseCors(allowSpecificOrigins);
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MaxApi V1");
            });
            app.UseTenantMiddleware();
            app.UseSession();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseGlobalExceptionMiddleware();
            app.UseMiddleware<JwtMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private void ConfigureModelBindingExceptionHandling(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    ValidationProblemDetails error = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new ValidationProblemDetails(actionContext.ModelState)).FirstOrDefault();

                    Log.Error($"{{@RequestPath}} received invalid message format: {{@Exception}}",
                    actionContext.HttpContext.Request.Path.Value,
                    error.Errors.Values);
                    return new BadRequestObjectResult(error);
                };
            });
        }
    }
}
