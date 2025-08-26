using MariApps.Framework.Core.Abstractions.Contracts;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.DbContext;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using MariApps.MS.Purchase.MSA.Employee.Repository.DbContext;
using MariApps.MS.Purchase.MSA.Employee.Repository.Repositories;
using MariApps.MS.Purchase.MSA.Employee.Test.UnitTest.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Test.UnitTest
{
    [TestClass]
    public class SampleRepositoryTest
    {
        [TestMethod]
        public void GetSampleDataTest()
        {
            //Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IConfiguration>(AppSettings.GetConfiguration());

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
            
            //Act
            var serviceProvider = services.BuildServiceProvider();

            var sampleRepository = serviceProvider.GetRequiredService<ISampleRepository>();
            var result = sampleRepository.GetData();

            //Assert
            Assert.AreEqual("Sample Data", result);
        }
    }
}
