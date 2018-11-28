using SuccincT.Options;

namespace Converto.SuccincT
{
    public static class Main
    {
        public static Option<T> TryCopy<T>(this T @object) where T : class
        {
            return @object.Copy().ToOption();
        }

        public static Option<T> TryWith<T, TProps>(this T itemToCopy, TProps propertiesToUpdate)
            where T : class where TProps : class
        {
            return itemToCopy.With(propertiesToUpdate).ToOption();
        }

        public static Option<T> TryConvertTo<T>(this object @object) where T : class
        {
            return @object.ConvertTo<T>().ToOption();
        }
    }
}
