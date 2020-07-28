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
    [Route("[controller]")]
    public class StatsController : ControllerBase
    {
        private readonly ILogger<StatsController> _logger;

        public StatsController(ILogger<StatsController> logger)
        {
            _logger = logger;
        }

        private readonly EmailAddressAttribute emailChecker = new EmailAddressAttribute();


        [HttpGet]
        public ActionResult<List<Stat>> GetStats() {
            return DocumentStore.Instance.GetDocumentStats();
        }
    }
}
