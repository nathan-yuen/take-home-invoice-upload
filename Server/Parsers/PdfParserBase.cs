using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using xero.Models;

namespace xero.Parsers {
    abstract class PdfParserBase {

        JObject config;

        public abstract string GetDocType();

        public abstract string GetVersion();

        public JObject GetConfig() {
            if (config == null) {
                StreamReader file = File.OpenText($@".\configs\{GetDocType()}\{GetVersion()}.json");
                JsonTextReader jsonReader = new JsonTextReader(file);
                config = JObject.Load(jsonReader);
            }
            return config;
        }

        public abstract string GetLineSeparator();

        public Document Parse(string name, Stream readStream) {
            // PdfDocument pdf = PdfDocument.Open(readStream);
            List<string> textList = new List<string>();
            using (var pdf = PdfDocument.Open(readStream)) {
                foreach (var page in pdf.GetPages()) {
                    var lines = ContentOrderTextExtractor.GetText(page, true)
                        .Split(GetLineSeparator()).Select(line => line.Trim())
                        .Where(line => !line.Equals(""));
                    textList.AddRange(lines);
                }
            }
            string[] texts = textList.ToArray();
            
            Document doc = new Document();

            // Positional lookup for "relativeTo" rule
            Dictionary<string, int> extractedPosition = new Dictionary<string, int>();

            // Get configs
            JObject lookup = GetConfig()["lookup"] as JObject;
            if (lookup is null) throw new Exception("Invalid parser config: lookup is null");

            // Use a queue for iterating through properties, since some text extraction could depend on others
            Queue<PropertyInfo> propQueue = new Queue<PropertyInfo>(Document.Properties);

            while(propQueue.Count > 0) {
                PropertyInfo docProp = propQueue.Dequeue();
                JToken config = lookup[docProp.Name];

                if (config is null) continue;

                JTokenType configType = config.Type;

                // Rules based on string matching
                if (configType.Equals(JTokenType.Object)) {
                    JObject rules = config as JObject;

                    string target = null;
                    int targetPos = -1;
                    string value = "";

                    if (rules["index"] != null) {
                        target = texts[(int) rules["index"]];                
                    } else if (rules["startsWith"] != null) {
                        JToken startsWith = rules["startsWith"];
                        if (startsWith.Type == JTokenType.String) {
                            (target, targetPos) = findLine(texts, (text => text.StartsWith((string) rules["startsWith"])));
                        } else {
                            string[] startsWithes = startsWith.ToObject<string[]>();                        
                            (target, targetPos) = findLine(texts, text => startsWithes.Any(query => text.StartsWith(query)));
                        }
                    } else if (rules["indexOf"] != null) {                        
                        targetPos = Array.IndexOf(texts, (string) rules["indexOf"]);
                        if (targetPos < 0) continue;
                        target = texts[targetPos];
                    } else if (rules["relativeTo"] != null) {
                        targetPos = extractedPosition.GetValueOrDefault((string) rules["relativeTo"], -1);

                        // Relative Member not parsed yet, requeue for processing
                        if (targetPos == -1) {
                            propQueue.Enqueue(docProp);
                            continue;
                        }

                        target = texts[targetPos];
                    }

                    // Allow user to find specific line relative to somone common string like "Total Due"
                    if (rules["offset"] != null) {
                        targetPos += (int) rules["offset"];
                        target = texts[targetPos];
                    }

                    if (target is null) continue;

                    extractedPosition.Add(docProp.Name, targetPos);

                    // Further extraction with regex if needed, select specific group of text
                    if (rules["regex"] != null) {
                        string regexPattern = (string) rules["regex"];
                        Regex regex = new Regex(regexPattern);  
                        Match matched = regex.Match(target);                    

                        int group = (int?) rules["group"] ?? 0;

                        value = matched.Groups[group].Value;
                    } else {
                        value = target;
                    }
                    
                    docProp.SetValue(doc, serializeValue(value, docProp.PropertyType));
                }
            }

            return doc;
        }

        private (string, int) findLine(string[] texts, Func<string, bool> predicate) {
            return texts.Select((text, idx) => (text, idx))
                .Where(obj => predicate(obj.text))
                .First();
        }

        protected Object serializeValue(string input, Type type) {
            if (String.IsNullOrEmpty(input)) return null;

            if (type == typeof(DateTime)) {
                return DateTime.Parse(input);
            } else if (type == typeof(decimal)) {
               return decimal.Parse(input);
            } else {
                return input;
            }
        }
    }
}