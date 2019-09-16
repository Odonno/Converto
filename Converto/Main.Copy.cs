using System;
using System.Linq;

namespace Converto
{
    public static partial class Main
    {
        /// <summary>
        /// The Copy function allows you to strictly copy an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="object">Object to copy.</param>
        /// <returns>Returns a copy of the object.</returns>
        public static T Copy<T>(this T @object) where T : class
        {
            if (@object == null)
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
                GetConstructorParameterValuesForCopy(@object, sourceReadProperties, constructorParameters);

            if (constructorParameterValues.Length != constructorParameters.Count)
                return null;

            var newObject = Activator.CreateInstance(typeof(T), constructorParameterValues) as T;
            var destWriteProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.ReadOnlyProperties);

            var propertiesToOverwrite = sourceReadProperties
                                           .Select(p => destWriteProperties.FirstOrDefault(x => p.Name == x.Name))
                                           .Where(x => x != null);

            foreach (var propertyToOverwrite in propertiesToOverwrite)
            {
                CopyPropertyValue(@object, propertyToOverwrite, newObject);
            }

            return newObject;
        }
        /// <summary>
        /// The TryCopy function allows you to strictly copy an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="object">Object to copy.</param>
        /// <param name="result">Returns the result of the function.</param>
        /// <returns>Returns true if the Copy function succeed.</returns>
        public static bool TryCopy<T>(this T @object, out T result) where T : class
        {
            result = Copy(@object);
            return result != null;
        }
    }
}
