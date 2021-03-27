namespace EBind.MethodDelegates
{
    internal delegate void RefAction<TTarget>(ref TTarget obj) where TTarget : struct;

    internal delegate void RefAction<TTarget, T1>(ref TTarget obj, T1? arg1) where TTarget : struct;

    internal delegate void RefAction<TTarget, T1, T2>(ref TTarget obj, T1? arg1, T2? arg2) where TTarget : struct;

    internal delegate void RefAction<TTarget, T1, T2, T3>(ref TTarget obj, T1? arg1, T2? arg2, T3? arg3) where TTarget : struct;

    internal delegate void RefAction<TTarget, T1, T2, T3, T4>(ref TTarget obj, T1? arg1, T2? arg2, T3? arg3, T4? arg4) where TTarget : struct;
}
