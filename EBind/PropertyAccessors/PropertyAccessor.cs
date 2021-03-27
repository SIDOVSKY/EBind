using System.Reflection;

namespace EBind.PropertyAccessors
{
    internal class RefTargetRefPropertyAccessor : PropertyAccessor<object, object>
    {
        public RefTargetRefPropertyAccessor(PropertyInfo info) : base(info)
        {
        }
    }

    // Separate type for nint AOT support
    // AOT compiler cannot compile set method for nint with generic target.
    // It needs exact PropertyAccessor<UIPageControl, nint>.
    [Platform.Linker.Preserve(AllMembers = true)]
    internal class RefTargetPropertyAccessor<TProperty> : PropertyAccessor<object, TProperty>
    {
        public RefTargetPropertyAccessor(PropertyInfo info) : base(info)
        {
        }
    }

    [Platform.Linker.Preserve(AllMembers = true)]
    // Using C# 9 function pointers because Delegate.CreateDelegate does not fully support virtual methods in iOS AOT –
    // throws `System.ExecutionEngineException : Attempting to JIT compile method '(wrapper delegate-invoke) void <Module>:invoke_callvirt_...`
    internal unsafe class PropertyAccessor<TTarget, TProperty> : IAccessor
    {
        private delegate*<TProperty?> _staticGetter;
        private delegate*<TTarget, TProperty?> _instanceGetter;
        private delegate*<TProperty?, void> _staticSetter;
        private delegate*<TTarget, TProperty?, void> _instanceSetter;

        public PropertyAccessor(PropertyInfo info)
        {
            CreateGetter(info);
            CreateSetter(info);
        }

        private void CreateGetter(PropertyInfo info)
        {
            var method = info.GetMethod;
            var methodPointer = method.MethodHandle.GetFunctionPointer();

            if (method.IsStatic)
            {
                _staticGetter = (delegate*<TProperty?>)methodPointer;
            }

            _instanceGetter = (delegate*<TTarget, TProperty?>)methodPointer;
        }

        private void CreateSetter(PropertyInfo info)
        {
            var method = info.SetMethod;

            if (method is null)
                return;

            var methodPointer = method.MethodHandle.GetFunctionPointer();

            if (method.IsStatic)
            {
                _staticSetter = (delegate*<TProperty?, void>)methodPointer;
            }

            _instanceSetter = (delegate*<TTarget, TProperty?, void>)methodPointer;
        }

        public object? Get(object? obj)
        {
            if (_staticGetter != null)
                return _staticGetter();

            return _instanceGetter((TTarget)obj!);
        }

        public void Set(object? obj, object? value)
        {
            if (_staticSetter != null)
            {
                _staticSetter((TProperty)value);
                return;
            }

            if (_instanceSetter != null)
            {
                _instanceSetter((TTarget)obj!, (TProperty)value);
            }
        }
    }
}
