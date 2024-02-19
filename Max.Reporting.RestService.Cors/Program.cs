
using CSharp.Core31.Html5IntegrationDemo;
using Max.Reporting.RestService.Cors.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Telerik.Reporting.Services;
using Telerik.WebReportDesigner.Services;

EnableTracing();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddRazorPages()
    .AddNewtonsoftJson();

var connectionString = builder.Configuration.GetConnectionString("Default");
// Add services to the container.
builder.Services.AddDbContext<ReportContext>(options =>
{
    options.UseMySQL(connectionString);
});

var reportsPath = System.IO.Path.Combine(builder.Environment.ContentRootPath, "Reports");
// Configure dependencies for ReportsController.
builder.Services.TryAddSingleton<IReportServiceConfiguration>(sp =>
    new ReportServiceConfiguration
    {
        // The default ReportingEngineConfiguration will be initialized from appsettings.json or appsettings.{EnvironmentName}.json:
        ReportingEngineConfiguration = sp.GetService<IConfiguration>(),

        // In case the ReportingEngineConfiguration needs to be loaded from a specific configuration file, use the approach below:
        //ReportingEngineConfiguration = ResolveSpecificReportingConfiguration(sp.GetService<IWebHostEnvironment>()),
        HostAppId = "Max.Reporting.RestService.Cors",
        Storage = new MySqlServerStorage(connectionString),
        ReportSourceResolver = new TypeReportSourceResolver()
                                    .AddFallbackResolver(
                                        new UriReportSourceResolver(reportsPath))
    });

// Configure dependencies for ReportDesignerController.
builder.Services.TryAddSingleton<IReportDesignerServiceConfiguration>(sp => new ReportDesignerServiceConfiguration
{
    DefinitionStorage = new FileDefinitionStorage(reportsPath),
    //ResourceStorage = new ResourceStorage(
    //    System.IO.Path.Combine(builder.Environment.ContentRootPath, "Resources")),
    //SharedDataSourceStorage = new FileSharedDataSourceStorage(
    //    System.IO.Path.Combine(builder.Environment.ContentRootPath, "Shared Data Sources")),
    SettingsStorage = new FileSettingsStorage(reportsPath),
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ReportContext>();
    dbContext.Database.Migrate();
}

app.Run();

/// <summary>
/// Uncomment the lines to enable tracing in the current application.
/// The trace log will be persisted in a file named log.txt in the application root directory.
/// </summary>
static void EnableTracing()
{
    // System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(File.CreateText("log.txt")));
    // System.Diagnostics.Trace.AutoFlush = true;
}

/// <summary>
/// Loads a reporting configuration from a specific JSON-based configuration file.
/// </summary>
/// <param name="environment">The current web hosting environment used to obtain the content root path</param>
/// <returns>IConfiguration instance used to initialize the Reporting engine</returns>
static IConfiguration ResolveSpecificReportingConfiguration(IWebHostEnvironment environment)
{
    // If a specific configuration needs to be passed to the reporting engine, add it through a new IConfiguration instance.
    var reportingConfigFileName = System.IO.Path.Combine(environment.ContentRootPath, "reportingAppSettings.json");
    return new ConfigurationBuilder()
        .AddJsonFile(reportingConfigFileName, true)
        .Build();
}