
using Max.Reporting.Application.Contracts.Persistence;
using Max.Reporting.Infrastructure.Persistence;
using Max.Reporting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Max.Reporting.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ReportContext>(options =>
            {
                options.UseMySQL(configuration.GetConnectionString("Default"));
            });

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportTemplateRepository, ReportTemplateRepository>();
            services.AddScoped<IReportCategoryRepository, ReportCategoryRepository>();
            services.AddScoped<IReportDefinitionRepository, ReportDefinitionRepository>();

            return services;
        }

    }
}
