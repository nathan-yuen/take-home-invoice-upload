using System;
using System.Reflection;

namespace xero.Models {
    public class Stat {
        public string uploadedBy { get; set; }

        public int fileCount { get; set; }

        public long totalFileSize { get; set; }

        public decimal totalAmount { get; set; }

        public decimal totalAmountDue { get; set; }

        public StatDocument[] documents { get; set; }
    }

    public class StatDocument {

        public long id { get; set; }

        public DateTime uploadedTimeStamp { get; set; }

        public DateTime invoiceDate { get; set; }
        
        public string vendorName { get; set; }
    }
}