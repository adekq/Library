using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestLibrary.Client;
using System.Threading.Tasks;

namespace RESTLibrary.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public async Task ShouldLogin()
        {
            /*
            This not work at a moment. Need to mock ignite access / provide connection parameters.  
              
            const string expectedEmail = "admin@library.com";
            
            using var applicationFactory = new WebApplicationFactory<Program>();
            var httpClient = applicationFactory.CreateClient();
            var userClient = new UserClient(httpClient);

            LoginResponse response = await userClient.LoginAsync(new LoginRequest
            {
                Email = expectedEmail,
                Password = "pass"
            });

            Assert.AreEqual(expectedEmail, response.Email);
            */
        }
    }
}
