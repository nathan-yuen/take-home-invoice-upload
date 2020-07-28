using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using xero.Inputs;
using xero.Models;
using xero.Outputs;
using xero.Parsers;
using xero.Stores;

namespace xero.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger)
        {
            _logger = logger;
        }

        private readonly EmailAddressAttribute emailChecker = new EmailAddressAttribute();


        [HttpPost]
        public ActionResult<UploadResult> UploadDocument([FromForm] UploadInput input)
        {
            var filePath = Path.GetTempFileName();

            if (input.email == null || input.email.Trim().Equals("")) {
                return new UploadResult {
                    success = false,
                    message = "Please provide email."
                };
            } 

            if (!emailChecker.IsValid(input.email.Trim())) {
                return new UploadResult {
                    success = false,
                    message = $"{input.email.Trim()} is an invalid email."
                };
            }

            if (input.file != null && input.file.Length > 0) {
                // Could swap out parser based on document type (hubdoc etc.)
                // Document type could be user specified or deduced?
                HDInvoiceParser parser = new HDInvoiceParser();
                Document document = null;

                try {
                    document = parser.Parse(input.file.FileName, input.file.OpenReadStream());
                    document.uploadedTimeStamp = DateTime.Now;
                    document.uploadedBy = input.email.Trim();
                    document.fileSize = input.file.Length;
                } catch(Exception e) {
                    Debug.WriteLine(e);
                    return new UploadResult {
                        success = false,
                        message = "File might be invalid, please verify tempalte configuration."
                    };
                }

                long newId = DocumentStore.Instance.AddDocument(document);

                return new UploadResult {
                    id = newId,
                    success = true
                };
            } else {
                return new UploadResult {
                    success = false,
                    message = "File is empty."
                };
            }
        }
    }
}
