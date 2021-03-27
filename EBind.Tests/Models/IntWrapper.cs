namespace EBind.Tests
{
    public struct IntWrapper
    {
        public int Value;

        public static explicit operator int(IntWrapper v) => v.Value;
    }
}
