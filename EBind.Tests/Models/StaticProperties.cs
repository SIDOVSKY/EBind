namespace EBind.Tests
{
    /// <summary>
    /// Its properties should be set from one test only to avoid side effects
    /// </summary>
    internal static class StaticProperties
    {
        public static int Int33 { get; set; } = 33;

        public static int Int33ToInvoke { get; set; } = 33;

        public static string HelloToInvoke { get; set; } = "Hello";

        public static string StringEmpty { get; set; } = string.Empty;
    }
}
