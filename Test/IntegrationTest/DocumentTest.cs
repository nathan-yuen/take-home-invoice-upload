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
    public class DocumentTest 
        : IClassFixture<WebApplicationFactory<xero.Startup>>
    {
        private readonly WebApplicationFactory<xero.Startup> _factory;

        private readonly ITestOutputHelper _output;

        public DocumentTest(WebApplicationFactory<xero.Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        string baseUrl = "/document";

        [Fact]
        public async Task Test_UnknownDoc()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"{baseUrl}/1000");

            // Assert
            response.EnsureSuccessStatusCode();

            string jsonBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("", jsonBody);
        }

        [Fact]
        public async Task Test_GetDoc() {

            string email = "a@a.com";

            // Arrange
            var client = _factory.CreateClient();
            await UploadHelper.Upload(client, email, "invoice-4.pdf");

            // Act
            var response = await client.GetAsync($"{baseUrl}/0");

            // Assert
            response.EnsureSuccessStatusCode();
            string jsonBody = await response.Content.ReadAsStringAsync();
            var document = JsonConvert.DeserializeObject<Document>(jsonBody);
            
            Assert.Equal(0, document.id);
            Assert.Equal(email, document.uploadedBy);
            Assert.Equal("The Company", document.vendorName);
            Assert.Equal(2019, document.invoiceDate.Year);
            Assert.Equal(3, document.invoiceDate.Month);
            Assert.Equal(18, document.invoiceDate.Day);
            Assert.Equal(new Decimal(118.65), document.totalAmount);
            Assert.Equal(new Decimal(0), document.totalAmountDue);
            Assert.Equal(new Decimal(13.65), document.taxAmount);            
            Assert.Equal("CAD", document.currency);
        }
    }
}