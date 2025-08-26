
using MariApps.MS.Purchase.MSA.Employee.ApiService.Extensions;
using MariApps.Framework.MS.Core.Extensions;
using MariApps.Framework.MS.Core.Middlewares;

namespace MariApps.MS.Purchase.MSA.Employee.ApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.RegisterServiceDependecies();

            builder.InitMSCoreService();

            var app = builder.Build();

            app.ConfigureMsRequestPineline();

            app.Run();
        }
    }
}