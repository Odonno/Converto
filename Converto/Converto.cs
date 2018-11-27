namespace Converto
{
    public static class Converto
    {
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
            // TODO
        }
        public static bool TryConvertTo<T>(this object @object, out T result) where T : class
        {
            // TODO
        }
    }
}
