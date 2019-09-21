using System;
using System.Collections.Generic;

namespace Converto
{
    public static partial class Main
    {
        /// <summary>
        /// Create a dictionary from the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the blueprint object.</typeparam>
        /// <param name="object">The object to use to create the dictionary.</param>
        /// <param name="recursive">Indicates if we convert nested object in dictionary recursively.</param>
        /// <returns>Returns a dictionary with each object's property as a key/value pair.</returns>
		public static Dictionary<string, object> ToDictionary<T>(this T @object, bool recursive = false) where T : class
        {
            return ToDictionary(@object, typeof(T), recursive);
        }
        private static Dictionary<string, object> ToDictionary(object @object, Type objectType, bool recursive = false)
        {
            if (@object == null)
                return default;

            var cachedTypeInfo = GetCachedTypeInfo(objectType);
            var result = new Dictionary<string, object>();

            foreach (var property in cachedTypeInfo.ReadableProperties)
            {
                if (recursive)
                {
                    bool hasNestedDictionary = 
                        !property.PropertyType.IsPrimitive && 
                        property.PropertyType != typeof(string) &&
                        (!property.PropertyType.IsGenericType || property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>));

                    if (hasNestedDictionary)
                    {
                        result.Add(
                            property.Name,
                            ToDictionary(property.GetValue(@object), property.PropertyType, true)
                        );
                    }
                    else
                    {
                        result.Add(
                            property.Name,
                            property.GetValue(@object)
                        );
                    }
                }
                else
                {
                    result.Add(
                        property.Name,
                        property.GetValue(@object)
                    );
                }
            }

            return result;
        }
    }
}
