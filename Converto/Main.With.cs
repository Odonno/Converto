using System.Linq;
using System.Reflection;

namespace Converto
{
    public static partial class Main
    {
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
                return default;

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
    }
}
