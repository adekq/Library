using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RESTLibrary.Persisters;

namespace RESTLibrary.Tests
{
    [TestClass]
    public class IgnitePersisterTests
    {
        [TestMethod]
        public void ShouldInitialize()
        {
            var loggerMock = new Mock<ILogger<IgnitePersister>>();
            new IgnitePersister(loggerMock.Object, new Models.AdminLibrarian(), new IgniteClientConfiguration
            {
                Ip = "127.0.0.1",
                Port = "10800",
                DataRegion = "testDataRegion"
            });
        }
    }
}
