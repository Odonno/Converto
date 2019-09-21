using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Converto
{
    public static partial class Main
    {
        private static readonly ConcurrentDictionary<string, CachedTypeInfo> _cachedTypeInfoDetails =
            new ConcurrentDictionary<string, CachedTypeInfo>();

        private static object[] GetConstructorParameterValuesFromObject<T>(
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
        private static object[] GetConstructorParameterValuesFromDictionary(
            Dictionary<string, object> dictionary,
            IEnumerable<PropertyInfo> sourceReadProperties,
            IEnumerable<ParameterInfo> constructorParameters
        )
        {
            return constructorParameters.Select(p => sourceReadProperties.FirstOrDefault(x => AreLinked(x, p)))
                                        .Where(x => x != null)
                                        .Select(sourceReadProperty =>
                                        {
                                            if (dictionary.ContainsKey(sourceReadProperty.Name))
                                            {
                                                return dictionary[sourceReadProperty.Name];
                                            }

                                            return sourceReadProperty.PropertyType.IsValueType 
                                                ? Activator.CreateInstance(sourceReadProperty.PropertyType)
                                                : null;
                                        })
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
            => _cachedTypeInfoDetails.GetOrAdd(type.FullName, _ => new CachedTypeInfo(type));

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
