using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Reflection;

using xero.Models;
using xero.Tools;

namespace xero.Stores {
    public sealed class DocumentStore {

        private static DocumentStore instance;

        private static string COLUMN_ID = "id";

        // For the ease of running this project, we are using a in-memory DB table for demo purpose
        DataTable table;

        static DocumentStore() {
            instance = new DocumentStore();
        }

        public static DocumentStore Instance {
            get {
                return instance;
            }
        }

        private DocumentStore() {
            table = new DataTable();
            
            DataColumn idCol = new DataColumn();
            idCol.DataType = System.Type.GetType("System.Int64");
            idCol.ColumnName = COLUMN_ID;
            idCol.Unique = true;
            idCol.AutoIncrement = true;
            table.Columns.Add(idCol);

            var keys = new DataColumn[1];
            keys[0] = idCol;
            table.PrimaryKey = keys;

            foreach (PropertyInfo property in Document.Properties) {
                if (property.Name.Equals(COLUMN_ID)) continue;
                DataColumn column = new DataColumn();
                column.DataType = property.PropertyType;
                column.ColumnName = property.Name;
                table.Columns.Add(column);
            }
        }

        public long AddDocument(Document newDocument) {
            DataRow row = table.NewRow();   
            foreach (PropertyInfo property in Document.Properties) {
                if (property.Name.Equals(COLUMN_ID)) continue;
                row[property.Name] = property.GetValue(newDocument);                
            }

            table.Rows.Add(row);
            return (long?) row[COLUMN_ID] ?? -1;
        }

        private Document RowToDocument(DataRow row) {
            if (row == null) return null;

            Document document = new Document();

            foreach (PropertyInfo property in Document.Properties) {
                var value = row[property.Name];
                if (value != DBNull.Value) {
                    property.SetValue(document, value);            
                }                
            }

            return document;
        }

        public Document FindById(int Id) {
            DataRow row = table.Rows.Find(Id);
            return RowToDocument(row);
        }

        public List<Document> FindAllByUploadedBy(string email) {
            return table.AsEnumerable()
                .Where(r => r["uploadedBy"].Equals(email))
                .Select(RowToDocument)
                .ToList();
        }

        public List<Stat> GetDocumentStats() {
            var results = table.AsEnumerable()
                .GroupBy(r => r.Field<string>("uploadedBy"))
                .Select(group => {
                    Stat stat = new Stat();
                    stat.uploadedBy = group.Key;
                    stat.fileCount = group.Count();
                    stat.totalFileSize = group.Sum(r => (long) r["fileSize"]);
                    stat.totalAmount = group.Sum(r => ForexHelper.ToUSDAmount((decimal) r["totalAmount"], (string) r["currency"]));
                    stat.totalAmountDue = group.Sum(r => ForexHelper.ToUSDAmount((decimal) r["totalAmountDue"], (string) r["currency"]));
                    stat.documents = group.Select(r => {
                        StatDocument doc = new StatDocument {
                            id = (long) r[COLUMN_ID],
                            uploadedTimeStamp = (DateTime) r["uploadedTimeStamp"],
                            invoiceDate = (DateTime) r["invoiceDate"],
                            vendorName = (string) r["vendorName"]
                        };
                        return doc;   
                    }).ToArray();

                    return stat;
                }).ToList();

            return results;
        }
    }
}