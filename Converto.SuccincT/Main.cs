﻿using SuccincT.Options;
using System.Collections.Generic;

namespace Converto.SuccincT
{
    public static class Main
    {
        /// <summary>
        /// The TryCopy function allows you to strictly copy an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to copy.</typeparam>
        /// <param name="object">Object to copy.</param>
        /// <returns>Returns a copy of the object.</returns>
        public static Option<T> TryCopy<T>(this T @object) where T : class
        {
            return @object.Copy().ToOption();
        }

        /// <summary>
        /// The TryWith function allows you to create a new object by mutating some properties.
        /// </summary>
        /// <typeparam name="T">The type of the blueprint object.</typeparam>
        /// <typeparam name="TProps">The type of the anonymous object of properties to update.</typeparam>
        /// <param name="object">Blueprint object to use to create a new one.</param>
        /// <param name="propertiesToUpdate">Anonymous object with a list of properties to update.</param>
        /// <returns>Returns a new object from by mutating the properties of the origin object.</returns>
        public static Option<T> TryWith<T, TProps>(this T @object, TProps propertiesToUpdate)
            where T : class where TProps : class
        {
            return @object.With(propertiesToUpdate).ToOption();
        }

        /// <summary>
        /// The TryConvertTo function allows you to create an object of a different type using the matching properties of another object.
        /// </summary>
        /// <typeparam name="T">The type of the new object.</typeparam>
        /// <param name="object">Blueprint object to use to convert to another type.</param>
        /// <returns>Returns an object of a different type.</returns>
        public static Option<T> TryConvertTo<T>(this object @object) where T : class
        {
            return @object.ConvertTo<T>().ToOption();
        }

        /// <summary>
        /// The TryToObject function allows you to create an object from the specified dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the object to create.</typeparam>
        /// <param name="dictionary">The dictionary of key/value pairs to become the properties of the object.</param>
        /// <param name="recursive">Indicates if we convert nested dictionary in object recursively.</param>
        /// <returns>Returns a new object filled with properties from the dictionary.</returns>
        public static Option<T> TryToObject<T>(this Dictionary<string, object> dictionary, bool recursive = false) where T : class
        {
            return dictionary.ToObject<T>(recursive).ToOption();
        }
    }
}
