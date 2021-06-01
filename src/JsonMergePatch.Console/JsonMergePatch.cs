using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace JsonMergePatch
{
    public static class JsonMergePatch
    {
        public static string Patch(object target, string patch)
        {
            return Patch(JsonSerializer.Serialize(target), patch);
        }

        public static string Patch(string target, string patch)
        {
            var targetDoc = JsonDocument.Parse(target);
            var patchDoc = JsonDocument.Parse(patch);

            return DoPatch(targetDoc.RootElement, patchDoc.RootElement);
        }

        private static string DoPatch(JsonElement targetElement, JsonElement patchElement)
        {
            var sb = new StringBuilder();

            if (patchElement.ValueKind == JsonValueKind.Object)
            {
                if (targetElement.ValueKind != JsonValueKind.Object)
                {
                    sb.Append("{}");
                }

                sb.Append('{');

                foreach (var property in patchElement.EnumerateObject())
                {
                    var propExistsOnTarget = targetElement.TryGetProperty(property.Name, out var targetProp);

                    // The value 1 here is the length of the start object token ({), which we've added above. 
                    // If the string so far is any longer than that then it means we've already written at least one property and we need to 
                    // add a comma as a separator
                    if (sb.Length > 1)
                        sb.Append(',');

                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        if (propExistsOnTarget)
                        {
                            // Remove from target (i.e. do NOT write out to writer)
                        }
                    }
                    else
                    {
                        // Pass the current value of property, and the relevant target element to DoPatch
                        var val = DoPatch(targetProp, patchElement.GetProperty(property.Name));
                        // write val
                        sb.AppendFormat("\"{0}\":{1}", property.Name, val);
                    }
                }

                sb.Append('}');
            }
            else
            {
                // write the patch value
                return patchElement.GetRawText();
            }

            return sb.ToString();
        }
    }
}
