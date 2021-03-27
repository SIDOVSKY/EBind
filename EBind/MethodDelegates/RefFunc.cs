namespace EBind.MethodDelegates
{
    internal delegate TReturn RefFunc<TTarget, TReturn>(ref TTarget obj) where TTarget : struct;

    internal delegate TReturn RefFunc<TTarget, T1, TReturn>(ref TTarget obj, T1? arg1) where TTarget : struct;

    internal delegate TReturn RefFunc<TTarget, T1, T2, TReturn>(ref TTarget obj, T1? arg1, T2? arg2) where TTarget : struct;

    internal delegate TReturn RefFunc<TTarget, T1, T2, T3, TReturn>(ref TTarget obj, T1? arg1, T2? arg2, T3? arg3) where TTarget : struct;

    internal delegate TReturn RefFunc<TTarget, T1, T2, T3, T4, TReturn>(ref TTarget obj, T1? arg1, T2? arg2, T3? arg3, T4? arg4) where TTarget : struct;
}
