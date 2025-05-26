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
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5004");
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            _client.Dispose();
        }

        [Test]
        public async Task Test_Get_Test_Endpoint_Returns_Hello_Message()
        {
            // Arrange /
            var url = "/api/Test";

            // Act
            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(1, Is.EqualTo(1));
            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(content, Is.EqualTo("Hello from Gataway!"));
        }
    }
}