using IronPdf;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using xero.Models;
using xero.Inputs;
using xero.Parsers;
using xero.Stores;

namespace xero.Controllers
{
    [ApiController]
    [Route("document/list")]
    public class DocumentListController : ControllerBase {
        private readonly ILogger<DocumentListController> _logger;

        public DocumentListController(ILogger<DocumentListController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<Document>> list(string uploadedBy) {
            return DocumentStore.Instance.FindAllByUploadedBy(uploadedBy);
        }
    }
}
