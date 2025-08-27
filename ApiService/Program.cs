using MariApps.MS.Purchase.MSA.Employee.ApiService.Extensions;
using MariApps.MS.Purchase.MSA.Employee.Business;
using MariApps.MS.Purchase.MSA.Employee.Business.Contracts;
using MariApps.MS.Purchase.MSA.Employee.Repository.Repositories;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using MariApps.Framework.MS.Core.Extensions;
using MariApps.Framework.MS.Core.Middlewares;

namespace MariApps.MS.Purchase.MSA.Employee.ApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register your repository and service dependencies
            builder.Services.AddScoped<IEmployeeRepository>(sp =>
            {
                var connString = builder.Configuration.GetConnectionString("PALConnectionString");
                return new EmployeeRepository(connString);
            });
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            // Your existing extension methods for other services
            builder.Services.RegisterServiceDependecies();
            builder.InitMSCoreService();

            var app = builder.Build();

            // Middleware pipeline
            app.ConfigureMsRequestPineline();

            app.Run();
        }
    }
}
