using MariApps.Framework.Core.Abstractions.Contracts;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.DbContext;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using MariApps.MS.Purchase.MSA.Employee.Repository.DbContext;
using MariApps.MS.Purchase.MSA.Employee.Repository.Repositories;

namespace MariApps.MS.Purchase.MSA.Employee.ApiService.Extensions
{
    public static class DependencyRegistration
    {
        public static void RegisterServiceDependecies(this IServiceCollection services)
        {
            services.AddScoped<IAdoDbContext, AdoDbContext>(provider =>
            {
                IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
                ILogger<AdoDbContext> logger = provider.GetRequiredService<ILogger<AdoDbContext>>();
                IAuditLogsOrchestrator auditlog = provider.GetRequiredService<IAuditLogsOrchestrator>();
                IRequestCorrelationIdAccessor correlationIdAccessor = provider.GetRequiredService<IRequestCorrelationIdAccessor>();

                string? palConnectionString = configuration.GetConnectionString("PALConnectionString");

                if (string.IsNullOrEmpty(palConnectionString))
                    throw new ArgumentNullException("Connection string not found");

                return new AdoDbContext(palConnectionString, logger, auditlog, correlationIdAccessor);
            });

            services.AddScoped<ISampleRepository, SampleRepository>();
        }
    }
}
