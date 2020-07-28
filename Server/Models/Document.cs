using System;
using System.Reflection;

namespace xero.Models
{
    public class Document {
        public static PropertyInfo[] Properties = typeof(Document).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        public long id { get; set; }

        public string uploadedBy { get; set; }

        public DateTime uploadedTimeStamp { get; set; }

        public long fileSize { get; set; }

        public string vendorName { get; set; }

        public DateTime invoiceDate { get; set; }

        public decimal totalAmount { get; set; }

        public decimal totalAmountDue { get; set; }

        public string currency { get; set; }

        public decimal taxAmount { get; set; }

        public String processingStatus { get; set; }
    }
}
