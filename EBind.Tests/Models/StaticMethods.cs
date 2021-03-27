namespace EBind.Tests
{
    internal static class StaticMethods
    {
        public static T JustReturn<T>(T value) => value;

        public static void Append0(ref string value)
        {
            value += "0";
        }

        public static int Return33() => 33;
    }
}
