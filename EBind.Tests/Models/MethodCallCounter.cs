namespace EBind.Tests
{
    public class MethodCallCounter
    {
        public int Count { get; set; }

        public void Action()
        {
            Count++;
        }

        public void Action<T>(T value)
        {
            Count++;
        }

        public T Return<T>(T value)
        {
            Count++;
            return value;
        }
    }
}
