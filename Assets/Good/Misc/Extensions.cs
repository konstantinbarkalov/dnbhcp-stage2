namespace System.Runtime.CompilerServices
{
    // https://stackoverflow.com/a/64749403/16593275
    internal static class IsExternalInit { }
}

namespace System
{

    public static class ArrayExtensions
    {

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

    }

}
