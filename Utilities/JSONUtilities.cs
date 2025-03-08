using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Utilities
{
    public static class JSONUtilities
    {
        public static bool DoesJSONContainField(JObject json, string fieldname)
        {
            if (json[fieldname] == null) return false;
            if (json[fieldname].ToString() == "") return false;
            if (json[fieldname].ToString() == " ") return false;
            return true;
        }
        public static bool DoesJSONContainField(JObject json, List<string> fieldnames)
        {
            foreach (string name in fieldnames) { if (!DoesJSONContainField(json, name)) return false; } 
            return true;
        }
    }
}
