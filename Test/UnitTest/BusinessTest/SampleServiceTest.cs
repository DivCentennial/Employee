using MariApps.MS.Purchase.MSA.Employee.Business;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Test.UnitTest.BusinessTest
{
    [TestClass]
    public class SampleServiceTest
    {
        [TestMethod]
        public void GetDataTest()
        {
            Mock<ISampleRepository> mockSampleRepository = new Mock<ISampleRepository>();
            mockSampleRepository.Setup(x => x.GetData()).Returns("Sample mock data");

            var services = new ServiceCollection();
            services.AddScoped<ISampleService, SampleService>(provider => new SampleService(mockSampleRepository.Object));

            var provider = services.BuildServiceProvider();

            var sampleService = provider.GetRequiredService<ISampleService>();
            var result = sampleService.GetData();

            Assert.AreEqual("Sample mock data", result);
        }
    }
}
