using System;
using System.Text;

namespace Kofax.Base64ConnectorV1
{
    public class JsonBuilder
    {
        public static string BuildJson(string[,] keyValuePairs)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            string key, value, confidence;

            jsonBuilder.Append("{\"FIELD1\":[");

            for (int i = 0; i < keyValuePairs.GetLength(0); i++)
            {
                key = keyValuePairs[i, 0];
                value = keyValuePairs[i, 1];
                confidence = keyValuePairs[i, 2];

                jsonBuilder.Append("{");
                //jsonBuilder.Append("\"kvp\": {");
                jsonBuilder.Append($"\"Key\": \"{EscapeJsonString(key)}\", \"Value\": \"{EscapeJsonString(value)}\", \"Confidence\": \"{EscapeJsonString(confidence)}\"");
                //jsonBuilder.Append("}");
                jsonBuilder.Append("}");

                if (i < keyValuePairs.GetLength(0) - 1)
                {
                    jsonBuilder.Append(",");
                }
            }

            jsonBuilder.Append("]}");

            return jsonBuilder.ToString();
        }

        private static string EscapeJsonString(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '\\':
                        result.Append("\\\\");
                        break;
                    case '"':
                        result.Append("\\\"");
                        break;
                    case '\b':
                        result.Append("\\b");
                        break;
                    case '\f':
                        result.Append("\\f");
                        break;
                    case '\n':
                        result.Append("\\n");
                        break;
                    case '\r':
                        result.Append("\\r");
                        break;
                    case '\t':
                        result.Append("\\t");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }
            return result.ToString();
        }
    }
}
