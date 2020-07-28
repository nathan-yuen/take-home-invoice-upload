using Microsoft.AspNetCore.Http;
using System;

namespace xero.Inputs
{
    public class UploadInput
    {
        public IFormFile file { get; set; }

        public String email { get; set; }
    }
}
