using Microsoft.AspNetCore.Http;
using System;

namespace xero.Outputs
{
    public class UploadResult
    {
        public long id { get; set; }

        public bool success { get; set; }

        public string message { get; set; }
    }
}
