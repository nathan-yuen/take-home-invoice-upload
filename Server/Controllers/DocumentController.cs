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
    [Route("[controller]/{id}")]
    public class DocumentController : ControllerBase {
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<Document> byId(int id) {
            return DocumentStore.Instance.FindById(id);
        }
    }
}
