using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace xero.Test.Tools {
  
  class UploadHelper {
    static async public Task<HttpResponseMessage> Upload(HttpClient client, string email, string fileName) {
        var requestContent = new MultipartFormDataContent(); 

        byte[] bytes = File.ReadAllBytes($@"../../../../invoices/{fileName}");

        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = 
            MediaTypeHeaderValue.Parse("application/pdf");

        var emailContent = new StringContent(email);

        requestContent.Add(fileContent, "file", $"{fileName}.pdf");
        requestContent.Add(emailContent, "email");

        return await client.PostAsync("/upload", requestContent);
    }
  }

}