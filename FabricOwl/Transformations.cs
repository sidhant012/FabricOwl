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
        public static Dictionary<string, Func<string, string, string>> transformationKey = new()
        {
            { "trimFront", Transformations.TrimFront },
            { "trimBack", Transformations.TrimBack },
            { "prefix", Transformations.Prefix },
            { "trimWhiteSpace", (parsed, value) => Transformations.TrimWhiteSpace(parsed) }
        };
        public static string TrimFront(string parsed, string value)
        {
            int index = parsed.IndexOf(value);
            if(index == -1)
            {
                return parsed;
            }
            return parsed.Substring(index + 1);
        }

        public static string TrimBack(string parsed, string value)
        {
            int index = parsed.IndexOf(value);
            if (index == -1)
            {
                return parsed;
            }
            return parsed.Substring(0, index);
        }

        public static string TrimWhiteSpace(string parsed) {
            return parsed.Trim();
        }

        public static string Prefix(string parsed, string value) {
            return value + parsed;
        }

        public static string GetTransformations(IEnumerable<Transform> transformations, string parsed)
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
