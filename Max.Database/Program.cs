using System;
using Microsoft.Extensions.DependencyInjection;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Data;
using Max.Services.Interfaces;
using Max.Services;
using Microsoft.EntityFrameworkCore;

namespace Max.Database
{
    class Program
    {
        static void Main(string[] args)
        {

            var services = new ServiceCollection();
            services.AddDbContext<membermaxContext>(options => options.UseInMemoryDatabase(databaseName: "TestDatabase"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStaffUserService, StaffUserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStaffRoleService, StaffRoleService>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
           
             var roleService = serviceProvider.GetService<IRoleService>();

            Console.WriteLine("Max.Database!");
        }
    }
}
