using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace GatewayService.Tests
{
    [TestFixture]
    public class GatewayControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Test_Get_Test_Endpoint_Returns_Hello_Message()
        {
            // Arrange
            var url = "/api/Test";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(content, Is.EqualTo("Hello from Gataway!"));
        }
    }
}