using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;

using xero.Models;
using xero.Test.Tools;

namespace xero.Test
{
    [Collection("Sequential")]
    public class StatsTest 
        : IClassFixture<WebApplicationFactory<xero.Startup>>
    {
        private readonly WebApplicationFactory<xero.Startup> _factory;

        private readonly ITestOutputHelper _output;

        public StatsTest(WebApplicationFactory<xero.Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        string statsBaseUrl = "/stats";

        [Fact]
        public async Task Test_ExistingStats()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(statsBaseUrl);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());

            string jsonBody = await response.Content.ReadAsStringAsync();
            var stats = JsonConvert.DeserializeObject<Stat[]>(jsonBody);
            Assert.Equal(1, stats.Length);
        }

        [Fact]
        public async Task Test_NewStat() {

            string email = "b@a.com";

            // Arrange
            var client = _factory.CreateClient();
            await UploadHelper.Upload(client, email, "invoice-3.pdf");
            await UploadHelper.Upload(client, email, "invoice-4.pdf");

            // Act
            var response = await client.GetAsync(statsBaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            string jsonBody = await response.Content.ReadAsStringAsync();
            var stats = JsonConvert.DeserializeObject<Stat[]>(jsonBody);
            Assert.Equal(2, stats.Length);
            
            Stat stat = stats[1];
            Assert.Equal(email, stat.uploadedBy);
            Assert.Equal(2, stat.fileCount);
            Assert.Equal(54910, stat.totalFileSize);
            Assert.Equal(new Decimal(132.78), stat.totalAmount);
            Assert.Equal(new Decimal(0.0), stat.totalAmountDue);            
        }
    }
}