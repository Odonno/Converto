using System;
using System.Linq;

namespace Converto
{
    public static partial class Main
    {
        /// <summary>
        /// The IsDeepEqual function detects if two objects have strictly the same properties (not necessarily the same object).
        /// </summary>
        /// <typeparam name="T">The type of objects that are compared.</typeparam>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Returns true if all properties of the object are the same.</returns>
        public static bool IsDeepEqual<T>(this T x, T y) where T : class
        {
            if (x == y)
                return true;

            if (x == null || y == null)
                return false;

            var leftType = x.GetType();
            var equatableType = leftType.GetInterface(typeof(IEquatable<>).Name)?.GenericTypeArguments
                .SingleOrDefault();

            var rightType = y.GetType();

            if (equatableType == rightType)
            {
                return x.Equals(y);
            }

            var properties = GetCachedTypeInfo(typeof(T))
                .Properties
                .Where(p => p.CanRead);

            foreach (var property in properties)
            {
                var leftValue = property.GetValue(x, null);
                var rightValue = property.GetValue(y, null);

                if (property.PropertyType.IsClass && property.PropertyType.IsNested)
                {
                    if (!IsDeepEqual(leftValue, rightValue))
                        return false;
                }
                else
                {
                    if (leftValue != rightValue)
                        return false;
                }
            }

            return true;
        }
    }
}
