using System;
using System.Collections.Generic;
using System.Text;

namespace Converto
{
    public static partial class Main
    {
        /// <summary>
        /// Create a dictionary from the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the blueprint object.</typeparam>
        /// <param name="object">The object to use to create the dictionary.</param>
        /// <returns>Returns a dictionary with each object's property as a key/value pair.</returns>
		public static Dictionary<string, object> ToDictionary<T>(this T @object) where T : class
        {
            if (@object == null)
                return default;

            var cachedTypeInfo = GetCachedTypeInfo(typeof(T));
            var result = new Dictionary<string, object>();

            foreach (var property in cachedTypeInfo.ReadableProperties)
            {
                result.Add(
                    property.Name,
                    property.GetValue(@object)
                );
            }

            return result;
        }
    }
}
