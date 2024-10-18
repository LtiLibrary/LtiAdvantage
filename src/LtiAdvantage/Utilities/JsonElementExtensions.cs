using System.Collections.Generic;
using System.Text.Json;

namespace LtiAdvantage.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonElementExtensions
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool TryGetString(this JsonElement source, string propertyName, out string item)
        {
            if(source.TryGetProperty(propertyName, out var property))
            {
                item = property.GetString();
                return true;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<string> ToStringList(this JsonElement source)
        {
            if (source.ValueKind != JsonValueKind.Array)
            {
                return null;
            }

            var items = new List<string>();

            // Enumerate the array and add each string to the list
            foreach (JsonElement item in source.EnumerateArray())
            {
                items.Add(item.ToString());
            }

            return items;
        }
    }
}
