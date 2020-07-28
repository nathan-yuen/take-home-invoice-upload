using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace xero.Tools {
  
  //Ideally this would be interact with live API, but 
  class ForexHelper {

    private static readonly JObject forexLookup;

    static ForexHelper() {
      StreamReader file = File.OpenText($@".\forex.json");
      JsonTextReader jsonReader = new JsonTextReader(file);
      forexLookup = JObject.Load(jsonReader);
    }

    public static decimal ToUSDAmount(decimal amount, string currency) {
      if ("USD".Equals(currency)) return amount;

      JObject entry = (JObject) forexLookup.GetValue(currency.ToLower());
      decimal inverseRate = (decimal) entry.GetValue("inverseRate");
      return inverseRate * amount;
    }
  }

}