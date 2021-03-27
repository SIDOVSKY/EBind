using System;

namespace EBind.Test.Models
{
    public static class LambdaInvoker
    {
        public static T Invoke<T>(Func<T> func)
        {
            return func.Invoke();
        }
    }
}
