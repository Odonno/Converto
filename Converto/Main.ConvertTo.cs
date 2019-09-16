using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Converto
{
    public static partial class Main
    {
        /// <summary>
        /// The ConvertTo function allows you to create an object of a different type using the matching properties of another object.
        /// </summary>
        /// <typeparam name="T">The type of the new object.</typeparam>
        /// <param name="object">Blueprint object to use to convert to another type.</param>
        /// <returns>Returns an object of a different type.</returns>
        public static T ConvertTo<T>(this object @object) where T : class
        {
            if (@object == null)
                return default;

            if (@object is JObject jsonObject)
                return jsonObject.ToObject<T>();

            var objectTypeInfo = GetCachedTypeInfo(@object.GetType());
            var targetTypeInfo = GetCachedTypeInfo(typeof(T));

            var result = Activator.CreateInstance(targetTypeInfo.Type, false);

            var targetPropertiesInfo = targetTypeInfo.Properties.Where(p => p.CanWrite);

            foreach (var targetPropertyInfo in targetPropertiesInfo)
            {
                var objectPropertyInfo = objectTypeInfo.Properties.FirstOrDefault(p => p.Name == targetPropertyInfo.Name && p.CanRead);
                if (objectPropertyInfo != null)
                {
                    CopyPropertyValue(@object, objectPropertyInfo, result, targetPropertyInfo);
                }
            }

            return (T)result;
        }
        /// <summary>
        /// The TryConvertTo function allows you to create an object of a different type using the matching properties of another object.
        /// </summary>
        /// <typeparam name="T">The type of the new object.</typeparam>
        /// <param name="object">Blueprint object to use to convert to another type.</param>
        /// <param name="result">Returns the result of the function.</param>
        /// <returns>Returns true if the ConvertTo function succeed.</returns>
        public static bool TryConvertTo<T>(this object @object, out T result) where T : class
        {
            result = ConvertTo<T>(@object);
            return result != null;
        }
    }
}
