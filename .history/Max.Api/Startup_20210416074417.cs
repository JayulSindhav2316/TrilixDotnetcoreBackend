using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Data.DataModel;  
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Max.Core;
using Max.Data;
using Max.Services;
using Max.Core.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace Max.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string mySqlConnectionStr = Configuration.GetConnectionString("Default");  
            services.AddDbContextPool<membermaxContext>(options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));  
  
            services.AddControllers();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IStaffUserService, StaffUserService>();
            services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1", new OpenApiInfo { Title = "MaxApi", Version = "v1" });
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
            c.RoutePrefix = "";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Music V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
