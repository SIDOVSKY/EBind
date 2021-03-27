namespace EBind.PropertyAccessors
{
    internal interface IAccessor
    {
        object? Get(object? obj);

        void Set(object? obj, object? value);
    }
}
