using MariApps.MS.Purchase.MSA.Employee.ApiService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Net;

namespace MariApps.MS.Purchase.MSA.Employee.Test.IntegrationTest
{
    [TestClass]
    public class HelloWorldTest 
    {
        [TestMethod]
        public async Task GetMe_should_return_200Success()
        {
            //Arrange
            var webAppFactory = new WebApplicationFactory<Program>();
            var httpClient = webAppFactory.CreateClient();

            //Act
            var response = await httpClient.GetAsync("/.about-me");
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JObject.Parse(responseString);
            
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("MariApps Marine Solutions", result["Company"]);
            Assert.AreEqual("pal-configuration-manager-api", result["Name"]);
            Assert.AreEqual("pal-configuration-manager-api", result["ApiKey"]);
        }

        [TestMethod]
        public async Task GetMe_Invalid_url_should_return_404NotFound()
        {
            //Arrange
            var webAppFactory = new WebApplicationFactory<Program>();
            var httpClient = webAppFactory.CreateClient();

            //Act
            var response = await httpClient.GetAsync("/.aboutme");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
