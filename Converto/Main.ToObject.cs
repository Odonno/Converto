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
        /// <returns>Returns a new object filled with properties from the dictionary.</returns>
		public static T ToObject<T>(this Dictionary<string, object> dictionary) where T : class
        {
            if (dictionary == null)
                return default;

            var cachedTypeInfo = GetCachedTypeInfo(typeof(T));

            var cachePublicConstructor = cachedTypeInfo.CachedPublicConstructors
                                                  .OrderByDescending(cc => cc.Parameters.Count)
                                                  .FirstOrDefault();

            if (cachePublicConstructor == null)
                return null;

            var sourceReadProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.WriteOnlyProperties).ToList();
            var constructorParameters = cachePublicConstructor.Parameters;

            var constructorParameterValues =
                GetConstructorParameterValuesFromDictionary<T>(dictionary, sourceReadProperties, constructorParameters);

            if (constructorParameterValues.Length != constructorParameters.Count)
                return null;

            var newObject = Activator.CreateInstance(typeof(T), constructorParameterValues) as T;
            var destWriteProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.ReadOnlyProperties);

            var propertiesToOverwrite = sourceReadProperties
                                           .Select(p => destWriteProperties.FirstOrDefault(x => p.Name == x.Name))
                                           .Where(x => x != null);

            foreach (var propertyToOverwrite in propertiesToOverwrite)
            {
                if (dictionary.ContainsKey(propertyToOverwrite.Name))
                {
                    propertyToOverwrite.SetValue(
                        newObject,
                        dictionary[propertyToOverwrite.Name]
                    );
                }
            }

            return newObject;
        }
        /// <summary>
        /// Create an object from the specified dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="dictionary">The dictionary of key/value pairs to become the properties of the object.</param>
        /// <param name="result">Returns the result of the function.</param>
        /// <returns>Returns true if the ToObject function succeed.</returns>
        public static bool TryToObject<T>(this Dictionary<string, object> dictionary, out T result) where T : class
        {
            result = dictionary.ToObject<T>();
            return result != null;
        }
    }
}
