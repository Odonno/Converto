using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Converto
{
    public static class Main
    {
        private static readonly ConcurrentDictionary<string, CachedTypeInfo> CachedTypeInfoDetails =
            new ConcurrentDictionary<string, CachedTypeInfo>();

        /// <summary>
        /// The Copy function allows you to strictly copy an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="object">Object to copy.</param>
        /// <returns>Returns a copy of the object.</returns>
        public static T Copy<T>(this T @object) where T : class
        {
            if (@object == null)
                return default(T);

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

        /// <summary>
        /// The With function allows you to create a new object by mutating some properties.
        /// </summary>
        /// <typeparam name="T">The type of the blueprint object.</typeparam>
        /// <typeparam name="TProps">The type of the anonymous object of properties to update.</typeparam>
        /// <param name="object">Blueprint object to use to create a new one.</param>
        /// <param name="propertiesToUpdate">Anonymous object with a list of properties to update.</param>
        /// <returns>Returns a new object from by mutating the properties of the origin object.</returns>
        public static T With<T, TProps>(this T @object, TProps propertiesToUpdate)
            where T : class where TProps : class
        {
            if (@object == null)
                return default(T);

            if (propertiesToUpdate == null)
                return null;

            var cachedTypeInfo = GetCachedTypeInfo(typeof(T));
            var sourceReadProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.WriteOnlyProperties).ToList();
            var updateProperties = typeof(TProps).GetRuntimeProperties().Where(x => x.CanRead).ToList();

            var constructorToUse = ConstructorToUseForWith(cachedTypeInfo, updateProperties, sourceReadProperties);

            if (constructorToUse == null)
                return null;

            var constructorParameters = constructorToUse.Parameters;
            var constructorParameterValues = MapUpdateValuesToConstructorParameters(@object,
                                                                                    propertiesToUpdate,
                                                                                    constructorParameters,
                                                                                    updateProperties,
                                                                                    sourceReadProperties);

            var destWriteProperties = cachedTypeInfo.Properties.Except(cachedTypeInfo.ReadOnlyProperties);
            var propsToSetFromUpdateData = GetPropertiesToSetFromUpdateData(updateProperties,
                                                                            constructorParameters,
                                                                            sourceReadProperties);

            var propsToSetFromSourceObject = GetPropertiesToSetFromSourceObject(sourceReadProperties,
                                                                                constructorParameters,
                                                                                propsToSetFromUpdateData,
                                                                                destWriteProperties);

            return CreateNewObjectApplyingUpdates(@object,
                                                    propertiesToUpdate,
                                                    constructorParameterValues,
                                                    propsToSetFromSourceObject,
                                                    propsToSetFromUpdateData);
        }
        /// <summary>
        /// The TryWith function allows you to create a new object by mutating some properties.
        /// </summary>
        /// <typeparam name="T">The type of the blueprint object.</typeparam>
        /// <typeparam name="TProps">The type of the anonymous object of properties to update.</typeparam>
        /// <param name="object">Blueprint object to use to create a new one.</param>
        /// <param name="propertiesToUpdate">Anonymous object with a list of properties to update.</param>
        /// <param name="result">Returns the result of the function.</param>
        /// <returns>Returns true if the With function succeed.</returns>
        public static bool TryWith<T, TProps>(this T @object, TProps propertiesToUpdate, out T result)
            where T : class where TProps : class
        {
            result = With(@object, propertiesToUpdate);
            return result != null;
        }

        /// <summary>
        /// The ConvertTo function allows you to create an object of a different type using the matching properties of another object.
        /// </summary>
        /// <typeparam name="T">The type of the new object.</typeparam>
        /// <param name="object">Blueprint object to use to convert to another type.</param>
        /// <returns>Returns an object of a different type.</returns>
        public static T ConvertTo<T>(this object @object) where T : class
        {
            if (@object == null)
                return default(T);

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

        private static T CreateNewObjectApplyingUpdates<T, TProps>(
            T itemToCopy,
            TProps propertiesToUpdate,
            object[] constructorParameterValues,
            IEnumerable<PropertyInfo> propsToSetFromSourceObject,
            IEnumerable<(PropertyInfo Value, PropertyInfo PropToUpdate)> propsToSetFromUpdateData
        )
            where T : class where TProps : class
        {
            var newObject = Activator.CreateInstance(typeof(T), constructorParameterValues) as T;

            foreach (var propertyToOverwrite in propsToSetFromSourceObject)
            {
                CopyPropertyValue(itemToCopy, propertyToOverwrite, newObject);
            }

            foreach (var (sourceProp, propToUpdate) in propsToSetFromUpdateData)
            {
                CopyPropertyValue(propertiesToUpdate, propToUpdate, newObject, sourceProp);
            }

            return newObject;
        }

        private static List<PropertyInfo> GetPropertiesToSetFromSourceObject(
            IEnumerable<PropertyInfo> sourceReadProperties,
            List<ParameterInfo> constructorParameters,
            List<(PropertyInfo Value, PropertyInfo PropToUpdate)> propsToSetFromUpdateData,
            IEnumerable<PropertyInfo> destWriteProperties)
        {
            return sourceReadProperties
                   .Where(p => !constructorParameters.Any(cp => AreLinked(cp, p)))
                   .Where(p => !propsToSetFromUpdateData.Any(tp => AreLinked(p, tp.PropToUpdate)))
                   .Select(sourceProperty =>
                               destWriteProperties.FirstOrDefault(
                                   destProperty => AreLinked(sourceProperty, destProperty)))
                   .Where(x => x != null)
                   .ToList();
        }

        private static List<(PropertyInfo Value, PropertyInfo PropToUpdate)> GetPropertiesToSetFromUpdateData(
            IEnumerable<PropertyInfo> updateProperties,
            List<ParameterInfo> constructorParameters,
            List<PropertyInfo> sourceReadProperties)
        {
            return updateProperties
                   .Where(p => !constructorParameters.Any(cp => AreLinked(cp, p)))
                   .Select(propToUpdate =>
                               (
                               SourceProp: sourceReadProperties.FirstOrDefault(
                                   sp => AreLinked(sp, propToUpdate)),
                               PropToUpdate: propToUpdate
                               )
                   )
                   .Where(x => x.SourceProp != null && x.SourceProp.CanWrite)
                   .Select(x => (x.SourceProp, x.PropToUpdate)).ToList();
        }

        private static object[] MapUpdateValuesToConstructorParameters<T, TProps>(
           T @object,
           TProps propertiesToUpdate,
           List<ParameterInfo> constructorParameters,
           List<PropertyInfo> updateProperties,
           List<PropertyInfo> sourceReadProperties
        ) 
            where T : class where TProps : class
        {
            return constructorParameters
                   .Select(p =>
                   {
                       var updateProp = updateProperties.FirstOrDefault(ptu => AreLinked(ptu, p));
                       if (updateProp != null)
                       {
                           return updateProp.GetValue(propertiesToUpdate, null);
                       }

                       var sourceProp = sourceReadProperties.FirstOrDefault(sp => AreLinked(sp, p));
                       if (sourceProp != null)
                       {
                           return sourceProp.GetValue(@object, null);
                       }

                       return null;
                   })
                   .Where(x => x != null)
                   .ToArray();
        }

        private static CachedConstructorInfo ConstructorToUseForWith(
            CachedTypeInfo cachedTypeInfo,
            IEnumerable<PropertyInfo> updateProperties,
            IEnumerable<PropertyInfo> readProperties
        )
        {
            return (from constructor in cachedTypeInfo.CachedPublicConstructors
                    let paramsNotCoveredByUpdates =
                        constructor.Parameters.Where(p => !updateProperties.Any(ptu => AreLinked(ptu, p)))
                    let remainingParamsNotCoveredByProperties =
                        paramsNotCoveredByUpdates.Where(p => !readProperties.Any(rp => AreLinked(rp, p))).ToList()
                    where !remainingParamsNotCoveredByProperties.Any()
                    orderby constructor.Parameters.Count descending
                    select constructor).FirstOrDefault();
        }

        private static CachedTypeInfo GetCachedTypeInfo(Type type)
            => CachedTypeInfoDetails.GetOrAdd(type.FullName, _ => new CachedTypeInfo(type));

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
