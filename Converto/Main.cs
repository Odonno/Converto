using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Converto
{
    public static class Main
    {
        private static readonly Dictionary<string, CachedTypeInfo> CachedTypeInfoDetails =
            new Dictionary<string, CachedTypeInfo>();

        public static T Copy<T>(this T @object) where T : class
        {
            // TODO
        }
        public static bool TryCopy<T>(this T @object, out T result) where T : class
        {
            // TODO
        }

        public static T With<T, TProps>(this T itemToCopy, TProps propertiesToUpdate)
            where T : class where TProps : class
        {
            // TODO
        }
        public static bool TryWith<T, TProps>(this T itemToCopy, TProps propertiesToUpdate, out T result)
            where T : class where TProps : class
        {
            // TODO
        }

        public static T ConvertTo<T>(this object @object) where T : class
        {
            if (@object == null)
                return default(T);

            if (@object is JObject jsonObject)
                return jsonObject.ToObject<T>();

            var objectTypeInfo = GetCachedTypeInfo(@object.GetType());
            var targetTypeInfo = GetCachedTypeInfo(typeof(T));

            var result = Activator.CreateInstance(objectTypeInfo.Type, false);

            var targetPropertiesInfo = targetTypeInfo.Properties.Where(p => p.CanWrite);

            foreach (var targetPropertyInfo in targetPropertiesInfo)
            {
                var objectPropertyInfo = objectTypeInfo.Properties.FirstOrDefault(p => p.Name == targetPropertyInfo.Name && p.CanRead);
                if (objectPropertyInfo != null)
                {
                    var value = objectPropertyInfo.GetValue(@object, null);
                    targetPropertyInfo.SetValue(result, value, null);
                }
            }

            return (T)result;
        }
        public static bool TryConvertTo<T>(this object @object, out T result) where T : class
        {
            result = ConvertTo<T>(@object);
            return result != null;
        }

        private static CachedTypeInfo GetCachedTypeInfo(Type type)
            => CachedTypeInfoDetails.GetOrAddValue(type.FullName, () => new CachedTypeInfo(type));

        private static T GetOrAddValue<T>(this Dictionary<string, T> dictionary, string key, Func<T> createValue)
        {
            if (dictionary.TryGetValue(key, out T value))
            {
                return value;
            }

            value = createValue();
            dictionary.Add(key, value);
            return value;
        }
    }
}
