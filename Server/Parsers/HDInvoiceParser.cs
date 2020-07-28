using IronPdf;
using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace xero.Parsers {
    class HDInvoiceParser : PdfParserBase {
        public override string GetDocType() {
            return "hubdoc";
        }

        public override string GetVersion() {
            return "1.0";
        }

        public override string GetLineSeparator() {
            return "\r\n";
        }
    }
}