using FabricOwl.Rules;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FabricOwl
{
    public class Transformations
    {
        public static Dictionary<string, Func<string, string, string>> transformationKey = new Dictionary<string, Func<string, string, string>> {
            { "trimFront", Transformations.trimFront },
            { "trimBack", Transformations.trimBack },
            { "prefix", Transformations.prefix },
            { "trimWhiteSpace", (parsed, value) => Transformations.trimWhiteSpace(parsed) }
        };
        public static string trimFront(string parsed, string value)
        {
            int index = parsed.IndexOf(value);
            if(index == -1)
            {
                return parsed;
            }
            return parsed.Substring(index + 1);
        }

        public static string trimBack(string parsed, string value)
        {
            int index = parsed.IndexOf(value);
            if (index == -1)
            {
                return parsed;
            }
            return parsed.Substring(0, index);
        }

        public static string trimWhiteSpace(string parsed) {
            return parsed.Trim();
        }

        public static string prefix(string parsed, string value) {
            return value + parsed;
        }

        public static string getTransformations(IEnumerable<Transform> transformations, string parsed)
        {
            foreach(Transform transform in transformations)
            {
                string func = transform.Type;
                object value = transform.Value;
                if (transformationKey.ContainsKey(func))
                {
                    parsed = transformationKey[func](parsed, (string)value);
                } else
                {
                    throw new Exception("Method " + func + " is not implemented.");
                }
            }
            return parsed;
        }
    }
}
