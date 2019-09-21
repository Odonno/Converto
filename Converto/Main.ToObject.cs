using System;
using System.Collections.Generic;
using System.Linq;

namespace Converto
{
    public static partial class Main
    {
        /// <summary>
        /// Create an object from the specified dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="dictionary">The dictionary of key/value pairs to become the properties of the object.</param>
        /// <param name="recursive">Indicates if we convert nested dictionary in object recursively.</param>
        /// <returns>Returns a new object filled with properties from the dictionary.</returns>
		public static T ToObject<T>(this Dictionary<string, object> dictionary, bool recursive = false) where T : class
        {
            return ToObject(dictionary, typeof(T), recursive) as T;
        }
        /// <summary>
        /// Create an object from the specified dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="dictionary">The dictionary of key/value pairs to become the properties of the object.</param>
        /// <param name="result">Returns the result of the function.</param>
        /// <param name="recursive">Indicates if we convert nested dictionary in object recursively.</param>
        /// <returns>Returns true if the ToObject function succeed.</returns>
        public static bool TryToObject<T>(this Dictionary<string, object> dictionary, out T result, bool recursive = false) where T : class
        {
            result = dictionary.ToObject<T>(recursive);
            return result != null;
        }

        private static object ToObject(Dictionary<string, object> dictionary, Type objectType, bool recursive = false)
        {
            if (dictionary == null)
                return default;

            var cachedTypeInfo = GetCachedTypeInfo(objectType);

            var cachePublicConstructor = cachedTypeInfo.CachedPublicConstructors
                                                  .OrderByDescending(cc => cc.Parameters.Count)
                                                  .FirstOrDefault();

            if (cachePublicConstructor == null)
                return null;

            var sourceReadProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.WriteOnlyProperties).ToList();
            var constructorParameters = cachePublicConstructor.Parameters;

            var constructorParameterValues =
                GetConstructorParameterValuesFromDictionary(dictionary, sourceReadProperties, constructorParameters);

            if (constructorParameterValues.Length != constructorParameters.Count)
                return null;

            var newObject = Activator.CreateInstance(objectType, constructorParameterValues);
            var destWriteProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.ReadOnlyProperties);

            var propertiesToOverwrite = sourceReadProperties
                                           .Select(p => destWriteProperties.FirstOrDefault(x => p.Name == x.Name))
                                           .Where(x => x != null);

            foreach (var propertyToOverwrite in propertiesToOverwrite)
            {
                if (dictionary.ContainsKey(propertyToOverwrite.Name))
                {
                    if (recursive)
                    {
                        bool hasNestedProperties =
                            !propertyToOverwrite.PropertyType.IsPrimitive &&
                            propertyToOverwrite.PropertyType != typeof(string) &&
                            (!propertyToOverwrite.PropertyType.IsGenericType || propertyToOverwrite.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>));

                        var dictionaryValue = dictionary[propertyToOverwrite.Name];

                        if (hasNestedProperties && dictionaryValue is Dictionary<string, object> nestedDictionary)
                        {
                            propertyToOverwrite.SetValue(
                                newObject,
                                ToObject(
                                    nestedDictionary,
                                    propertyToOverwrite.PropertyType,
                                    recursive
                                )
                            );
                        }
                        else
                        {
                            propertyToOverwrite.SetValue(
                               newObject,
                               dictionary[propertyToOverwrite.Name]
                           );
                        }
                    }
                    else
                    {
                        propertyToOverwrite.SetValue(
                            newObject,
                            dictionary[propertyToOverwrite.Name]
                        );
                    }
                }
            }

            return newObject;
        }
    }
}
