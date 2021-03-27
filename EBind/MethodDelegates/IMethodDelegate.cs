namespace EBind.MethodDelegates
{
    internal interface IMethodDelegate
    {
        object? Invoke(params object?[] args);
    }
}
