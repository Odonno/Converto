using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Converto
{
    public static class Main
    {
        private static readonly Dictionary<string, CachedTypeInfo> CachedTypeInfoDetails =
            new Dictionary<string, CachedTypeInfo>();

        public static T Copy<T>(this T @object) where T : class
        {
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
        public static bool TryCopy<T>(this T @object, out T result) where T : class
        {
            result = Copy(@object);
            return result != null;
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
                    CopyPropertyValue(@object, objectPropertyInfo, result, targetPropertyInfo);
                }
            }

            return (T)result;
        }
        public static bool TryConvertTo<T>(this object @object, out T result) where T : class
        {
            result = ConvertTo<T>(@object);
            return result != null;
        }

        private static object[] GetConstructorParameterValuesForCopy<T>(
            T @object,
            IEnumerable<PropertyInfo> sourceReadProperties,
            IEnumerable<ParameterInfo> constructorParameters
        )
        {
            return constructorParameters.Select(p => sourceReadProperties.FirstOrDefault(x => AreLinked(x, p)))
                                        .Where(x => x != null)
                                        .Select(sourceReadProperty => sourceReadProperty.GetValue(@object, null))
                                        .ToArray();
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

        private static bool AreLinked(MemberInfo memberInfo, ParameterInfo parameterInfo) =>
            string.Equals(memberInfo.Name, parameterInfo.Name, StringComparison.CurrentCultureIgnoreCase);

        private static bool AreLinked(MemberInfo memberInfo, PropertyInfo propertyInfo) =>
            string.Equals(memberInfo.Name, propertyInfo.Name, StringComparison.CurrentCultureIgnoreCase);

        private static bool AreLinked(ParameterInfo parameterInfo, PropertyInfo propertyInfo) =>
            string.Equals(parameterInfo.Name, propertyInfo.Name, StringComparison.CurrentCultureIgnoreCase);

        private static void CopyPropertyValue<T>(T from, PropertyInfo property, T to) where T : class
            => property.SetValue(to, property.GetValue(from, null));

        private static void CopyPropertyValue<T1, T2>(
            T1 from, PropertyInfo fromProperty,
            T2 to, PropertyInfo toProperty
        ) 
            where T1 : class where T2 : class
        {
            toProperty.SetValue(to, fromProperty.GetValue(from, null));
        }
    }
}
